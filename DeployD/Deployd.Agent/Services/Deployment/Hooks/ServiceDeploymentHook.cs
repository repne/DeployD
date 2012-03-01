using System.IO;
using System.Linq;
using System.ServiceProcess;
using Deployd.Core.AgentConfiguration;
using log4net;

namespace Deployd.Agent.Services.Deployment.Hooks
{
    public class ServiceDeploymentHook : DeploymentHookBase
    {
        private ILog _log = LogManager.GetLogger("ServiceDeployment");
        public override bool HookValidForPackage(DeploymentContext context)
        {
            return context.Package.Tags.ToLower().Contains("service");
        }

        public ServiceDeploymentHook(IAgentSettings agentSettings) : base(agentSettings)
        {
        }

        public override void BeforeDeploy(DeploymentContext context)
        {
            if (!EnvironmentIsValidForPackage(context)) return;

            using (var service = ServiceController.GetServices().SingleOrDefault(s => s.ServiceName == context.Package.Title))
            {
                // if no such service then pass
                if (service == null)
                    return;

                // shut down the service if it's installed
                // todo: recursively shut down dependent services
                if (service.Status.Equals(ServiceControllerStatus.Running)
                    || service.Status.Equals(ServiceControllerStatus.StartPending))
                {
                    _log.InfoFormat("Stopping service {0}", service.ServiceName);
                    service.Stop();

                    int waitCount = 10; // wait 10 retries
                    while (service.Status != ServiceControllerStatus.Stopped && --waitCount > 0)
                    {
                        System.Threading.Thread.Sleep(100);
                        service.Refresh();
                    }
                    _log.InfoFormat("service is now {0}", service.Status);
                }
            }
        }

        public override void Deploy(DeploymentContext context)
        {
            if (!EnvironmentIsValidForPackage(context)) return;

            // services are installed in a '\services' subfolder
            context.TargetInstallationFolder = Path.Combine(@"d:\wwwcom\services", context.Package.Title);
            
            base.Deploy(context);
        }

        public override void AfterDeploy(DeploymentContext context)
        {
            if (!EnvironmentIsValidForPackage(context)) return;

            // if no such service then install it
            using (var service = ServiceController.GetServices().SingleOrDefault(s => s.ServiceName == context.Package.Title))
            {
                if (service == null)
                {
                    string pathToExecutable = Path.Combine(context.TargetInstallationFolder,
                                                           context.Package.Title + ".exe");
                    _log.InfoFormat("Installing service {0} from {1}", context.Package.Title, pathToExecutable);

                    System.Configuration.Install.ManagedInstallerClass.InstallHelper(new[] {pathToExecutable});
                }

                // start the service if it's stopped
                // todo: recursively shut down dependent services
                if (service.Status.Equals(ServiceControllerStatus.Stopped)
                    || service.Status.Equals(ServiceControllerStatus.StopPending))
                {
                    _log.InfoFormat("Starting service {0}", service.ServiceName);
                    service.Start();

                    int waitCount = 10; // wait 10 retries
                    while(service.Status != ServiceControllerStatus.Running && --waitCount>0)
                    {
                        System.Threading.Thread.Sleep(100);
                        service.Refresh();
                    }
                    _log.InfoFormat("service is now {0}", service.Status);
                }
            }
        }
    }
}