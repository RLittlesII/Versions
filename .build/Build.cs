using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Rocket.Surgery.Nuke.ContinuousIntegration;
using Rocket.Surgery.Nuke.GithubActions;
using Rocket.Surgery.Nuke.Xamarin;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Versions : NukeBuild,
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

    [OptionalGitRepository]
    public GitRepository? GitRepository { get; }

    [ComputedGitVersion]
    public GitVersion GitVersion { get; } = null!;
    public Target Clean => _ => _.Inherit<ICanClean>(x => x.Clean);

    public Target Restore => _ => _.Inherit<ICanRestoreXamarin>(x => x.Restore).DependsOn(Clean);

    public Target Build => _ => _.Inherit<ICanBuildXamariniOS>(x => x.BuildiPhone)
        .DependsOn(Restore)
        .DependsOn(Boots)
        .DependsOn(ModifyInfoPlist);

    public Target ModifyInfoPlist => _ => _.Inherit<ICanBuildXamariniOS>(x => x.ModifyInfoPlist);

    public Target Pack => _ => _.Inherit<ICanPackXamariniOS>(x => x.PackiPhone).DependsOn(Build);

    public Target Test => _ => _
        .DependsOn(Build)
        .Executes(() => { });

    public Target Boots => _ => _
        .DependsOn(Clean)
        .DependsOn(Restore)
        .OnlyWhenStatic(() => !IsLocalBuild)
        .Executes(() =>
        {
            DotNetTasks.DotNetToolInstall(cfg => cfg.SetPackageName("Boots").EnableGlobal());
            ProcessTasks.StartProcess("boots", "--preview Mono");
            ProcessTasks.StartProcess("boots", "--preview Xamarin.Android");
            ProcessTasks.StartProcess("boots", "--preview Xamarin.iOS");
        });

    public Target XamariniOS => _ => _
        .DependsOn(Restore)
        .DependsOn(Boots)
        .DependsOn(ModifyInfoPlist)
        .DependsOn(Build)
        .DependsOn(Pack);

    public Target Default => _ => _
        .DependsOn(XamariniOS);

    public Target BuildVersion => _ => _
       .Inherit<IHaveBuildVersion>(x => x.BuildVersion)
       .Before(Default)
       .Before(Clean);
}