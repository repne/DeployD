﻿using System.Collections.Generic;
using System.Threading;
using Deployd.Agent.Services.AgentConfiguration;
using Deployd.Agent.Services.PackageDownloading;
using Deployd.Core.AgentConfiguration;
using Deployd.Core.PackageCaching;
using Deployd.Core.PackageTransport;
using Moq;
using NUnit.Framework;
using NuGet;

namespace Deployd.Agent.Test.Unit.Services.PackageDownloading
{
    [TestFixture]
    public class PackageDownloadingServiceTests 
    {
        private Mock<IRetrievePackageQuery> _packageRepoMock;
        private Mock<ILocalPackageCache> _packageCacheMock;
        private PackageDownloadingService _pds;
        private Mock<IAgentConfigurationManager> _agentConfigManagerMock;
        private AgentSettings _agentSettings;
        private const string PACKAGE_ID = "packageId";

        [SetUp]
        public void SetUp()
        {
            _agentSettings = new AgentSettings {DeploymentEnvironment = "Staging", PackageSyncIntervalMs = 1};
            _agentConfigManagerMock = new Mock<IAgentConfigurationManager>();
            _agentConfigManagerMock.Setup(x => x.GetWatchedPackages(_agentSettings.DeploymentEnvironment)).Returns(new List<string> { PACKAGE_ID });
            _packageRepoMock = new Mock<IRetrievePackageQuery>();
            _packageCacheMock = new Mock<ILocalPackageCache>();
            _pds = new PackageDownloadingService(_agentSettings, _packageRepoMock.Object, _packageCacheMock.Object, _agentConfigManagerMock.Object);
        }

        [Test]
        public void LocallyCachePackages_CallsPackageRepoForPackageList()
        {
            IPackage items = null;
            _packageRepoMock.Setup(x => x.GetLatestPackage(PACKAGE_ID)).Returns(items);

            _pds.FetchPackages();

            _packageRepoMock.Verify(x => x.GetLatestPackage(PACKAGE_ID));
        }

        [Test]
        public void Start_WhenInvoked_UsesTimerToCallDownloadConfiguration()
        {
            IPackage items = null;
            _packageRepoMock.Setup(x => x.GetLatestPackage(PACKAGE_ID)).Returns(items);

            _pds.Start(new string[0]);
            Thread.Sleep(100);

            _packageRepoMock.VerifyAll();
        }

        [Test]
        public void Stop_WhenInvoked_StopsTask()
        {
            _agentSettings.ConfigurationSyncIntervalMs = 10000;

            _pds.Start(new string[0]);
            Assert.That(_pds.TimedTask.IsRunning, Is.True);

            _pds.Stop();
            Assert.That(_pds.TimedTask.IsRunning, Is.False);
        }

    }
}
