using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Rocket.Surgery.Nuke.Azp;

/// <summary>
/// Represents the Azure Pipeline portion of the build.
/// </summary>
[AzurePipelinesSecretStepsAttribute(
    InvokeTargets = new[] { nameof(AzurePipelines) },
    Parameters = new[] { nameof(IHaveConfiguration.Configuration), nameof(Verbosity) },
    Secrets = new[] { nameof(BucketAccessKey), nameof(BucketSecretAccessKey) },
    AutoGenerate = false)]
internal partial class Versions
{
    /// <summary>
    /// Gets azure Pipelines target.
    /// </summary>
    private Target AzurePipelines => _ => _
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .DependsOn(Clean)
        .DependsOn(Restore)
        .DependsOn(ModifyInfoPlist)
        .DependsOn(SetupKeychain)
        .DependsOn(Fastlane)
        .DependsOn(ArchiveIpa)
        .DependsOn(CopyIpa);

    private Target SetupKeychain => _ => _
        .DependsOn(ModifyInfoPlist)
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .Executes(() =>
        {
            var createKeychain = ProcessTasks.StartProcess("security", "create-keychain -p doesntmatteritwillbetemporaryanyway temporary.keychain ", logInvocation: true, logOutput: true).AssertZeroExitCode().WaitForExit();
            var listKeychain = ProcessTasks.StartProcess("security", "list-keychains -s temporary.keychain", logInvocation: true, logOutput: true).AssertZeroExitCode().WaitForExit();
            var unlockKeychain = ProcessTasks.StartProcess("security", "unlock-keychain -p doesntmatteritwillbetemporaryanyway temporary.keychain", logInvocation: true, logOutput: true).AssertZeroExitCode().WaitForExit();
            var findKeychain = ProcessTasks.StartProcess("security", "find-identity -v -p codesigning temporary.keychain", logInvocation: true, logOutput: true).AssertZeroExitCode().WaitForExit();
            return new[] { createKeychain, listKeychain, unlockKeychain, findKeychain };
        });

    private Target CopyIpa => _ => _
        .DependsOn(ArchiveIpa)
        .Executes(() => FileSystemTasks.CopyFileToDirectory(
            ((IHaveArtifacts) this).ArtifactsDirectory / "ios" / "Versions.iOS.ipa",
            "$(build.artifactstagingdirectory)"));
}