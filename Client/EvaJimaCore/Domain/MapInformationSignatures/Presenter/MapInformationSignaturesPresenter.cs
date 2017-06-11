using EveJimaCore.Domain.MapToolbar.Contracts;

namespace EveJimaCore.Domain.MapToolbar.Presenter
{
    public class MapInformationSignaturesPresenter : IPresenter
    {
        IMapInformationSignaturesModel _model;
        IMapInformationSignaturesView _view;

        public MapInformationSignaturesPresenter(IMapInformationSignaturesModel model, IMapInformationSignaturesView view)
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
