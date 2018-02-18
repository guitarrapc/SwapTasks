using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;

namespace MSBuildTasks
{
    public class SwapFile : Task
    {
        [Required]
        public string FileName { get; set; }
        [Required]
        public string Extension { get; set; }
        [Required]
        public string SourceDir { get; set; }
        [Required]
        public string DestinationDir { get; set; }

        public string Trigger { get; set; }
        public string Fallback { get; set; }
        public string Configuration { get; set; }

        private string Message { get; set; }
        private string Source { get; set; }
        private string Destination => Path.Combine(DestinationDir, $"{FileName}.{Extension}");

        public override bool Execute()
        {
            Message = $"Missing Parameter '{nameof(Trigger)}'. Skipping {FileName}.{Extension} swap task.";

            // Evaluate
            if (!string.IsNullOrWhiteSpace(Trigger) && File.Exists(Path.Combine(SourceDir, $"{FileName}.{Trigger}.{Extension}")))
            {
                // For "dotnet build|publish -c Debug /p:Trigger=Xxxx" where xxx.Trigger.config exists
                // app.{Trigger}.config -> app.config
                Message = $"Trigger : Detected /p:'{nameof(Trigger)}'. Copy {FileName}.{Trigger}.{Extension} to {FileName}.{Extension}.";
                Source = Path.Combine(SourceDir, $"{FileName}.{Trigger}.{Extension}");
            }
            else if (!string.IsNullOrWhiteSpace(Fallback) && !string.IsNullOrWhiteSpace(Trigger) && !File.Exists(Path.Combine(SourceDir, $"{FileName}.{Trigger}.{Extension}")))
            {
                // app.{Fallback}.config -> app.config
                // For "dotnet build|publish -c Debug /p:Trigger=Xxxx" where xxx.Trigger.config NOT exists
                Message = $"Fallback : Detected /p:'{nameof(Trigger)}' but {FileName}.{Trigger}.{Extension} missing. Copy {FileName}.{Fallback}.{Extension} to {FileName}.{Extension}.";
                Source = Path.Combine(SourceDir, $"{FileName}.{Fallback}.{Extension}");
            }
            else if (!string.IsNullOrWhiteSpace(Configuration) && File.Exists(Path.Combine(SourceDir, $"{FileName}.{Configuration}.{Extension}")))
            {
                // app.{Configuration}.config -> app.config
                // For "dotnet build|publish without /p:Trigger=Xxxx"
                Message = $"Configuration : Missing /p:'{nameof(Trigger)}'. Copy {FileName}.{Configuration}.{Extension} to {FileName}.{Extension}.";
                Source = Path.Combine(SourceDir, $"{FileName}.{Configuration}.{Extension}");
            }

            Log.LogMessage(MessageImportance.High, $"@{nameof(SwapFile)}@ [{FileName}.{Extension}] {Message}");
            Log.LogMessage(MessageImportance.High, $"* {nameof(Source)}({File.Exists(Source)}) : {Source}");
            Log.LogMessage(MessageImportance.High, $"* {nameof(Destination)}  : {Destination}");

            // Execute
            if (!string.IsNullOrWhiteSpace(Source) && !string.IsNullOrWhiteSpace(Destination))
            {
                File.Copy(Source, Destination, true);
            }
            return true;
        }
    }
}
