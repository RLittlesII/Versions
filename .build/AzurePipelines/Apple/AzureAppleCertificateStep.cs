using Nuke.Common.CI.AzurePipelines.Configuration;
using Nuke.Common.Utilities;

namespace AzurePipelines.Apple
{
    public class AppleInstallStep : AzurePipelinesStep
    {
        public override void Write(CustomFileWriter writer)
        {
        }
    }

    public class InstallAppleCertificateStep : AppleInstallStep
    {
        public InstallAppleCertificateStep(string appleSigningCertificate, string appleCertificatePassword)
        {
            AppleSigningCertificate = appleSigningCertificate;
            AppleCertificatePassword = appleCertificatePassword;
        }

        public override void Write(CustomFileWriter writer)
        {
            using (writer.Task("InstallAppleCertificate@2"))
            {
                writer.DisplayName("Install Apple Certificate");
                using (writer.Inputs())
                {
                    writer.Input("certSecureFile",AppleSigningCertificate);
                    writer.Input("certPwd",AppleCertificatePassword);
                    writer.Input("setUpPartitionIdACLForPrivateKey","false");
                }
            }
        }

        readonly string AppleSigningCertificate;
        readonly string AppleCertificatePassword;
    }

    public class InstallProvisioningProfileStep : AppleInstallStep
    {
        public InstallProvisioningProfileStep(string appleProvisioningProfile)
        {
            AppleProvisioningProfile = appleProvisioningProfile;
        }

        public string AppleProvisioningProfile { get; }

        public override void Write(CustomFileWriter writer)
        {
            using (writer.Task("InstallAppleProvisioningProfile@1"))
            {
                writer.DisplayName("Install Provisioning Profile");
                using (writer.Inputs())
                {
                    writer.Input("provProfileSecureFile", AppleProvisioningProfile);
                }
            }
        }
    }
}