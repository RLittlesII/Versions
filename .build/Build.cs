using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Rocket.Surgery.Nuke.Azp;
using Rocket.Surgery.Nuke.Xamarin;
using Serilog;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
[DotNetVerbosityMapping]
[MSBuildVerbosityMapping]
[NuGetVerbosityMapping]
partial class Versions : NukeBuild,
    ICanClean,
    ICanRestoreXamarin,
    ICanBuildXamariniOS,
    ICanPackXamariniOS,
    ICanTestXamarin,
    ICanArchiveiOS,
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

    public string BaseBundleIdentifier { get; } = "com.companyname.versions";

    public TargetPlatform iOSTargetPlatform { get; } = TargetPlatform.iPhoneSimulator;

    [Parameter("Configuration to build")]
    public Configuration Configuration { get; } = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("")] public string SigningCertificate { get; } = "";

    [Parameter("")] public string ProvisioningProfile { get; } = "";


    [OptionalGitRepository] public GitRepository? GitRepository { get; }

    [ComputedGitVersion] public GitVersion GitVersion { get; } = null!;

    [Parameter] public bool EnableRestore { get; } = AzurePipelinesTasks.IsRunningOnAzurePipelines.Compile().Invoke();

    public Target BuildVersion => _ => _
        .Before(Clean)
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .Inherit<IHaveBuildVersion>(x => x.BuildVersion);

    public Target Clean => _ => _
        .DependsOn(BuildVersion)
        .Inherit<ICanClean>(x => x.Clean);

    public Target Restore => _ => _
        .DependsOn(Clean)
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .Inherit<ICanRestoreXamarin>(x => x.Restore);

    public Target Build => _ => _
        .DependsOn(Restore)
        .DependsOn(ModifyInfoPlist)
        .Executes(() =>
            MSBuild(settings =>
                settings
                    .EnableRestore()
                    .SetSolutionFile(((IHaveSolution) this).Solution)
                    .SetProperty("Platform", iOSTargetPlatform)
                    .SetConfiguration(Configuration)
                    .SetDefaultLoggers(((IHaveOutputLogs) this).LogsDirectory / "build.log")
                    .SetGitVersionEnvironment(GitVersion)
                    .SetAssemblyVersion(GitVersion?.AssemblySemVer)
                    .SetPackageVersion(GitVersion?.NuGetVersionV2)));

    public Target ModifyInfoPlist => _ => _.Inherit<ICanBuildXamariniOS>(x => x.ModifyInfoPlist);

    public Target Pack => _ => _;

    public Target Test => _ => _;

    public Target InstallCertificate => _ => _
        .Executes(() =>
        {
            ProcessTasks.StartProcess("openssl", "--preview Mono");
        });

    /// <summary>
    ///     packages a binary for distribution.
    /// </summary>
    public Target ArchiveIpa => _ => _
        .DependsOn(ModifyInfoPlist)
        .OnlyWhenStatic(() => EnvironmentInfo.Platform == PlatformFamily.OSX)
        .Inherit<ICanArchiveiOS>(x => x.ArchiveIpa);

    public Target XamariniOS => _ => _
        .DependsOn(Clean)
        .DependsOn(Restore)
        .DependsOn(ModifyInfoPlist)
        .DependsOn(ArchiveIpa);

    public Target Default => _ => _
        .DependsOn(XamariniOS);
}