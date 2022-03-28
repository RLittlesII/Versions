using Nuke.Common;
using Nuke.Common.Tooling;

internal partial class Versions
{
    private Target Fastlane => _ => _
        .DependsOn(SetupKeychain)
        .Executes(() =>
        {
            using var process = ProcessTasks.StartProcess("fastlane", "ci --verbose", logInvocation: true, logOutput: true).AssertZeroExitCode();
        });

    Target FastlaneMatch => _ => _
        .DependsOn(ModifyInfoPlist)
        .Executes(() =>
        {
            using var process = ProcessTasks.StartProcess("fastlane", "match development --verbose", logInvocation: true, logOutput: true);
            return process.WaitForExit();
        });
}