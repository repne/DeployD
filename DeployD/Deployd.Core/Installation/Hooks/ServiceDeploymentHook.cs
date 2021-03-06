using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.ServiceProcess;
using Deployd.Core.AgentConfiguration;
using log4net;

namespace Deployd.Core.Installation.Hooks
{
    public class ServiceDeploymentHook : DeploymentHookBase
    {
        private string _serviceInstallationPath;

        public override bool HookValidForPackage(DeploymentContext context)
        {
            return context.Package.Tags.ToLower().Contains("service");
        }

        public ServiceDeploymentHook(IFileSystem fileSystem, IAgentSettings agentSettings)
            : base(agentSettings, fileSystem)
        {
            _serviceInstallationPath = Path.Combine(agentSettings.BaseInstallationPath, "services");
        }

        public override void BeforeDeploy(DeploymentContext context)
        {
            var logger = context.GetLoggerFor(this);
            if (!EnvironmentIsValidForPackage(context))
            {
                return;
            }

            ShutdownRequiredServices(context, logger);
        }

        private void ShutdownRequiredServices(DeploymentContext context, ILog logger)
        {
            using (var service = ServiceController.GetServices().SingleOrDefault(s => s.ServiceName == context.Package.Title))
            {
                if (service == null)
                {
                    return;
                }

                // todo: recursively shut down dependent services
                if (!service.Status.Equals(ServiceControllerStatus.Running) &&
                    !service.Status.Equals(ServiceControllerStatus.StartPending))
                {
                    return;
                }

                ChangeServiceStateTo(service, ServiceControllerStatus.Stopped, service.Stop, logger);
            }
        }

        public override void Deploy(DeploymentContext context)
        {
            if (!EnvironmentIsValidForPackage(context))
            {
                return;
            }

            // services are installed in a '\services' subfolder
            context.TargetInstallationFolder = Path.Combine(_serviceInstallationPath, context.Package.Id);
            
            CopyAllFilesToDestination(context);
        }

        public override void AfterDeploy(DeploymentContext context)
        {
            var logger = context.GetLoggerFor(this);
            if (!EnvironmentIsValidForPackage(context))
            {
                return;
            }

            // if no such service then install it
            using (var service = ServiceController.GetServices().SingleOrDefault(s => s.ServiceName == context.Package.Id))
            {
                if (service == null)
                {
                    var pathToExecutable = Path.Combine(context.TargetInstallationFolder, context.Package.Id + ".exe");
                    logger.InfoFormat("Installing service {0} from {1}", context.Package.Title, pathToExecutable);

                    System.Configuration.Install.ManagedInstallerClass.InstallHelper(new[] {pathToExecutable});
                }
            }

            using (var service = ServiceController.GetServices().SingleOrDefault(s => s.ServiceName == context.Package.Id))
            {
                // todo: recursively shut down dependent services
                if (!service.Status.Equals(ServiceControllerStatus.Stopped) &&
                    !service.Status.Equals(ServiceControllerStatus.StopPending))
                {
                    return;
                }
                
                ChangeServiceStateTo(service, ServiceControllerStatus.Running, service.Start, logger);
            }
        }

        private void ChangeServiceStateTo(ServiceController service, ServiceControllerStatus verifyMeetsThisStatus, Action switchAction, ILog logger)
        {
            logger.InfoFormat("Stopping service {0}", service.ServiceName);
            switchAction();

            var retryCount = 10; // wait 10 retries
            while (service.Status != verifyMeetsThisStatus && --retryCount > 0)
            {
                System.Threading.Thread.Sleep(100);
                service.Refresh();
            }

            logger.InfoFormat("service is now {0}", service.Status);            
        }
    }
}
