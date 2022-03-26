using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Rocket.Surgery.Nuke.Azp;

partial class Versions
{
    Target Fastlane => _ => _
        .DependsOn(FastlaneMatch);

    Target InstallFastlane => _ => _
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .Executes(() => ProcessTasks.StartProcess("sudo", "gem install fastlane"));

    Target FastlaneMatch => _ => _
        .After(ModifyInfoPlist)
        .DependsOn(InstallFastlane)
        .DependentFor(ArchiveIpa)
        .Executes(() =>
        {
            var env = IsLocalBuild ? "development" : "adhoc";
            return ProcessTasks.StartProcess("fastlane", $"match adhoc --verbose", logInvocation: true, logOutput: true);
        });
}