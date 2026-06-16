using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;

namespace tabsik12.Ets2TelemetryPlugin
{
    public class FuelAction : PluginAction
    {
        public override string Name => "ETS2 Fuel";
        public override string Description => "Display fuel percentage from ETS2 telemetry";
        public override bool CanConfigure => false;

        public override void Trigger(string clientId, ActionButton actionButton)
        {
        }
    }
}