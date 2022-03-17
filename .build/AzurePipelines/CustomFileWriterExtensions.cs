using System;
using Nuke.Common.CI.AzurePipelines.Configuration;
using Nuke.Common.Utilities;

namespace AzurePipelines
{
    public static class CustomFileWriterExtensions
    {
        public static IDisposable Task(this CustomFileWriter writer, string taskName) => writer.WriteBlock($"- task: {taskName}");
        public static void DisplayName(this CustomFileWriter writer, string displayName) => writer.WriteLine($"displayName: '{displayName}'");
        public static IDisposable Inputs(this CustomFileWriter writer) => writer.WriteBlock("inputs:");
        public static void Input(this CustomFileWriter writer, string name, string value) => writer.WriteLine($"{name}: '{value}'");
    }
}