using System;
using System.Windows.Forms;

namespace tabsik12.Ets2TelemetryPlugin
{
    public class SettingsForm : Form
    {
        private readonly Main _plugin;
        private CheckBox _chkUseMph;
        private Button _btnOk;
        private Button _btnCancel;

        public SettingsForm(Main plugin)
        {
            _plugin = plugin;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "ETS2 Telemetry Settings";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 300;
            this.Height = 150;

            _chkUseMph = new CheckBox
            {
                Text = "Use mph instead of km/h",
                Left = 15,
                Top = 15,
                Width = 250,
                Checked = _plugin.UseMph
            };

            _btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Left = 50,
                Top = 60,
                Width = 80
            };
            _btnOk.Click += BtnOk_Click;

            _btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Left = 150,
                Top = 60,
                Width = 80
            };

            this.Controls.Add(_chkUseMph);
            this.Controls.Add(_btnOk);
            this.Controls.Add(_btnCancel);
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            _plugin.SaveConfiguration(_chkUseMph.Checked);
            this.Close();
        }
    }
}