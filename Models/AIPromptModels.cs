using System;

namespace AuroraDesignSuite.Models
{
    public class CustomPromptItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string PromptText { get; set; } = string.Empty;
        public bool IsPreset { get; set; }
    }
}
