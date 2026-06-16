using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;


namespace tabsik12.Ets2TelemetryPlugin
{
    public class GearAction : PluginAction
    {
        public override string Name => "ETS2 Gear";
        public override string Description => "Display current gear from ETS2 telemetry";
        public override bool CanConfigure => false;

        public override void Trigger(string clientId, ActionButton actionButton)
        {
        
        }
    }
}