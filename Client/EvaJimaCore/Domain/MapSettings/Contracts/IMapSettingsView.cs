
using System;

namespace EveJimaCore.Domain.MapToolbar.Contracts
{
    public interface IMapSettingsView
    {
        event Action<string> ChangeMapKey;

        void Reload();
    }
}
