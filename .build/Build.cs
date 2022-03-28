using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities;
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

    public string BaseBundleIdentifier { get; } = "com.company.versions";

    public TargetPlatform iOSTargetPlatform { get; } = TargetPlatform.iPhone;

    [Parameter("Configuration to build")] public Configuration Configuration { get; } = Configuration.Release;

    [Parameter] public string IdentifierSuffix { get; } = string.Empty;

    [Parameter] public bool EnableRestore { get; } = AzurePipelinesTasks.IsRunningOnAzurePipelines.Compile().Invoke();

    [OptionalGitRepository] public GitRepository? GitRepository { get; }

    [ComputedGitVersion] public GitVersion GitVersion { get; } = null!;

    [Parameter] string BucketRegion { get; set; }

    [Parameter] [Secret] readonly string BucketAccessKey;
    [Parameter] [Secret] readonly string BucketSecretAccessKey;

    public Target BuildVersion => _ => _
        .Before(Clean)
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .Executes(
            () =>
            {
                Log.Information(
                    "Building version {FullSemVer} of {SolutionName} ({@Configuration}) using version {NukeVersion} of Nuke",
                    GitVersion.FullSemanticVersion(),
                    ((IHaveSolution) this).Solution.Name,
                    Configuration,
                    typeof(NukeBuild).Assembly.GetVersionText()
                );
            });

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
        .DependsOn(FastlaneMatch)
        .Executes(() =>
            MSBuild(settings =>
                settings
                    .EnableRestore()
                    .SetSolutionFile(((IHaveSolution) this).Solution)
                    .SetProperty("Platform", iOSTargetPlatform)
                    .SetConfiguration(Configuration)
                    .SetDefaultLoggers(((IHaveOutputLogs) this).LogsDirectory / "build.log")
                    .SetGitVersionEnvironment(GitVersion)
                    .SetAssemblyVersion(GitVersion.FullSemanticVersion())
                    .SetPackageVersion(GitVersion?.NuGetVersionV2)));

    public Target ModifyInfoPlist => _ => _
        .DependsOn(Restore)
        .Executes(
            () =>
            {
                Serilog.Log.Verbose("Info.plist Path: {InfoPlist}", InfoPlist);
                var plist = Plist.Deserialize(InfoPlist);

                Log.Verbose("PList {@Plist}", plist);

                plist["CFBundleIdentifier"] = $"{BaseBundleIdentifier}.{IdentifierSuffix.ToLower()}".TrimEnd('.');
                Serilog.Log.Information("CFBundleIdentifier: {CFBundleIdentifier}", plist["CFBundleIdentifier"]);

                plist["CFBundleShortVersionString"] = $"{GitVersion?.Major}.{GitVersion?.Minor}.{GitVersion?.Patch}";
                Serilog.Log.Information(
                    "CFBundleShortVersionString: {CFBundleShortVersionString}",
                    plist["CFBundleShortVersionString"]);

                plist["CFBundleVersion"] = $"{GitVersion?.FullSemanticVersion()}";
                Serilog.Log.Information("CFBundleVersion: {CFBundleVersion}", plist["CFBundleVersion"]);

                Log.Verbose("PList {@Plist}", plist);
                Plist.Serialize(InfoPlist, plist);
            });

    public Target Pack => _ => _;

    public Target Test => _ => _;

    /// <summary>
    ///     packages a binary for distribution.
    /// </summary>
    public Target ArchiveIpa => _ => _
        .DependsOn(Restore)
        .DependsOn(ModifyInfoPlist)
        .OnlyWhenStatic(() => EnvironmentInfo.Platform == PlatformFamily.OSX)
        .Executes(
            () =>
                MSBuild(
                    settings =>
                        settings.SetSolutionFile(((IHaveSolution) this).Solution)
                            .SetRestore(EnableRestore)
                            .SetProperty("Platform", iOSTargetPlatform)
                            .SetProperty("BuildIpa", "true")
                            .SetProperty("ArchiveOnBuild", "true")
                            .SetProperty("IpaPackageDir", ((IHaveArtifacts) this).ArtifactsDirectory / "ios")
                            .SetConfiguration(Configuration)
                            .SetDefaultLoggers(((IHaveOutputLogs) this).LogsDirectory / "package.log")
                            .SetGitVersionEnvironment(GitVersion)
                            .SetAssemblyVersion(GitVersion.FullSemanticVersion())
                            .SetPackageVersion(GitVersion.FullSemanticVersion())));

    public Target XamariniOS => _ => _
        .DependsOn(Clean)
        .DependsOn(Restore)
        .DependsOn(ModifyInfoPlist)
        .DependsOn(FastlaneMatch)
        .DependsOn(ArchiveIpa);

    public Target Default => _ => _
        .DependsOn(XamariniOS);
}