using EveJimaCore.Domain.MapToolbar.Contracts;

namespace EveJimaCore.Domain.MapToolbar.Presenter
{
    public class TemplatePresenter : IPresenter
    {
        ITemplateModel _model;
        ITemplateView _view;

        public TemplatePresenter(ITemplateModel model, ITemplateView view)
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
