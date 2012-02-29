﻿using System.Collections.Specialized;
using Deployd.Core.AgentConfiguration;
using NUnit.Framework;

namespace Deployd.Core.Test.Unit.AgentConfiguration
{
    [TestFixture]
    public class AgentSettingsManagerTests
    {
        private AgentSettingsManager _mgr;

        [SetUp]
        public void SetUp()
        {
            _mgr = new AgentSettingsManager();            
        }

        [Test]
        public void LoadSettings_WhenSuppliedWithNoConfiguration_TakesDefaults()
        {
            var settings = _mgr.LoadSettings();

            Assert.That(settings.ConfigurationSyncIntervalMs, Is.EqualTo(60000));
            Assert.That(settings.DeploymentEnvironment, Is.EqualTo("Production"));
            Assert.That(settings.InstallationDirectory, Is.StringContaining("\\app_root"));
            Assert.That(settings.NuGetRepository, Is.StringContaining("\\DebugPackageSource"));
            Assert.That(settings.PackageSyncIntervalMs, Is.EqualTo(60000));
            Assert.That(settings.UnpackingLocation, Is.StringContaining("\\app_unpack"));
        }

        [Test]
        public void LoadSettings_WhenSuppliedWithConfiguration_TakesConfiguration()
        {
            var dictionary = new NameValueCollection
                                 {
                                     {"ConfigurationSyncIntervalMs","1"},
                                     {"DeploymentEnvironment","2"},
                                     {"InstallationDirectory","3"},
                                     {"NuGetRepository","4"},
                                     {"PackageSyncIntervalMs","5"},
                                     {"UnpackingLocation","6"},
                                 };

            var settings = _mgr.LoadSettings(dictionary);

            Assert.That(settings.ConfigurationSyncIntervalMs, Is.EqualTo(1));
            Assert.That(settings.DeploymentEnvironment, Is.EqualTo("2"));
            Assert.That(settings.InstallationDirectory, Is.StringContaining("3"));
            Assert.That(settings.NuGetRepository, Is.StringContaining("4"));
            Assert.That(settings.PackageSyncIntervalMs, Is.EqualTo(5));
            Assert.That(settings.UnpackingLocation, Is.StringContaining("6"));
        }
    }
}
