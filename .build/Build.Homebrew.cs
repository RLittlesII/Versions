using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Rocket.Surgery.Nuke.Azp;

partial class Versions
{
    Target Homebrew => _ => _
        .OnlyWhenStatic(AzurePipelinesTasks.IsRunningOnAzurePipelines)
        .DependsOn(InstallHomebrew);

    Target InstallHomebrew => _ => _
        .Executes(() =>
        {
            FileSystemTasks.EnsureExistingDirectory("homebrew");
            ProcessTasks.StartShell(
                "mkdir homebrew && curl -L https://github.com/Homebrew/brew/tarball/master | tar xz --strip 1 -C homebrew", logInvocation: true, logOutput: true);
            // ProcessTasks.StartShell(
            //     "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)", logInvocation: true, logOutput: true);
        });
}