using System.Collections.Generic;
using System.Linq;
using Azp;
using JetBrains.Annotations;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.CI.AzurePipelines.Configuration;
using Nuke.Common.Execution;

[Azp.AzurePipelines(AzurePipelinesImage.MacOsLatest,
    TriggerBranchesInclude = new[] { "main" },
    InvokedTargets = new[] { nameof(Default) },
    CacheKeyFiles = new string[] {},
    CachePaths = new string[] {},
    AutoGenerate = true)]
partial class Versions
{
}