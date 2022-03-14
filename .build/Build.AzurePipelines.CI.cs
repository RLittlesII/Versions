using System.Collections.Generic;
using Azp;
using JetBrains.Annotations;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.CI.AzurePipelines.Configuration;
using Nuke.Common.Execution;

[AzurePipelines(AzurePipelinesImage.MacOsLatest,
    TriggerBranchesInclude = new[] { "main" },
    InvokedTargets = new[] { nameof(Default) },
    AutoGenerate = true)]
partial class Versions
{
    // public class AzurePipelinesAttribute : Nuke.Common.CI.AzurePipelines.AzurePipelinesAttribute
    // {
    //     public AzurePipelinesAttribute(AzurePipelinesImage image, params AzurePipelinesImage[] images)
    //         : base(image, images)
    //     {
    //     }
    //
    //     public AzurePipelinesAttribute([CanBeNull] string suffix, AzurePipelinesImage image,
    //         params AzurePipelinesImage[] images)
    //         : base(suffix, image, images)
    //     {
    //     }
    //
    //     protected override IEnumerable<AzurePipelinesStep> GetSteps(ExecutableTarget executableTarget,
    //         IReadOnlyCollection<ExecutableTarget> relevantTargets, AzurePipelinesImage image)
    //     {
    //         if (executableTarget.Name == "InstallDotNet")
    //         {
    //             yield return new AzurePipelineUseDotNetStep() { Version = "6.0.x" };
    //         }
    //         else
    //         {
    //             foreach (var azurePipelinesStep in base.GetSteps(executableTarget, relevantTargets, image))
    //             {
    //                 yield return azurePipelinesStep;
    //             }
    //         }
    //     }
    // }
}