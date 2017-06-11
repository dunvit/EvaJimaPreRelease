using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using EvaJimaCore;
using EveJimaCore.Domain.MapInformationSignatures.View;
using EveJimaCore.Domain.MapInformationSolarSystem.View;
using EveJimaCore.Domain.MapSettings.View;
using EveJimaCore.Domain.MapToolbar.Contracts;
using EveJimaUniverse;
using log4net;

namespace EveJimaCore.Domain.MapInformation.View
{
    public partial class ElementMapInformation : Form, IMapInformationView
    {
        private static readonly ILog Log = LogManager.GetLogger("All");

        public event Action<string> DeleteSelectedSystem;

        public event Action<string> DeathNotice;

        public event Action<string> CentreScreenSelectedSystem;

        public event Action<string> CentreScreenLocationSystem;

        public event Action<string, List<CosmicSignature>> UpdateSignatures;

        public event Action<string> ChangeMapKey;

        Dictionary<string, IMapInformationView> elements = new Dictionary<string, IMapInformationView>();

        ElementMapInformationSolarSystem elementMapInformationSolarSystem = new ElementMapInformationSolarSystem();

        ElementMapInformationSignatures elementMapInformationSignatures = new ElementMapInformationSignatures();

        ElementMapSettings elementMapSettings = new ElementMapSettings();

        string SelectedPanelName { get; set; }

        public ElementMapInformation()
        {
            InitializeComponent();

            elementMapInformationSolarSystem.TopLevel = false;

            elementMapInformationSolarSystem.Location = new Point(0, 0);
            elementMapInformationSolarSystem.Size = new Size(800, 900);
            elementMapInformationSolarSystem.FormBorderStyle = FormBorderStyle.None;
            elementMapInformationSolarSystem.Visible = true;
            elementMapInformationSolarSystem.Dock = DockStyle.Fill;

            elementMapInformationSolarSystem.DeleteSelectedSystem += DeleteSystem;
            elementMapInformationSolarSystem.CentreScreenSelectedSystem += CentreScreenSystemSelected;
            elementMapInformationSolarSystem.CentreScreenLocationSystem += CentreScreenSystemLocation;
            
            elementMapInformationSolarSystem.DeathNotice += Event_DeathNotice;

            elements.Add("SolarSystem", elementMapInformationSolarSystem);



            elementMapInformationSignatures.TopLevel = false;

            elementMapInformationSignatures.Location = new Point(0, 0);
            elementMapInformationSignatures.Size = new Size(800, 900);
            elementMapInformationSignatures.FormBorderStyle = FormBorderStyle.None;
            elementMapInformationSignatures.Visible = true;
            elementMapInformationSignatures.Dock = DockStyle.Fill;

            elementMapInformationSignatures.UpdateSignatures += ViewUpdateSignatures;

            elements.Add("MapSignatures", elementMapInformationSignatures);

            #region  MapSettings element

            elementMapSettings.TopLevel = false;

            elementMapSettings.Location = new Point(0, 0);
            elementMapSettings.Size = new Size(800, 900);
            elementMapSettings.FormBorderStyle = FormBorderStyle.None;
            elementMapSettings.Visible = true;
            elementMapSettings.Dock = DockStyle.Fill;

            elementMapSettings.ChangeMapKey += ViewChangeMapKey;
            

            //elementMapSettings.UpdateSignatures += ViewUpdateSignatures;

            elements.Add("MapSettings", elementMapSettings);

            #endregion

            // Default panel
            SelectedPanelName = "SolarSystem";
        }

        private void Event_DeathNotice(string locationSolarSystem)
        {
            if(DeathNotice != null) DeathNotice(locationSolarSystem);
        }

        private void ViewChangeMapKey(string mapKey)
        { 
            if (ChangeMapKey != null) ChangeMapKey(mapKey);
        }

        private void ViewUpdateSignatures(string solarSystemName, List<CosmicSignature> signatures)
        {
            if (UpdateSignatures != null) UpdateSignatures(solarSystemName, signatures);
        }

        private void CentreScreenSystemLocation(string obj)
        {
            if(CentreScreenLocationSystem != null) CentreScreenLocationSystem(obj);
        }

        private void CentreScreenSystemSelected(string obj)
        {
            if(CentreScreenSelectedSystem != null) CentreScreenSelectedSystem(obj);
        }

        

        public void ActivateContainer()
        {
            elementMapSettings.ActivateContainer();
        }

        private void DeleteSystem(string solarSystemName)
        {
            if(DeleteSelectedSystem != null) DeleteSelectedSystem(solarSystemName);
            Refresh();
        }

        public void ShowInformationPanel(string panelName)
        {
            SelectedPanelName = panelName;

            Controls.Clear();

            if(elements.ContainsKey(panelName))
            {
                Controls.Add((Control)elements[panelName]);
                elements[panelName].ForceRefresh();
                elements[panelName].Show();
            }
        }

        public void ChangeSolarSystem(SolarSystem solarSystem)
        {
            foreach(var mapInformationView in elements)
            {
                elements[mapInformationView.Key].ChangeSolarSystem(solarSystem);
            }
        }


        public void ForceRefresh()
        {
            elementMapInformationSolarSystem.ForceRefresh();

            elementMapSettings.ForceRefresh();

            elementMapInformationSignatures.ForceRefresh();
        }
    }
}
