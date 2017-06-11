using EveJimaCore.Domain.MapToolbar.Contracts;

namespace EveJimaCore.Domain.MapToolbar.Presenter
{
    public class MapSettingsPresenter : IPresenter
    {
        IMapSettingsModel _model;
        IMapSettingsView _view;

        public MapSettingsPresenter(IMapSettingsModel model, IMapSettingsView view)
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
            _view.Reload();
        }
    }
}
