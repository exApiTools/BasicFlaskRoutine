using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;


namespace TreeRoutine
{
    public class BaseTreeSettings : ISettings
    {
        [Menu("Enable")]
        public ToggleNode Enable { get; set; } = new(false);

        [Menu("Debug")]
        public ToggleNode Debug { get; set; } = new(false);

        public ToggleNode EnableMissingConfigEntryNotifications { get; set; } = new(true);
    }
}
