using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;

namespace tabsik12.Ets2TelemetryPlugin
{
    public class SpeedAction : PluginAction
    {
        public override string Name => "ETS2 Speed";
        public override string Description => "Display current truck speed from ETS2 telemetry";
        public override bool CanConfigure => false;

        public override void Trigger(string clientId, ActionButton actionButton)
        {
            // Nic nie musi tu być – wartości są w zmiennych
         }
    }
}