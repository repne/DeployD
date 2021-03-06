using DeployD.Hub.Areas.Api.Code;
using Ninject.Parameters;
using Raven.Client;
using Raven.Client.Embedded;
using log4net;

[assembly: WebActivator.PreApplicationStartMethod(typeof(DeployD.Hub.App_Start.NinjectMVC3), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(DeployD.Hub.App_Start.NinjectMVC3), "Stop")]

namespace DeployD.Hub.App_Start
{
    using System.Reflection;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Mvc;

    public static class NinjectMVC3 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestModule));
            DynamicModuleUtility.RegisterModule(typeof(HttpApplicationInitializationModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IApiHttpChannel>().To<ApiHttpChannel>();
            kernel.Bind<IRepresentationBuilder>().To<XmlRepresentationBuilder>();
            kernel.Bind<IRepresentationBuilder>().To<JsonRepresentationBuilder>();
            kernel.Bind<IAgentRepository>().To<RavenDbAgentRepository>().InSingletonScope();
            kernel.Bind<IAgentManager>().To<AgentManager>().InSingletonScope();
            kernel.Bind<IPackageStore>().To<LocalPackageStore>().InSingletonScope();
            kernel.Bind<IAgentRemoteService>().To<AgentRemoteService>().InSingletonScope();
            kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger(context.Request.Target.Name));

            kernel.Bind<IDocumentStore>()
                .ToMethod(ctx =>
                              {
                                  var documentStore = new EmbeddableDocumentStore()
                                {
                                    DataDirectory =System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Database")
                                };
                                  documentStore.Initialize();

                                  return documentStore;
                              }).InSingletonScope();
        }        
    }
}
