using System;
using System.DirectoryServices;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Deployd.Core.AgentConfiguration;
using log4net;

namespace Deployd.Core.Installation.Hooks
{
    public class IisMsDeployDeploymentHook : DeploymentHookBase
    {
        protected string MsWebDeployPath = string.Empty;

        private readonly string[] _knownMsWebDeployPaths = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"IIS\Microsoft Web Deploy\msdeploy.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"IIS\Microsoft Web Deploy\msdeploy.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"IIS\Microsoft Web Deploy V2\msdeploy.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"IIS\Microsoft Web Deploy V2\msdeploy.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"IIS\Microsoft Web Deploy V3\msdeploy.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"IIS\Microsoft Web Deploy V3\msdeploy.exe"),
        };

        public IisMsDeployDeploymentHook(IAgentSettings agentSettings, IFileSystem fileSystem) : base(agentSettings, fileSystem)
        {
        }

        protected void LocateMsDeploy(ILog logger)
        {
            if (_knownMsWebDeployPaths.Any(FileSystem.File.Exists))
            {
                MsWebDeployPath = _knownMsWebDeployPaths.Last(FileSystem.File.Exists);
            }
            else
            {
                if (string.IsNullOrEmpty(MsWebDeployPath))
                {
                    logger.Fatal(
                        "Web Deploy could not be located. Ensure that Microsoft Web Deploy has been installed. Locations searched: " +
                        string.Join("\r\n", _knownMsWebDeployPaths));
                }
            }
        }

        public override bool HookValidForPackage(DeploymentContext context)
        {
            DirectoryEntry website;
            LocateMsDeploy(context.GetLoggerFor(this));
            return context.Package.Tags.ToLower().Split(' ', ',', ';').Contains("website")
                && !string.IsNullOrEmpty(MsWebDeployPath)
                && TryFindWebsite("localhost", context.Package.Title, out website);
        }

        public override void Deploy(DeploymentContext context)
        {
            var installationLogger = context.GetLoggerFor(this);
            LocateMsDeploy(installationLogger);
            DeployWebsite(
                "localhost",
                Path.Combine(context.WorkingFolder, "Content\\" + context.Package.Id + ".zip"),
                context.Package.Title,
                installationLogger,
                Ignore.AppOffline().And().LogFiles().And().MaintenanceFile());
        }

        public override void AfterDeploy(DeploymentContext context)
        {
            var installationLogger = context.GetLoggerFor(this);
            RestartApplication(context, installationLogger);

        }

        private void RestartApplication(DeploymentContext context, ILog logger)
        {
            string virtualDirectoryPath = null;
            string[] websitePath = context.Package.Title.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            if (websitePath.Length > 1)
            {
                virtualDirectoryPath = string.Join("/", websitePath.Skip(1).ToArray());
            }
            using (var website = FindVirtualDirectory("localhost", websitePath[0], virtualDirectoryPath))
            {
                if (website == null)
                {
                    logger.WarnFormat("No such IIS website found: '{0}'", context.Package.Id);
                }
                var appPoolId = website.Properties["AppPoolId"].Value;

                using (var applicationPool = new DirectoryEntry("IIS://localhost/W3SVC/AppPools/" + appPoolId))
                {
                    logger.InfoFormat("Stopping AppPool {0}...", appPoolId);
                    applicationPool.Invoke("Stop");
                    logger.InfoFormat("Starting AppPool {0}...", appPoolId);
                    applicationPool.Invoke("Start");
                }
            }
        }

        protected void DeployWebsite(string targetMachineName, string sourcePackagePath, string iisApplicationName, ILog logger, params string[] ignoreRegexPaths)
        {
            var ignore = string.Join(" -skip:objectName=filePath,absolutePath=", ignoreRegexPaths);
            
            if (ignoreRegexPaths.Length > 0)
            {
                ignore = " -skip:objectName=filePath,absolutePath=" + ignore;
            }

            const string msDeployArgsFormat = @"-verb:sync -source:package=""{0}"" -dest:auto,computername=""{1}"" {3} -allowUntrusted -setParam:""IIS Web Application Name""=""{2}"" -verbose";
            var executableArgs = string.Format(msDeployArgsFormat, sourcePackagePath, AgentSettings.MsDeployServiceUrl,
                                               iisApplicationName, ignore);

            RunProcess(MsWebDeployPath, executableArgs, logger);
           
        }

        static DirectoryEntry FindVirtualDirectory(string server, string website, string virtualdir=null)
        {
            DirectoryEntry siteEntry = null;
            DirectoryEntry rootEntry = null;
            try
            {
                siteEntry = FindWebSite(server, website);
                if (siteEntry == null)
                {
                    return null;
                }

                rootEntry = siteEntry.Children.Find("ROOT", "IIsWebVirtualDir");
                if (rootEntry == null)
                {
                    return null;

                }

                if (string.IsNullOrWhiteSpace(virtualdir))
                {
                    return rootEntry;
                }

                return rootEntry.Children.Find(virtualdir, "IIsWebVirtualDir");
            }
            catch (DirectoryNotFoundException ex)
            {
                if (rootEntry != null) rootEntry.Dispose();
                if (siteEntry != null) siteEntry.Dispose();

                throw;
            }
        }

        static DirectoryEntry FindWebSite(string server, string friendlyName)
        {
            string path = String.Format("IIS://{0}/W3SVC", server);

            using (DirectoryEntry w3svc = new DirectoryEntry(path))
            {
                foreach (DirectoryEntry entry in w3svc.Children)
                {
                    if (entry.SchemaClassName == "IIsWebServer" &&
                        entry.Properties["ServerComment"].Value.Equals(friendlyName))
                    {
                        return entry;
                    }
                }
            }
            return null;
        }

        static bool TryFindWebsite(string server, string friendlyName, out DirectoryEntry website)
        {
            try
            {
                website = FindWebSite(server, friendlyName);
                return true;
            } catch
            {
                website = null;
                return false;
            }
        }
    }

    public class Ignore
    {
        public static string[] AppOffline()
        {
            return new[] {@".*app_offline\.htm"};
        }
        public static string[] LogFiles()
        {
            return new[] {@".*\.log"};
        }
        public static string[] MaintenanceFile()
        {
            return new[] {@".*\maintenance\.htm"};
        }
    }


    public static class IgnoreExtensions
    {
        public static string[] AppOffline(this string[] chain)
        {
            return chain.Union(new[] {@".*app_offline\.htm"}).ToArray();
        }

        public static string[] LogFiles(this string[] chain)
        {
            return chain.Union(new[] { @".*\.log" }).ToArray();
        }

        public static string[] MaintenanceFile(this string[] chain)
        {
            return chain.Union(new[] {@".*\.log"}).ToArray();
        }

        public static string[] And(this string[] chain)
        {
            return chain;
        }

    }
}
