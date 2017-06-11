
using System;

namespace EveJimaCore.Domain.MapToolbar.Contracts
{
    public interface IMapToolbarView
    {
        string SelectedTab { get; set; }

        event Action<string> SelectTab;
    }
}
