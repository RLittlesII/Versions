using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Rocket.Surgery.Nuke.Azp;

partial class Versions
{
    Target Fastlane => _ => _
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .DependsOn(FastlaneMatch);

    Target InstallFastlane => _ => _
        .Executes(() => ProcessTasks.StartShell("gem", "install fastlane"));

    Target FastlaneMatch => _ => _
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .DependsOn(InstallFastlane)
        .DependentFor(ArchiveIpa)
        .Before(ModifyInfoPlist)
        .Executes(() => ProcessTasks.StartShell("fastlane","match --verbose", logInvocation: true, logOutput: true));
}