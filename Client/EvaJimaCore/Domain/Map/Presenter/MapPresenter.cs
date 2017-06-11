using System.Collections.Generic;
using System.Drawing;
using EvaJimaCore;
using EveJimaCore.Domain.Map.Contracts;
using EveJimaCore.Domain.MapToolbar.Contracts;
using EveJimaCore.Domain.MapToolbar.Presenter;
using EveJimaUniverse;

namespace EveJimaCore.Domain.Map.Presenter
{
    public class MapPresenter : IPresenter
    {
        private readonly IMapModel _model;
        private readonly IMapView _view;

        private IMapToolbarModel _modelToolbar;
        private readonly IMapToolbarView _viewToolbar;

        public  MapInformationPresenter MapInformationPresenter { get; set; }

        public MapPresenter(IMapModel model, IMapView view, IMapToolbarModel modelToolbar, IMapToolbarView viewToolbar)
        {
            _model = model;
            _view = view;

            _modelToolbar = modelToolbar;
            _viewToolbar = viewToolbar;

            if ( _model.SpaceMap.GetSystem(_model.SpaceMap.SelectedSolarSystemName) != null && _model.SpaceMap.GetSystem(_model.SpaceMap.SelectedSolarSystemName).Information == null)
            {
                _model.SpaceMap.GetSystem(_model.SpaceMap.SelectedSolarSystemName).Information = Global.Space.GetSolarSystem(_model.SpaceMap.SelectedSolarSystemName);
            }
        }

        private void WireUpViewEvents()
        {
            _view.SendMessage += MapViewSendMessage;
            _view.SelectSolarSystem += MapViewSelectSolarSystem;
            _view.RelocateSolarSystem += MapViewRelocateSolarSystem;

            _viewToolbar.SelectTab += MapToolbarViewSelectPanel;

            MapInformationPresenter.DeleteSelectedSystem += MapViewDeleteSelectedSystem;
            MapInformationPresenter.CentreScreenSelectedSystem += MapViewCentreScreenSelectedSystem;
            MapInformationPresenter.CentreScreenLocationSystem += MapViewCentreScreenLocationSystem;
            MapInformationPresenter.UpdateSignatures += MapViewUpdateSignatures;
            MapInformationPresenter.UpdateMap += OnUpdateMap;
        }

        private void OnUpdateMap()
        {
            _view.DrawSpaceMap(_model.SpaceMap.Systems, _model.SpaceMap.SelectedSolarSystemName);

            MapInformationPresenter.UpdateView();
        }

        private void MapViewUpdateSignatures(string solarSystemName, List<CosmicSignature> signatures)
        {
            Global.MapApiFunctions.PublishSignatures(Global.Pilots.Selected.Name, Global.Pilots.Selected.SpaceMap.Key, solarSystemName, signatures);

            Global.Pilots.Selected.SpaceMap.Update();
        }

        private void MapViewRelocateSolarSystem(Point newLocation, string relocatedSolarSystem)
        {
            _model.RelocateSolarSystem(newLocation);
        }

        private void MapViewCentreScreenLocationSystem(string obj)
        {
            _view.CentreScreenByLocationSystem();
        }

        private void MapViewCentreScreenSelectedSystem(string obj)
        {
            _view.CentreScreenBySelectedSystem();
        }

        private void MapViewDeleteSelectedSystem(string selectedSolarSystemName)
        {
            Global.MapApiFunctions.DeleteSolarSystem(Global.Pilots.Selected.SpaceMap.Key, selectedSolarSystemName, Global.Pilots.Selected.Name);

            Global.Pilots.Selected.SpaceMap.RemoveSystem(selectedSolarSystemName);

            _view.ForceRefreshMap();
        }

        private void MapToolbarViewSelectPanel(string panelName)
        {
            MapInformationPresenter.ActivatePanel(panelName);
        }

        private void MapViewSelectSolarSystem(string solarSystemName)
        {
            _model.SelectSolarSystem(solarSystemName);

            _model.SpaceMap.SelectedSolarSystemName = solarSystemName;

            if(_model.SpaceMap.GetSystem(solarSystemName).Information == null)
            {
                _model.SpaceMap.GetSystem(solarSystemName).Information = Global.Space.GetSolarSystem(solarSystemName);
            }

            MapInformationPresenter.SetSelectedSolarSystem(_model.SpaceMap.GetSystem(solarSystemName));
        }

        private void MapViewSendMessage(string obj)
        {
            _model.Activate();
            _view.GetMessage("test message");
        }


        public void Run()
        {
            WireUpViewEvents();

            MapInformationPresenter.SetSelectedSolarSystem(_model.SpaceMap.GetSystem(_model.SpaceMap.SelectedSolarSystemName));

            MapInformationPresenter.Run();

            _view.Show();

            _view.DrawSpaceMap(_model.SpaceMap.Systems, _model.SpaceMap.SelectedSolarSystemName);
        }
    }
}
