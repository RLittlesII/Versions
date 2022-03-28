using Nuke.Common;
using Nuke.Common.CI.AzurePipelines;
using Rocket.Surgery.Nuke.Azp;

// [AzurePipelines.AzurePipelines(AzurePipelinesImage.MacOsLatest,
//     TriggerBranchesInclude = new[] { "main" },
//     InvokedTargets = new[] { nameof(Default) },
//     CacheKeyFiles = new string[] {},
//     CachePaths = new string[] {},
//     AppleSigningCertificate = "versions.p12",
//     AppleProvisioningProfile = "",
//     AutoGenerate = true)]
[AzurePipelinesSecretStepsAttribute(
    InvokeTargets = new[]
    {
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
        .DependsOn(XamariniOS);
}