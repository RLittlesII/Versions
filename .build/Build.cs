using System.Diagnostics.CodeAnalysis;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities;
using Rocket.Surgery.Nuke.Azp;
using Rocket.Surgery.Nuke.Xamarin;
using Serilog;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

/// <summary>
/// Defines <see cref="Nuke"/> targets for the build.
/// </summary>
[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
[DotNetVerbosityMapping]
[MSBuildVerbosityMapping]
[NuGetVerbosityMapping]
[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1134:Attributes should not share line", Justification = "This is how I Nuke!")]
[SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:Closing parenthesis should be spaced correctly", Justification = "This is how I Nuke!")]
[SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1003:Symbols should be spaced correctly", Justification = "This is how I Nuke!")]
internal partial class Versions : NukeBuild,
    ICanClean,
    ICanRestoreXamarin,
    ICanBuildXamariniOS,
    ICanPackXamariniOS,
    ICanTestXamarin,
    ICanArchiveiOS,
    IHaveConfiguration<Configuration>
{
    /// <inheritdoc cref="ICanClean.Clean" />
    public Target Clean => _ => _
        .Inherit<ICanClean>(x => x.Clean);

    /// <inheritdoc cref="ICanRestoreXamarin.Restore" />
    public Target Restore => _ => _
        .DependsOn(Clean)
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .Inherit<ICanRestoreXamarin>(x => x.Restore);

    /// <inheritdoc/>
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
                    .SetPackageVersion(GitVersion.NuGetVersionV2)));

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public Target Pack => _ => _;

    /// <inheritdoc cref="ICanTestXamarin.Test" />
    public Target Test => _ => _;

    /// <inheritdoc/>
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
                            .SetProperty("IpaPackageDir", ((IHaveArtifacts)this).ArtifactsDirectory / "ios")
                            .SetConfiguration(Configuration)
                            .SetDefaultLoggers(((IHaveOutputLogs) this).LogsDirectory / "package.log")
                            .SetGitVersionEnvironment(GitVersion)
                            .SetAssemblyVersion(GitVersion.FullSemanticVersion())
                            .SetPackageVersion(GitVersion.FullSemanticVersion())))
        .Produces(((IHaveArtifacts)this).ArtifactsDirectory / "ios");

    /// <summary>
    /// Gets build the Xamarin iOS target.
    /// </summary>
    public Target XamariniOS => _ => _
        .DependsOn(Clean)
        .DependsOn(Restore)
        .DependsOn(ModifyInfoPlist)
        .DependsOn(FastlaneMatch)
        .DependsOn(ArchiveIpa);

    /// <summary>
    /// Gets the default execution path.
    /// </summary>
    public Target Default => _ => _
        .DependsOn(XamariniOS);

    /// <summary>
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    /// </summary>
    /// <returns>The exit code.</returns>
    public static int Main() => Execute<Versions>(x => x.Default);
}