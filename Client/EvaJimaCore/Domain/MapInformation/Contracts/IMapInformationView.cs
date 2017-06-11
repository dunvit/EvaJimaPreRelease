
using System;
using System.Collections.Generic;
using EveJimaUniverse;

namespace EveJimaCore.Domain.MapToolbar.Contracts
{
    public interface IMapInformationView : IView
    {


        void ActivateContainer();

        void ShowInformationPanel(string panelName);

        void ChangeSolarSystem(SolarSystem solarSystem);

        event Action<string> DeleteSelectedSystem;

        event Action<string> DeathNotice;

        event Action<string> CentreScreenSelectedSystem;

        event Action<string> CentreScreenLocationSystem;

        event Action<string, List<CosmicSignature>> UpdateSignatures;

        event Action<string> ChangeMapKey;
    }
}
