using System;
using EvaJimaCore;

namespace EveJimaCore
{
    public class EveJimaPresenter
    {
        public event Action<BLL.Map.Map> OnLocationChange;
        public event Action<BLL.Map.Map> OnChangeActivePilot;

        public void ActivatePilot(string pilotName)
        {
            if(pilotName == Global.Pilots.Selected.Name)
            {
                OnLocationChange?.Invoke(Global.Pilots.Selected.SpaceMap);
            }
        }

        public void ChangeActivePilot(string pilotName)
        {
            OnChangeActivePilot?.Invoke(Global.Pilots.Selected.SpaceMap);
        }

        public void SelectSolarSystem(string solarSystemName)
        {
            Global.Pilots.Selected.SpaceMap.SelectedSolarSystemName = solarSystemName;
        }
    }
}
