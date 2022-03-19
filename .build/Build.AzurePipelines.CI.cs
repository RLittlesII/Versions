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
[AzurePipelinesStepsAttribute(InvokeTargets = new[] { nameof(AzurePipelines) },
    Parameters = new[]
    {
        // nameof(IHaveAppleCertificate.SigningCertificate),
        // nameof(IHaveAppleProvisioningProfile.ProvisioningProfile),
        nameof(IHaveConfiguration.Configuration),
        nameof(Verbosity)
    },
    AutoGenerate = false)]
partial class Versions
{
    Target AzurePipelines => _ => _
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .DependsOn(Homebrew)
        .DependsOn(Fastlane)
        .DependsOn(Default);
}

interface IHaveAppleCertificate
{
    public string SigningCertificate { get; }
}

interface IHaveAppleProvisioningProfile
{
    public string ProvisioningProfile { get; }
}