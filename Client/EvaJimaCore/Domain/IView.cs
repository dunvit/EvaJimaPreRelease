
namespace EveJimaCore.Domain
{
    public interface IView
    {
        void Hide();
        void Show();
        void Close();

        void ForceRefresh();
    }
}
