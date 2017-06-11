using System;
using System.Collections.Generic;
using EveJimaCore.Domain.MapToolbar.Contracts;
using EveJimaUniverse;

namespace EveJimaCore.Domain.MapToolbar.Presenter
{
    public class MapInformationPresenter : IPresenter
    {
        IMapInformationModel _model;
        IMapInformationView _view;

        public event Action UpdateMap;
        public event Action<string> DeleteSelectedSystem;
        public event Action<string> CentreScreenSelectedSystem;
        public event Action<string> CentreScreenLocationSystem;
        public event Action<string, List<CosmicSignature>> UpdateSignatures;

        public MapInformationPresenter(IMapInformationModel model, IMapInformationView view)
        {
            _model = model;
            _view = view;

            WireUpViewEvents();
        }

        public void UpdateView()
        {
            _view.ForceRefresh();
        }

        private void WireUpViewEvents()
        {
            _view.DeleteSelectedSystem += ViewDeleteSolarSystem;
            _view.CentreScreenSelectedSystem += ViewCentreScreenSelectedSystem;
            _view.CentreScreenLocationSystem += ViewCentreScreenLocationSystem;
            _view.UpdateSignatures += ViewUpdateSignatures;
            _view.ChangeMapKey += ViewChangeMapKey;
            _view.DeathNotice += ViewDeathNotice;
        }

        private void ViewDeathNotice(string locationSolarSystem)
        {
            _model.DeathNotice(locationSolarSystem);

            if (UpdateMap != null) UpdateMap();
        }

        private void ViewChangeMapKey(string newMapKey)
        {
            _model.ChangeMapKey(newMapKey);

            if(UpdateMap != null) UpdateMap();
        }

        private void ViewUpdateSignatures(string solarSystemName, List<CosmicSignature> signatures)
        {
            if (UpdateSignatures != null) UpdateSignatures(solarSystemName, signatures);
        }

        private void ViewCentreScreenLocationSystem(string obj)
        {
            CentreScreenLocationSystem(obj);
        }

        private void ViewCentreScreenSelectedSystem(string obj)
        {
            if (CentreScreenSelectedSystem != null) CentreScreenSelectedSystem(obj);
        }

        private void ViewDeleteSolarSystem(string obj)
        {
            if(DeleteSelectedSystem != null) DeleteSelectedSystem(obj);
        }

        public void SetSelectedSolarSystem(SolarSystem system)
        {
            _model.SpaceMap.SelectedSolarSystemName = system.Name;
            _view.ChangeSolarSystem(system);
        }

        public void ActivatePanel(string panelName)
        {
            _view.ShowInformationPanel(panelName);
        }

        public void Run()
        {
            _view.Show();

            _view.ActivateContainer();

            _view.ShowInformationPanel("SolarSystem");
        }
    }
}
