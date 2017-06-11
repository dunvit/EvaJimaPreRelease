using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EveJimaUniverse;

namespace EveJimaCore.Logic
{
    interface IAMapInformationView
    {
        BLL.Map.Map SpaceMap { get; set; }

        void ShowInformationPanel(string panelName);

        void ChangeSolarSystem(SolarSystem solarSystem);

        event Action<string> DeleteSelectedSystem;

        event Action<string> CentreScreenSelectedSystem;

        event Action<string> CentreScreenLocationSystem;

        event Action<string, List<CosmicSignature>> UpdateSignatures;

        event Action<string> ChangeMapKey;
    }
}
