using System;
using System.Drawing;
using EvaJimaCore;
using EveJimaCore.BLL.Map;
using EveJimaCore.WhlControls;
using log4net;

namespace EveJimaCore.Logic.MapInformation
{
    public partial class MapControl : BaseContainer, IAMapInformationView
    {
        readonly ILog _commandsLog = LogManager.GetLogger("Errors");

        public MapControl()
        {
            InitializeComponent();

            containerInformation.CentreScreenLocationSystem += Event_CentreScreenLocationSystem;
            containerInformation.CentreScreenSelectedSystem += Event_CentreScreenSelectedSystem;
            containerInformation.DeleteSelectedSystem += Event_DeleteSelectedSystem;
            containerInformation.DeathNotice += Event_DeathNotice;
            containerToolbar.OnSelectTab += Event_SelectTab;
            containerMap.SelectSolarSystem += Event_SelectSolarSystem;
            containerMap.RelocateSolarSystem += Event_RelocateSolarSystem;
            Global.Presenter.OnLocationChange += Event_LocationChanged;
            Global.Presenter.OnChangeActivePilot += Event_ActivePilotChanged;
        }

        private void Event_DeathNotice(string selectedSolarSystemName)
        {
            Global.MapApiFunctions.PublishDeadLetter(Global.Pilots.Selected.SpaceMap.Key, Global.Pilots.Selected.Name, Global.Pilots.Selected.LocationPreviousSystemName, selectedSolarSystemName);
            Global.Pilots.Selected.SpaceMap.Update();
            containerMap.ForceRefresh(Global.Pilots.Selected.SpaceMap);
        }

        private void Event_DeleteSelectedSystem(string selectedSolarSystemName)
        {
            var screenUpdateToServer = new ScreenUpdateToServer();

            screenUpdateToServer.ShowDialog(this);

            //Global.MapApiFunctions.DeleteSolarSystem(Global.Pilots.Selected.SpaceMap.Key, selectedSolarSystemName, Global.Pilots.Selected.Name);

            //Global.Pilots.Selected.SpaceMap.RemoveSystem(selectedSolarSystemName);

            containerMap.ForceRefresh(Global.Pilots.Selected.SpaceMap);
        }

        private void Event_CentreScreenSelectedSystem(string obj)
        {
            containerMap.CentreScreenBySelectedSystem();
        }

        private void Event_CentreScreenLocationSystem(string obj)
        {
            containerMap.CentreScreenByLocationSystem();
        }

        private void Event_RelocateSolarSystem(Point solarSystemNewLocationInMap, string arg2)
        {
            try
            {
                Global.MapApiFunctions.UpdateSolarSystemCoordinates(Global.Pilots.Selected.SpaceMap.Key, Global.Pilots.Selected.SpaceMap.SelectedSolarSystemName, Global.Pilots.Selected.SpaceMap.ActivePilot, solarSystemNewLocationInMap.X, solarSystemNewLocationInMap.Y);
            }
            catch (Exception ex)
            {
                _commandsLog.ErrorFormat("[MapModel.RelocateSolarSystem] Point = {1} SolarSystemName = {2} Critical error {0}", ex, solarSystemNewLocationInMap, Global.Pilots.Selected.SpaceMap.SelectedSolarSystemName);
            }
        }

        private void Event_SelectSolarSystem(string obj)
        {
            containerInformation.ChangeLocation(Global.Pilots.Selected.SpaceMap);
        }

        private void Event_ActivePilotChanged(Map spaceMap)
        {
            containerMap.ForceRefresh(spaceMap);
        }

        private void Event_LocationChanged(Map spaceMap)
        {
            containerInformation.ChangeLocation(spaceMap);
        }

        private void Event_SelectTab(string containerName)
        {
            containerInformation.ActivatePanel(containerName);
        }

        private void MapControl_Load(object sender, EventArgs e)
        {

        }

        public void Close()
        {
        }

        public void Event_Map_OnResize()
        {
        }


    }
}
