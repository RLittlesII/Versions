using Nuke.Common;
using Nuke.Common.CI.AzurePipelines;

// [AzurePipelines.AzurePipelines(AzurePipelinesImage.MacOsLatest,
//     TriggerBranchesInclude = new[] { "main" },
//     InvokedTargets = new[] { nameof(Default) },
//     CacheKeyFiles = new string[] {},
//     CachePaths = new string[] {},
//     AppleSigningCertificate = "versions.p12",
//     AppleProvisioningProfile = "",
//     AutoGenerate = true)]
[AzurePipelinesStepsAttribute(InvokeTargets = new[] { nameof(Default) },
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
}

interface IHaveAppleCertificate
{
    public string SigningCertificate { get; }
}

interface IHaveAppleProvisioningProfile
{
    public string ProvisioningProfile { get; }
}