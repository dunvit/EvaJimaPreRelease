using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using EveJimaCore.BLL.Map;
using EveJimaCore.Logic.MapInformation;
using EveJimaCore.Logic.MapInformation.Views;

namespace TestPlatform.Logic.Views
{
    public partial class MapInformationControl : UserControl
    {
        public string SelectedTab { get; set; }

        Dictionary<string, Panel> _informationControls;

        SolarSystemInformationControl _solarSystemInformationControl;
        InformationSignaturesView _informationSignaturesView;
        InformationMapSettingsView _informationMapSettingsView;

        public event Action<string> CentreScreenSelectedSystem;

        public event Action<string> CentreScreenLocationSystem;

        public event Action<string> DeleteSelectedSystem;

        public event Action<string> DeathNotice;

        public MapInformationControl()
        {
            InitializeComponent();

            _informationControls = new Dictionary<string, Panel>();
            _solarSystemInformationControl = new SolarSystemInformationControl { Visible = true, Dock = DockStyle.Fill };
            _informationSignaturesView = new InformationSignaturesView { Visible = true, Dock = DockStyle.Fill };
            _informationMapSettingsView = new InformationMapSettingsView { Visible = true, Dock = DockStyle.Fill };

            _solarSystemInformationControl.CentreScreenLocationSystem += Event_CentreScreenLocationSystem;
            _solarSystemInformationControl.CentreScreenSelectedSystem += Event_CentreScreenSelectedSystem;
            _solarSystemInformationControl.DeleteSelectedSystem += Event_DeleteSelectedSystem;
            _solarSystemInformationControl.DeathNotice += Event_DeathNotice;

            var systemInfoPanel = new Panel { Location = new Point(0, 0), Tag = "SolarSystem", Visible = false, Dock = DockStyle.Fill };
            var signaturesPanel = new Panel { Location = new Point(0, 0), Tag = "MapSignatures", Visible = false, Dock = DockStyle.Fill };
            var mapSettingsPanel = new Panel { Location = new Point(0, 0), Tag = "MapSettings", Visible = false, Dock = DockStyle.Fill };

            systemInfoPanel.Controls.Add(_solarSystemInformationControl);
            signaturesPanel.Controls.Add(_informationSignaturesView);
            mapSettingsPanel.Controls.Add(_informationMapSettingsView);

            _informationControls.Add("SolarSystem", systemInfoPanel);
            _informationControls.Add("MapSignatures", signaturesPanel);
            _informationControls.Add("MapSettings", mapSettingsPanel);

            Controls.Add(systemInfoPanel);
            Controls.Add(signaturesPanel);
            Controls.Add(mapSettingsPanel);

            ActivatePanel("SolarSystem");
        }

        private void Event_DeathNotice(string obj)
        {
            DeathNotice(obj);
        }

        private void Event_DeleteSelectedSystem(string obj)
        {
            DeleteSelectedSystem(obj);
        }

        private void Event_CentreScreenSelectedSystem(string obj)
        {
            CentreScreenLocationSystem(obj);
        }

        private void Event_CentreScreenLocationSystem(string obj)
        {
            CentreScreenSelectedSystem(obj);
        }

        public void ChangeLocation(Map spaceMap)
        {
            foreach (var control in _informationControls.Values)
            {
                var controlPart = control.Controls[0] as IMapInformationControl;

                if(controlPart != null) controlPart.ForceRefresh(spaceMap);
            }
        }

        public void ActivatePanel(string panelName)
        {
            foreach (var control in _informationControls.Values)
            {
                if(control.Visible) control.Visible = false;
            }

            _informationControls[panelName].Visible = true;
            _informationControls[panelName].Show();

            SelectedTab = panelName;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MapInformationControl
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(14)))), ((int)(((byte)(14)))));
            this.Name = "MapInformationControl";
            this.Size = new System.Drawing.Size(269, 304);
            this.Load += new System.EventHandler(this.InformationView_Load);
            this.ResumeLayout(false);

        }


        private void InformationView_Load(object sender, System.EventArgs e)
        {
           
        }

        private void MapInformationControl_Load(object sender, System.EventArgs e)
        {

        }
    }
}
