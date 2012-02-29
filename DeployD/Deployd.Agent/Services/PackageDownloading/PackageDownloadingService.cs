﻿using System.Linq;
using Deployd.Agent.Services.AgentConfiguration;
using Deployd.Core;
using Deployd.Core.AgentConfiguration;
using Deployd.Core.Caching;
using Deployd.Core.Hosting;
using Deployd.Core.Queries;
using log4net;

namespace Deployd.Agent.Services.PackageDownloading
{
    public class PackageDownloadingService : IWindowsService
    {
        private readonly IAgentConfigurationManager _agentConfigurationManager; 
        protected static readonly ILog Logger = LogManager.GetLogger("PackageDownloadingService");

        public ApplicationContext AppContext { get; set; }

        private readonly IAgentSettings _settings;
        protected readonly IRetrieveAllAvailablePackageManifestsQuery AllPackagesQuery;
        protected readonly INuGetPackageCache AgentCache;

        private readonly TimedSingleExecutionTask _task;

        public PackageDownloadingService(IAgentSettings agentSettings, IRetrieveAllAvailablePackageManifestsQuery allPackagesQuery, INuGetPackageCache agentCache, IAgentConfigurationManager agentConfigurationManager)
        {
            _settings = agentSettings;
            AllPackagesQuery = allPackagesQuery;
            AgentCache = agentCache;
            _agentConfigurationManager = agentConfigurationManager;
            _task = new TimedSingleExecutionTask(agentSettings.PackageSyncIntervalMs, FetchPackages);
        }

        public void Start(string[] args)
        {
            _task.Start(args);
        }

        public void Stop()
        {
            _task.Stop();
        }

        public void FetchPackages()
        {
            var packages = _agentConfigurationManager.GetWatchedPackages(_settings.DeploymentEnvironment);
            foreach (var latestPackageOfType in packages.Select(packageId => AllPackagesQuery.GetLatestPackage(packageId)))
            {
                AgentCache.Add(latestPackageOfType);
            }
        }
    }
}
