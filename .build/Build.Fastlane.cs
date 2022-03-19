using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Rocket.Surgery.Nuke.Azp;

partial class Versions
{
    Target Fastlane => _ => _
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .DependsOn(Homebrew)
        .DependsOn(InstallFastlane);

    Target InstallFastlane => _ => _
        .DependsOn(InstallHomebrew)
        .Executes(() =>
        {
            ProcessTasks.StartProcess("brew install", "fastlane");
        });

    Target FastlaneMatch => _ => _
        .DependsOn(InstallFastlane)
        .DependentFor(ArchiveIpa)
        .After(ModifyInfoPlist)
        .Before(ArchiveIpa)
        .Executes(() =>
        {
            ProcessTasks.StartProcess("fastlane","--help");
        });
}