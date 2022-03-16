using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Rocket.Surgery.Nuke.Azp;
using Rocket.Surgery.Nuke.Xamarin;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
partial class Versions : NukeBuild,
    ICanClean,
    ICanRestoreXamarin,
    ICanBuildXamariniOS,
    ICanPackXamariniOS,
    ICanTestXamarin,
    IHaveConfiguration<Configuration>
{
    /// <summary>
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    /// </summary>
    public static int Main() => Execute<Versions>(x => x.Default);

    public AbsolutePath InfoPlist { get; } = RootDirectory / "src" / "Versions.iOS" / "info.plist";

    public string BaseBundleIdentifier { get; } = "com.companyname.Versions";

    [Parameter("Configuration to build")]
    public Configuration Configuration { get; } = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    public TargetPlatform iOSTargetPlatform { get; } = TargetPlatform.iPhoneSimulator;

    [OptionalGitRepository] public GitRepository? GitRepository { get; }

    [ComputedGitVersion] public GitVersion GitVersion { get; } = null!;

    public Target Clean => _ => _
        .DependsOn(BuildVersion)
        .Inherit<ICanClean>(x => x.Clean);

    public Target Restore => _ => _.DependsOn(Clean).Inherit<ICanRestoreXamarin>(x => x.Restore);

    public Target Build => _ => _.DependsOn(Restore)
        .DependsOn(ModifyInfoPlist)
        .Executes(() => MSBuild(
            settings =>
                settings
                    .SetSolutionFile(((IHaveSolution) this).Solution)
                    .EnableRestore()
                    .SetProperty("Platform", iOSTargetPlatform)
                    .SetConfiguration(Configuration)
                    .SetDefaultLoggers(((IHaveOutputLogs) this).LogsDirectory / "build.log")
                    .SetGitVersionEnvironment(GitVersion)
                    .SetAssemblyVersion(GitVersion?.AssemblySemVer)
                    .SetPackageVersion(GitVersion?.NuGetVersionV2)));

    public Target ModifyInfoPlist => _ => _.Inherit<ICanBuildXamariniOS>(x => x.ModifyInfoPlist);

    public Target Pack => _ => _;

    public Target Test => _ => _;

    /// <summary>
    ///     packages a binary for distribution.
    /// </summary>
    public Target ArchiveIpa => _ => _
        .DependsOn(ModifyInfoPlist)
        .OnlyWhenStatic(() => EnvironmentInfo.Platform == PlatformFamily.OSX)
        .Executes(() => MSBuild(settings =>
            settings.SetSolutionFile(((IHaveSolution) this).Solution)
                .EnableRestore()
                .SetConfiguration(Configuration.Release)
                .SetProperty("Platform", iOSTargetPlatform)
                .SetProperty("BuildIpa", "true")
                .SetProperty("ArchiveOnBuild", "true")
                .SetConfiguration(Configuration)
                .SetDefaultLoggers(((IHaveOutputLogs) this).LogsDirectory / "package.log")
                .SetGitVersionEnvironment(GitVersion)
                .SetAssemblyVersion(GitVersion?.AssemblySemVer)
                .SetPackageVersion(GitVersion?.NuGetVersionV2)));

    public Target XamariniOS => _ => _
        .DependsOn(Clean)
        .DependsOn(Restore)
        .DependsOn(ModifyInfoPlist)
        .DependsOn(ArchiveIpa);

    public Target Default => _ => _
        .DependsOn(XamariniOS);

    public Target BuildVersion => _ => _
        .Before(Clean)
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .Inherit<IHaveBuildVersion>(x => x.BuildVersion);
}