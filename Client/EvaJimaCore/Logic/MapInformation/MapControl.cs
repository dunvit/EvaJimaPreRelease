﻿using System;
using System.Collections.Generic;
using System.Drawing;
using EvaJimaCore;
using EveJimaCore.BLL.Map;
using EveJimaCore.WhlControls;
using EveJimaUniverse;
using log4net;

namespace EveJimaCore.Logic.MapInformation
{
    public partial class MapControl : BaseContainer, IAMapInformationView
    {
        readonly ILog _errorsLog = LogManager.GetLogger("Errors");
        readonly ILog _commandsLog = LogManager.GetLogger("CommandsMap");

        public MapControl()
        {
            InitializeComponent();

            containerInformation.CentreScreenLocationSystem += Event_CentreScreenLocationSystem;
            containerInformation.CentreScreenSelectedSystem += Event_CentreScreenSelectedSystem;
            containerInformation.DeleteSelectedSystem += Event_DeleteSelectedSystem;
            containerInformation.UpdateSignatures += Event_UpdateSignatures;
            containerInformation.DeathNotice += Event_DeathNotice;
            containerToolbar.OnSelectTab += Event_SelectTab;
            containerInformation.ChangeMapKey += Event_ChangeMapKey;
            containerMap.SelectSolarSystem += Event_SelectSolarSystem;
            containerMap.RelocateSolarSystem += Event_RelocateSolarSystem;
            Global.Presenter.OnLocationChange += Event_LocationChanged;
            Global.Presenter.OnChangeActivePilot += Event_ActivePilotChanged;
        }

        private void Event_ChangeMapKey(string key)
        {
            var screen = new ScreenUpdateToServer { ActionType  = "ChangeMapKey", MapKey = key};
            screen.RefreshMapControl += Event_RefreshMap;
            screen.ShowDialog();

            _commandsLog.InfoFormat("[ScreenUpdateToServer.Event_Activate] " + "After change mapKey");

            
        }

        private void Event_RefreshMap(string obj)
        {
            containerMap.ForceRefresh(Global.Pilots.Selected.SpaceMap);
            containerInformation.ForceRefresh(Global.Pilots.Selected.SpaceMap);
        }

        private void Event_UpdateSignatures(string arg1, List<CosmicSignature> signatures)
        {
            Global.Pilots.Selected.SpaceMap.ApiPublishSignatures(
                Global.Pilots.Selected.SpaceMap.Key, 
                Global.Pilots.Selected.SpaceMap.LocationSolarSystemName,
                Global.Pilots.Selected.Name,signatures);
        }

        private void Event_DeathNotice(string selectedSolarSystemName)
        {
            Global.MapApiFunctions.PublishDeadLetter(Global.Pilots.Selected.SpaceMap.Key, Global.Pilots.Selected.Name, Global.Pilots.Selected.LocationPreviousSystemName, selectedSolarSystemName);
            Global.Pilots.Selected.SpaceMap.Update();
            containerMap.ForceRefresh(Global.Pilots.Selected.SpaceMap);
        }

        private void Event_DeleteSelectedSystem(string selectedSolarSystemName)
        {
            Global.MapApiFunctions.DeleteSolarSystem(Global.Pilots.Selected.SpaceMap.Key, Global.Pilots.Selected.SpaceMap.SelectedSolarSystemName, Global.Pilots.Selected.Name);

            Global.Pilots.Selected.SpaceMap.RemoveSystem(Global.Pilots.Selected.SpaceMap.SelectedSolarSystemName);

            containerMap.ForceRefresh(Global.Pilots.Selected.SpaceMap);
            containerInformation.ForceRefresh(Global.Pilots.Selected.SpaceMap);
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
                _errorsLog.ErrorFormat("[MapModel.RelocateSolarSystem] Point = {1} SolarSystemName = {2} Critical error {0}", ex, solarSystemNewLocationInMap, Global.Pilots.Selected.SpaceMap.SelectedSolarSystemName);
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
