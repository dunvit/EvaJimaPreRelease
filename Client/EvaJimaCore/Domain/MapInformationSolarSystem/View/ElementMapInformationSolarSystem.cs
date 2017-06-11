using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Windows.Forms;
using EvaJimaCore;
using EveJimaCore.Domain.MapToolbar.Contracts;
using EveJimaUniverse;
using log4net;

namespace EveJimaCore.Domain.MapInformationSolarSystem.View
{
    public partial class ElementMapInformationSolarSystem : Form, IMapInformationSolarSystemView, IView, IMapInformationView
    {
        private static readonly ILog Log = LogManager.GetLogger("All");

        public event Action<string> DeleteSelectedSystem;

        public event Action<string> DeathNotice;

        public event Action<string> CentreScreenSelectedSystem;

        public event Action<string> CentreScreenLocationSystem;

        public event Action<string, List<CosmicSignature>> UpdateSignatures;

        public event Action<string> ChangeMapKey;

        SolarSystem SolarSystem { get; set; }

        public ElementMapInformationSolarSystem()
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
            SolarSystem = solarSystem;

            try
            {
                txtSolarSystemName.Text = solarSystem.Name;
                txtSolarSystemClass.Text = solarSystem.Information.Class;
                if (solarSystem.Information.Effect != null)
                {
                    txtSolarSystemEffect.Text = solarSystem.Information.Effect.Trim() == "" ? "None" : solarSystem.Information.Effect.Trim();
                }
                else
                {
                    txtSolarSystemEffect.Text = "";
                }

                if (solarSystem.Information.Region != null)
                {
                    txtSolarSystemRegion.Text = solarSystem.Information.Region.Replace(" Unknown (", "").Replace(")", "");
                }
                else
                {
                    txtSolarSystemRegion.Text = "";
                }


                txtSolarSystemStaticI.Text = "";
                txtSolarSystemStaticII.Text = "";

                txtSolarSystemStaticI.Visible = false;
                txtSolarSystemStaticII.Visible = false;

                label1.Visible = false;

                txtSolarSystemName.ForeColor = Tools.GetColorBySolarSystem(solarSystem.Information.Security.ToString());

                if (string.IsNullOrEmpty(solarSystem.Information.Static) == false)
                {
                    var wormholeI = Global.Space.WormholeTypes[solarSystem.Information.Static.Trim()];

                    txtSolarSystemStaticI.Text = wormholeI.Name + " " + wormholeI.LeadsTo;
                    txtSolarSystemStaticI.Visible = true;
                    txtSolarSystemStaticI.ForeColor = Tools.GetColorBySolarSystem(wormholeI.LeadsTo);

                    //toolTip1.SetToolTip(txtSolarSystemStaticI, "Max Stable Mass=" + wormholeI.TotalMass + "\r\nMax Jump  Mass=" + wormholeI.SingleMass + "\r\nMax Life time =" + wormholeI.Lifetime);
                }

                if (string.IsNullOrEmpty(solarSystem.Information.Static2) == false)
                {
                    label1.Visible = true;
                    var wormholeII = Global.Space.WormholeTypes[solarSystem.Information.Static2.Trim()];

                    txtSolarSystemStaticII.Text = wormholeII.Name + " " + wormholeII.LeadsTo;
                    txtSolarSystemStaticII.Visible = true;
                    txtSolarSystemStaticII.ForeColor = Tools.GetColorBySolarSystem(wormholeII.LeadsTo);

                    //toolTip2.SetToolTip(txtSolarSystemStaticII, "Max Stable Mass=" + wormholeII.TotalMass + "\r\nMax Jump  Mass=" + wormholeII.SingleMass + "\r\nMax Life time =" + wormholeII.Lifetime);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("[ElementMapInformationSolarSystem.ChangeSolarSystem] Critical error. Exception {0}", ex);
            }
        }


        private void ElementMapInformationSolarSystem_Load(object sender, EventArgs e)
        {
            
        }

        private void ejButton4_Click(object sender, EventArgs e)
        {
            if (DeleteSelectedSystem != null) DeleteSelectedSystem(SolarSystem.Name);
        }

        private void ejButton3_Click(object sender, EventArgs e)
        {
            if (CentreScreenLocationSystem != null) CentreScreenLocationSystem(SolarSystem.Name);
        }

        private void cmdMapSignatures_Click(object sender, EventArgs e)
        {
            if (CentreScreenSelectedSystem != null) CentreScreenSelectedSystem(SolarSystem.Name);
        }

        public void ForceRefresh()
        {
        }

        private void cmdDeathNotice_Click(object sender, EventArgs e)
        {
            if(DeathNotice != null) DeathNotice(Global.Pilots.Selected.SpaceMap.LocationSolarSystemName);
        }

        private void cmdZkillboard_Click(object sender, EventArgs e)
        {
            Global.InternalBrowser.OnBrowserNavigate(("https://zkillboard.com/system/" + Global.Space.BasicSolarSystems[Global.Pilots.Selected.SpaceMap.SelectedSolarSystemName.ToUpper()] + "/"));
        }
    }
}
