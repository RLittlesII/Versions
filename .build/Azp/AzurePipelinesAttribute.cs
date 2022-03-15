using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.CI.AzurePipelines.Configuration;
using Nuke.Common.Execution;

namespace Azp
{
    public class AzurePipelinesAttribute : Nuke.Common.CI.AzurePipelines.AzurePipelinesAttribute
    {
        public AzurePipelinesAttribute(AzurePipelinesImage image, params AzurePipelinesImage[] images)
            : base(image, images)
        {
        }

        public AzurePipelinesAttribute([CanBeNull] string suffix, AzurePipelinesImage image,
            params AzurePipelinesImage[] images)
            : base(suffix, image, images)
        {
        }

        protected override IEnumerable<AzurePipelinesStep> GetSteps(ExecutableTarget executableTarget,
            IReadOnlyCollection<ExecutableTarget> relevantTargets, AzurePipelinesImage image)
        {
            var azurePipelinesSteps = base.GetSteps(executableTarget, relevantTargets, image).ToList();

            azurePipelinesSteps.Insert(0, new AzurePipelineUseDotNetStep { Version = "6.0.x" });
            return azurePipelinesSteps;
        }
    }
}