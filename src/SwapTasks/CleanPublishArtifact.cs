using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;

namespace MSBuildTasks
{
    public class CleanPublishArtifact : Task
    {
        [Required]
        public string MSBuildThisFileDirectory { get; set; }
        public string PublishDir { get; set; } = "";

        private static readonly string[] invalids = new[] { "/", "\\", "." };

        public override bool Execute()
        {
            // Evaluate
            Log.LogMessage(MessageImportance.High, $"@{nameof(CleanPublishArtifact)}@ Clean up publish path before new publish execute.");
            if (string.IsNullOrWhiteSpace(PublishDir))
            {
                Log.LogMessage(MessageImportance.High, $"Skipped. {nameof(PublishDir)} empty.");
                return true;
            }
            foreach (var invalid in invalids)
            {
                if (PublishDir.StartsWith(invalid, StringComparison.OrdinalIgnoreCase))
                {
                    Log.LogMessage(MessageImportance.High, $"Skipped. {nameof(PublishDir)} starts with '{invalid}'");
                    return true;
                }
            }

            var path = Path.Combine(MSBuildThisFileDirectory, PublishDir);

            // Execute
            Log.LogMessage(MessageImportance.High, $"* {nameof(path)}({Directory.Exists(path)}) : {path}");
            if (MSBuildThisFileDirectory.Equals(path, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            return true;
        }
    }
}
