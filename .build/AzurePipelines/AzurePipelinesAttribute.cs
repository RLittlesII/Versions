using System;
using System.Collections.Generic;
using System.Linq;
using AzurePipelines.Apple;
using JetBrains.Annotations;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.CI.AzurePipelines.Configuration;
using Nuke.Common.Execution;

namespace AzurePipelines
{
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
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


        public string AppleSigningCertificate { get; set; }

        public string AppleSigningPassword { get; set; }

        public string AppleProvisioningProfile { get; set; }

        protected override IEnumerable<AzurePipelinesStep> GetSteps(ExecutableTarget executableTarget,
            IReadOnlyCollection<ExecutableTarget> relevantTargets, AzurePipelinesImage image)
        {
            var azurePipelinesSteps = base.GetSteps(executableTarget, relevantTargets, image).ToList();

            azurePipelinesSteps.Insert(0, new AzurePipelineUseDotNetStep { Version = "6.0.x" });

            if (executableTarget.Name == nameof(Versions.ArchiveIpa))
            {
                azurePipelinesSteps.Insert(1, new InstallAppleCertificateStep(AppleSigningCertificate, AppleSigningPassword));
                azurePipelinesSteps.Insert(2, new InstallProvisioningProfileStep(AppleProvisioningProfile));
            }

            return azurePipelinesSteps;
        }
    }
}