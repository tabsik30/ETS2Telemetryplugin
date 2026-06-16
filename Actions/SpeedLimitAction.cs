using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;


namespace tabsik12.Ets2TelemetryPlugin
{
    public class SpeedLimitAction : PluginAction
    {
        public override string Name => "ETS2 Speed Limit";
        public override string Description => "Display current speed limit from ETS2 telemetry";
        public override bool CanConfigure => false;

        public override void Trigger(string clientId, ActionButton actionButton)
        {
        
        }
    }
}