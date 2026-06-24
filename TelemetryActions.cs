using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Plugins;
using System;

namespace tabsik12.Ets2TelemetryPlugin
{
    public static class TelemetryPluginInstance
    {
        public static Main MainInstance { get; set; }
    }

    public class StartTelemetryAction : PluginAction
    {
        public override string Name => "Start ETS2/ATS telemetry";
        public override string Description => "Rozpoczyna odpytywanie ETS2/ATS Telemetry Servera.";
        public override bool CanConfigure => false;

        public override void Trigger(string clientId, ActionButton actionButton)
        {
            try
            {
                TelemetryPluginInstance.MainInstance?.StartTelemetry();
            }
            catch (Exception)
            {
                // celowo pusto – nie wywalamy całego pluginu jeśli coś pójdzie nie tak
            }
        }

        public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
        {
            return null;
        }
    }

    public class StopTelemetryAction : PluginAction
    {
        public override string Name => "Stop ETS2/ATS telemetry";
        public override string Description => "Zatrzymuje odpytywanie ETS2/ATS Telemetry Servera.";
        public override bool CanConfigure => false;

        public override void Trigger(string clientId, ActionButton actionButton)
        {
            try
            {
                TelemetryPluginInstance.MainInstance?.StopTelemetry();
            }
            catch (Exception)
            {
                // celowo pusto
            }
        }

        public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
        {
            return null;
        }
    }
}
