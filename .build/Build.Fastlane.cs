using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Rocket.Surgery.Nuke.Azp;

partial class Versions
{
    Target Fastlane => _ => _
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .DependsOn(FastlaneMatch);

    Target FastlaneMatch => _ => _
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .DependentFor(ArchiveIpa)
        .Before(ModifyInfoPlist)
        .Executes(() =>
        {
            ProcessTasks.StartProcess("fastlane","match", logInvocation: true, logOutput: true);
        });
}