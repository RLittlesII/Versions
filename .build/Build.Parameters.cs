using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tools.GitVersion;
using Rocket.Surgery.Nuke.Azp;
using Rocket.Surgery.Nuke.Xamarin;

/// <summary>
/// Represents the build parameters and properties.
/// </summary>
internal partial class Versions
{
    [Parameter] string BucketRegion { get; set; }

    [Parameter] [Secret] readonly string BucketAccessKey;
    [Parameter] [Secret] readonly string BucketSecretAccessKey;

    /// <inheritdoc/>
    public AbsolutePath InfoPlist { get; } = RootDirectory / "src" / "Versions.iOS" / "info.plist";

    /// <inheritdoc/>
    public string BaseBundleIdentifier { get; } = "com.company.versions";

    /// <inheritdoc/>
    public TargetPlatform iOSTargetPlatform { get; } = TargetPlatform.iPhone;

    [Parameter("Configuration to build")] public Configuration Configuration { get; } = Configuration.Release;

    [Parameter] public string IdentifierSuffix { get; } = string.Empty;

    /// <inheritdoc/>
    [Parameter] public bool EnableRestore { get; } = AzurePipelinesTasks.IsRunningOnAzurePipelines.Compile().Invoke();

    [OptionalGitRepository] public GitRepository? GitRepository { get; }

    /// <inheritdoc/>
    [ComputedGitVersion] public GitVersion GitVersion { get; } = null!;
}