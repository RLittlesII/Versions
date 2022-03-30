using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Rocket.Surgery.Nuke.Azp;
using Serilog;
using static Nuke.Common.EnvironmentInfo;

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
    private Target PrintAzurePipelinesEnvironment => _ => _.Before(Clean).Executes(() =>
    {
        Log.Information("AGENT_ID: {AgentID}", GetVariable<string>("AGENT_ID"));
        Log.Information("AGENT_NAME: {AgentName}", GetVariable<string>("AGENT_NAME"));
        Log.Information("AGENT_VERSION: {AgentVersion}", GetVariable<string>(" "));
        Log.Information("AGENT_JOBNAME: {AgentJobName}", GetVariable<string>("AGENT_JOBNAME"));
        Log.Information("AGENT_JOBSTATUS: {AgentJobStatus}", GetVariable<string>("AGENT_JOBSTATUS"));
        Log.Information(
            "AGENT_MACHINE_NAME: {AgentMachineName}", GetVariable<string>("AGENT_MACHINE_NAME")
        );
        Log.Information("\n");

        Log.Information("BUILD_BUILDID: {BuildBuildId}", GetVariable<string>("BUILD_BUILDID"));
        Log.Information(
            "BUILD_BUILDNUMBER: {BuildBuildnumber}", GetVariable<string>("BUILD_BUILDNUMBER")
        );
        Log.Information(
            "BUILD_DEFINITIONNAME: {BuildDefinitionName}", GetVariable<string>("BUILD_DEFINITIONNAME")
        );
        Log.Information(
            "BUILD_DEFINITIONVERSION: {BuildDefinitionVersion}",
            GetVariable<string>("BUILD_DEFINITIONVERSION")
        );
        Log.Information("BUILD_QUEUEDBY: {BuildQueuedBy}", GetVariable<string>("BUILD_QUEUEDBY"));
        Log.Information("\n");

        Log.Information(
            "BUILD_SOURCEBRANCHNAME: {BuildSourceBranchName}",
            GetVariable<string>("BUILD_SOURCEBRANCHNAME")
        );
        Log.Information(
            "BUILD_SOURCEVERSION: {BuildSourceVersion}", GetVariable<string>("BUILD_SOURCEVERSION")
        );
        Log.Information(
            "BUILD_REPOSITORY_NAME: {BuildRepositoryName}",
            GetVariable<string>("BUILD_REPOSITORY_NAME")
        );
        Log.Information(
            "BUILD_REPOSITORY_PROVIDER: {BuildRepositoryProvider}",
            GetVariable<string>("BUILD_REPOSITORY_PROVIDER")
        );
    });

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
        .DependsOn(ArchiveIpa);

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
}