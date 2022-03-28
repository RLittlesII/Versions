using Nuke.Common;
using Nuke.Common.Tooling;
using Rocket.Surgery.Nuke.Azp;

[AzurePipelinesSecretStepsAttribute(
    InvokeTargets = new[] { nameof(AzurePipelines) },
    Parameters = new[] { nameof(IHaveConfiguration.Configuration), nameof(Verbosity) },
    Secrets = new[] { nameof(BucketAccessKey), nameof(BucketSecretAccessKey) },
    AutoGenerate = false)]
partial class Versions
{
    Target AzurePipelines => _ => _
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .DependsOn(Clean)
        .DependsOn(Restore)
        .DependsOn(ModifyInfoPlist)
        .DependsOn(SetupKeychain)
        .DependsOn(Fastlane)
        .DependsOn(ArchiveIpa);

    Target SetupKeychain => _ => _
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .Executes(() =>
        {
            var createKeychain = ProcessTasks.StartProcess("security", "create-keychain -p temporary.keychain doesntmatteritwillbetemporaryanyway", logInvocation: true, logOutput: true).WaitForExit();
            var unlockKeychain = ProcessTasks.StartProcess("security", "unlock-keychain -p temporary.keychain doesntmatteritwillbetemporaryanyway", logInvocation: true, logOutput: true).WaitForExit();
            var findKeychain = ProcessTasks.StartProcess("security", "find-identity -v -p codesigning temporary.keychain", logInvocation: true, logOutput: true).WaitForExit();
            return new[] { createKeychain, unlockKeychain, findKeychain};
        });
}