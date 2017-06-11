using System;
using System.Collections.Generic;
using System.Drawing;

namespace EveJimaUniverse
{
    public class SolarSystem : ICloneable
    {
        public string Name { get; set; }

        public Point LocationInMap { get; set; }

        public List<string> Connections = new List<string>();

        public List<CosmicSignature> Signatures = new List<CosmicSignature>();

        public DateTime LastUpdate = DateTime.UtcNow;

        public DateTime Created { get; set; }

        public StarSystemEntity Information { get; set; }

        public string Type { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsHidden { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void AddSignature(CosmicSignature signature)
        {
            var cosmicSignature = GetSignature(signature.Code);

            if ( cosmicSignature == null )
            {
                // ---- Create new signature
                Signatures.Add(signature);
                return;
            }

            // ---- Update exist signature
            cosmicSignature.LastUpdate = signature.LastUpdate;
            cosmicSignature.Name = signature.Name;
            cosmicSignature.Type = signature.Type;

        }

        public CosmicSignature GetSignature(string code)
        {
            foreach (var cosmicSignature in Signatures)
            {
                if (cosmicSignature.Code == code)
                {
                    return cosmicSignature;
                }
            }

            return null;
        }
    }
}
