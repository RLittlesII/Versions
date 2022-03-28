using Nuke.Common;
using Nuke.Common.Tooling;

/// <summary>
/// Represents the Fastlane portion of the build.
/// </summary>
internal partial class Versions
{
    private Target Fastlane => _ => _
        .DependsOn(SetupKeychain)
        .Executes(() => ProcessTasks.StartProcess("fastlane", "ci --verbose", logInvocation: true, logOutput: true).AssertZeroExitCode().WaitForExit());

    private Target FastlaneMatch => _ => _
        .DependsOn(ModifyInfoPlist)
        .Executes(() => ProcessTasks.StartProcess("fastlane", "match development --verbose", logInvocation: true, logOutput: true).AssertZeroExitCode().WaitForExit());
}