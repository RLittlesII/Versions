using System.Collections.Generic;
using JetBrains.Annotations;
using Nuke.Common.CI.AzurePipelines.Configuration;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;

namespace Azp
{

    [PublicAPI]
    public class AzurePipelineUseDotNetStep : AzurePipelinesStep
    {
        // - task: UseDotNet@2
        // displayName: 'Use .NET Core sdk'
        // inputs:
        // packageType: sdk
        // version: 3.1.x
        // installationPath: $(Agent.ToolsDirectory)/dotnet

        public string Version { get; set; }
        public Dictionary<string, string> Imports { get; set; }

        public override void Write(CustomFileWriter writer)
        {
            using (writer.WriteBlock("- task: UseDotNet@2"))
            {
                writer.WriteLine($"displayName: 'Use .NET Core sdk'");
                using (writer.WriteBlock("inputs:"))
                {
                    writer.WriteLine($"packageType: sdk");
                    writer.WriteLine($"version: {Version}");
                    writer.WriteLine($"installationPath: $(Agent.ToolsDirectory)/dotnet");
                }

                if (Imports.Count > 0)
                {
                    using (writer.WriteBlock("env:"))
                    {
                        Imports.ForEach(x => writer.WriteLine($"{x.Key}: {x.Value}"));
                    }
                }
            }
        }
    }
}