using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EvaJimaCore;
using EveJimaCore.Domain.MapToolbar.Contracts;
using EveJimaUniverse;

namespace EveJimaCore.Domain.MapSettings.View
{
    public partial class ElementMapSettings : Form , IMapInformationView, IMapSettingsView
    {
        public ElementMapSettings()
        {
            InitializeComponent();
        }


        public void ActivateContainer()
        {
            
        }

        public void ShowInformationPanel(string panelName)
        {
        }

        public void ChangeSolarSystem(SolarSystem solarSystem)
        {
        }

        public event Action<string> DeleteSelectedSystem;

        public event Action<string> DeathNotice;

        public event Action<string> CentreScreenSelectedSystem;

        public event Action<string> CentreScreenLocationSystem;

        public event Action<string, List<CosmicSignature>> UpdateSignatures;

        public event Action<string> ChangeMapKey;

        public void Reload()
        {
            txtKey.Text = Global.Pilots.Selected.SpaceMap.Key;
        }

        private void ElementMapSettings_Load(object sender, EventArgs e)
        {
            
        }

        private void cmdUpdateSignatures_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKey.Text))
            {
                MessageBox.Show("Please fill 'Map Key' field.");
                return;
            }

            if (ChangeMapKey != null) ChangeMapKey(txtKey.Text);
        }

        public void ForceRefresh()
        {
            txtKey.Text = Global.Pilots.Selected.SpaceMap.Key;
            txtOwner.Text = Global.Pilots.Selected.SpaceMap.Owner;
            txtServerAddress.Text = Global.ApplicationSettings.Server_MapAddress;
        }

        private void ElementMapSettings_Activated(object sender, EventArgs e)
        {

        }
    }
}
