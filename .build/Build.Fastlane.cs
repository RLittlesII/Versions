using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Rocket.Surgery.Nuke.Azp;

internal partial class Versions
{
    Target Fastlane => _ => _
        .DependsOn(FastlaneMatch);

    Target InstallFastlane => _ => _
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .Executes(() =>
        {
            using var process = ProcessTasks.StartProcess("brew", "install fastlane");
            return process.WaitForExit();
        });

    Target FastlaneMatch => _ => _
        .DependsOn(ModifyInfoPlist)
        .Executes(() =>
        {
            using var process = ProcessTasks.StartProcess("fastlane", "match development --verbose", logInvocation: true, logOutput: true);
            return process.WaitForExit();
        });
}