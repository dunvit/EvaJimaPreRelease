using System.Drawing;
using System.Windows.Forms;
using EvaJimaCore;
using EveJimaCore.BLL.Map;
using EveJimaCore.Domain.Map.Contracts;
using EveJimaCore.Domain.Map.Modes;
using EveJimaCore.Domain.Map.Presenter;
using EveJimaCore.Domain.Map.View;
using EveJimaCore.Domain.MapInformation.View;
using EveJimaCore.Domain.MapToolbar.Contracts;
using EveJimaCore.Domain.MapToolbar.Modes;
using EveJimaCore.Domain.MapToolbar.Presenter;
using EveJimaCore.Domain.MapToolbar.View;

namespace EveJimaCore.WhlControls
{
    public partial class crlSpaceMap : baseContainer
    {
        readonly WindowMap _windowMap = new WindowMap();

        readonly ElementMapToolbatView _toolbarMapView = new ElementMapToolbatView();

        readonly ElementMapInformation _elementMapInformationView = new ElementMapInformation();

        private MapPresenter _mapPresenter;

        private MapInformationPresenter _mapInformationPresenter;

        private bool _isActivated;

        public crlSpaceMap()
        {
            InitializeComponent();
        }

        public override void ActivateContainer()
        {
            if(_isActivated) return;

            #region Map Control Initialization

            _windowMap.TopLevel = false;
            _windowMap.Location = new Point(5, 5);
            _windowMap.Size = new Size(800, 900);
            _windowMap.FormBorderStyle = FormBorderStyle.None;
            _windowMap.Visible = true;
            _windowMap.Dock = DockStyle.Fill;

            mapContainer.Controls.Add(_windowMap);

            MapTools.Normalization(Global.Pilots.Selected.SpaceMap);

            IMapModel model = new MapModel { SpaceMap = Global.Pilots.Selected.SpaceMap };

            #endregion

            #region Toolbar Control Initialization

            _toolbarMapView.TopLevel = false;

            _toolbarMapView.Location = new Point(0, 0);
            _toolbarMapView.Size = new Size(800, 900);
            _toolbarMapView.FormBorderStyle = FormBorderStyle.None;
            _toolbarMapView.Visible = true;
            _toolbarMapView.Dock = DockStyle.Fill;

            toolbarContainer.Controls.Add(_toolbarMapView);

            IMapToolbarModel mapToolbarModel = new MapToolbarModel();

            #endregion

            #region Map Information panel Initialization

            IMapInformationModel mapInformationModel = new MapInformationModel { SpaceMap = Global.Pilots.Selected.SpaceMap };

            _elementMapInformationView.TopLevel = false;

            _elementMapInformationView.Location = new Point(0, 0);
            _elementMapInformationView.Size = new Size(800, 900);
            _elementMapInformationView.FormBorderStyle = FormBorderStyle.None;
            _elementMapInformationView.Visible = true;
            _elementMapInformationView.Dock = DockStyle.Fill;

            informationContainer.Controls.Add(_elementMapInformationView);

            _mapInformationPresenter = new MapInformationPresenter(mapInformationModel, _elementMapInformationView);
            

            #endregion

            _mapPresenter = new MapPresenter(model, _windowMap, mapToolbarModel, _toolbarMapView);

            _mapPresenter.MapInformationPresenter = _mapInformationPresenter;

            _mapPresenter.Run();

            _isActivated = true;
        }

        private void MapSettingsEventChangeMapKey(string obj)
        {

        }

        public void Event_Map_OnResize()
        {

        }
    }
}
