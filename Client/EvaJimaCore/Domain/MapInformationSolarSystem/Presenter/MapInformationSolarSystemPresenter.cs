using EveJimaCore.Domain.MapToolbar.Contracts;

namespace EveJimaCore.Domain.MapToolbar.Presenter
{
    public class MapPanelSystemInformationPresenter : IPresenter
    {
        IMapInformationSolarSystemModel _model;
        IMapInformationSolarSystemView _view;

        public MapPanelSystemInformationPresenter(IMapInformationSolarSystemModel model, IMapInformationSolarSystemView view)
        {
            _model = model;
            _view = view;

            WireUpViewEvents();
        }

        private void WireUpViewEvents()
        {
            //_view.SendMessage += ViewSendMessage;
        }

        public void Run()
        {

        }
    }
}
