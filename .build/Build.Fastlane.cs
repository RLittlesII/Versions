using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Rocket.Surgery.Nuke.Azp;

partial class Versions
{
    Target Fastlane => _ => _
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .DependsOn(InstallFastlane)
        .DependsOn(FastlaneMatch);

    Target InstallFastlane => _ => _
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .Executes(() =>
        {
            ProcessTasks.StartProcess("brew", "install fastlane", logInvocation: true, logOutput: true);
        });

    Target FastlaneMatch => _ => _
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .DependsOn(InstallFastlane)
        .DependentFor(ArchiveIpa)
        .After(ModifyInfoPlist)
        .Before(ArchiveIpa)
        .Executes(() =>
        {
            ProcessTasks.StartProcess("fastlane","match", logInvocation: true, logOutput: true);
        });
}