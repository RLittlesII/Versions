using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Rocket.Surgery.Nuke.Azp;

internal partial class Versions
{
    Target Fastlane => _ => _
        .DependsOn(FastlaneMatch);

    Target FastlaneMatch => _ => _
        .DependsOn(ModifyInfoPlist)
        .Executes(() =>
        {
            using var process = ProcessTasks.StartProcess("fastlane", "match development --verbose", logInvocation: true, logOutput: true);
            return process.WaitForExit();
        });
}