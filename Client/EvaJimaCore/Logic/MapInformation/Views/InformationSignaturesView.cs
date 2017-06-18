using System.Windows.Forms;
using EveJimaCore.BLL.Map;
using EveJimaCore.Logic.MapInformation.Views;

namespace EveJimaCore.Logic.MapInformation
{
    public partial class InformationSignaturesView : UserControl, IMapInformationControl
    {
        public InformationSignaturesView()
        {
            InitializeComponent();
        }

        public void ForceRefresh(Map spaceMap)
        {

        }
    }
}
