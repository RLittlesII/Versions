using Nuke.Common.Tools.GitVersion;

public static class GitVersionEx
{
    /// <summary>
    /// Gets the full semantic version from <see cref="GitVersion"/>.
    /// </summary>
    /// <param name="gitVersion">The git version.</param>
    /// <returns>The converted semantic version.</returns>
    public static string FullSemanticVersion(this GitVersion gitVersion) =>
        gitVersion.FullSemVer.Replace('+', '.');
}