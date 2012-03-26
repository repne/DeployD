﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Deployd.Agent.WebUi.Models;
using Deployd.Core.Caching;
using Deployd.Core.Installation;

namespace Deployd.Agent.WebUi.Converters
{
    public static class RunningTasksToPackageListViewModelConverter
    {
        public static PackageListViewModel Convert(INuGetPackageCache cache, RunningInstallationTaskList runningTasks, ICurrentInstalledCache installCache)
        {
            var model = new PackageListViewModel();

            foreach(var packageId in cache.AvailablePackages)
            {
                var package = new LocalPackageInformation
                                  {
                                      PackageId = packageId,
                                      LatestAvailableVersion = cache.GetLatestVersion(packageId).ToString()
                                  };

                var installedPackage = installCache.GetCurrentInstalledVersion(packageId);
                package.InstalledVersion = installedPackage == null ? "" : installedPackage.ToString();

                package.CurrentTask = runningTasks.Count > 0 ? runningTasks
                           .Where(t => t.PackageId == packageId)
                           .Select(t =>
                                       {
                                           var lastOrDefault = t.ProgressReports.LastOrDefault();
                                           return lastOrDefault != null ? new InstallTaskViewModel
                                                                               {
                                                                                   Messages = t.ProgressReports.Select(pr => pr.Message).ToArray(),
                                                                                   Status = Enum.GetName(typeof(TaskStatus), t.Task.Status),
                                                                                   PackageId = t.PackageId,
                                                                                   Version = t.Version,
                                                                                   LastMessage = t.ProgressReports.Count > 0 ? lastOrDefault.Message : ""
                                                                               } : null;
                                       }).FirstOrDefault()
                           : null;
                
                model.Packages.Add(package);
            }

            model.CurrentTasks = runningTasks
                .Select(t =>
                            {
                                var progressReport = t.ProgressReports.LastOrDefault();
                                return progressReport != null
                                           ? new InstallTaskViewModel
                                                 {
                                                     Messages = t.ProgressReports.Select(pr => pr.Message).ToArray(),
                                                     Status = Enum.GetName(typeof (TaskStatus), t.Task.Status),
                                                     PackageId = t.PackageId,
                                                     Version = t.Version,
                                                     LastMessage =
                                                         t.ProgressReports.Count > 0 ? progressReport.Message : ""
                                                 }
                                           : null;
                            }).ToList();


            model.AvailableVersions =
                cache.AllCachedPackages().Select(p => p.Version.ToString()).Distinct().OrderByDescending(s => s);

            return model;
        }
    }
}