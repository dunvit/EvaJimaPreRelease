using System;
using System.Windows.Forms;
using EvaJimaCore;
using EveJimaCore.BLL.Map;
using EveJimaCore.Logic.MapInformation.Views;

namespace EveJimaCore.Logic.MapInformation
{
    public partial class InformationMapSettingsView : UserControl, IMapInformationControl
    {
        public event Action<string> ChangeMapKey;

        public InformationMapSettingsView()
        {
            InitializeComponent();
        }

        public void ForceRefresh(Map spaceMap)
        {
            txtKey.Text = spaceMap.Key;
            txtOwner.Text = spaceMap.GetOwner();
            txtServerAddress.Text = Global.ApplicationSettings.Server_MapAddress;
        }

        private void cmdUpdateMapSettings_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKey.Text))
            {
                MessageBox.Show(@"Please fill 'Map Key' field.");
                return;
            }

            if (ChangeMapKey != null) ChangeMapKey(txtKey.Text);
        }
    }
}
