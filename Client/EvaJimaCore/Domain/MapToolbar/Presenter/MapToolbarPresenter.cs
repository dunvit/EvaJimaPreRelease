using EveJimaCore.Domain.MapToolbar.Contracts;

namespace EveJimaCore.Domain.MapToolbar.Presenter
{
    public class MapToolbarPresenter : IPresenter
    {
        IMapToolbarModel _model;
        IMapToolbarView _view;

        public MapToolbarPresenter(IMapToolbarModel model, IMapToolbarView view)
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
