using System;
using System.Collections.Generic;
using EveJimaUniverse;

namespace EveJimaCore.Domain.MapToolbar.Contracts
{
    public interface IMapInformationSignaturesView
    {
        event Action<string, List<CosmicSignature>> UpdateSignatures;
    }
}
