using System;
using System.Collections.Generic;
using System.Drawing;
using EveJimaUniverse;

namespace EveJimaCore.Domain.Map.Contracts
{
    public interface IMapView : IView
    {


        event EventHandler DataChanged;

        event Action<string> SendMessage;

        void GetMessage(string message);

        void ForceRefreshMap();

        event Action<string> SelectSolarSystem;

        event Action<Point, string> RelocateSolarSystem;

        void DrawSpaceMap(List<SolarSystem> systems, string centreSystem);

        void CentreScreenBySelectedSystem();

        void CentreScreenByLocationSystem();
    }
}
