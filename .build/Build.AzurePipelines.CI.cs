using Nuke.Common;
using Nuke.Common.Tooling;
using Rocket.Surgery.Nuke.Azp;

[AzurePipelinesSecretStepsAttribute(
    InvokeTargets = new[] { nameof(AzurePipelines) },
    Parameters = new[] { nameof(IHaveConfiguration.Configuration), nameof(Verbosity) },
        nameof(AzurePipelines),
    },
    Parameters = new[]
    {
        nameof(IHaveConfiguration.Configuration),
        nameof(Verbosity),
        nameof(BucketRegion),
    },
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
            using var createKeychain = ProcessTasks.StartProcess("security", "create-keychain -p temporary.keychain doesntmatteritwillbetemporaryanyway", logInvocation: true, logOutput: true);
            using var unlockKeychain = ProcessTasks.StartProcess("security", "unlock-keychain -p temporary.keychain doesntmatteritwillbetemporaryanyway").AssertZeroExitCode();
            using var findKeychain = ProcessTasks.StartProcess("security", "find-identity -v -p codesigning temporary.keychain").AssertZeroExitCode();
            return createKeychain.WaitForExit();
        });
}