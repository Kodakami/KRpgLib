using KRpgLib.Utility;
using KRpgLib.Utility.Serialization;
using System;
using System.Collections.Generic;

namespace KRpgLib.Affixes.Serialization
{
    public sealed class AffixSerializer : Serializer<Affix>
    {
        // Layout:
        // Uint32 AffixTemplate Unique ID Little-Endian     (0 ~ 3)
        // List of ModTemplates                             (4 ~ ?)

        public static readonly AffixSerializer Singleton = new AffixSerializer();

        private readonly ListSerializer<Mod> _modListSerializer = new UniformListSerializer<Mod>(ModSerializer.Singleton);

        protected override bool TrySerialize_Internal(Affix affix, ByteWriter writerInProgress)
        {
            // If the affix template has a unique ID in the repo.
            if (AffixEnvironment.Instance.AffixTemplateRepo.TryGetUniqueID(affix.Template, out uint templateID))
            {
                // Try and write the affix unique ID.
                if (UInt32Serializer.Singleton.TrySerialize(templateID, writerInProgress))
                {
                    // Try and write the list of mods.
                    return _modListSerializer.TrySerialize(affix.GetAllModsForSerialization(), writerInProgress);
                }
            }

            return false;
        }

        protected override bool TryDeserialize_Internal(ByteReader readerInProgress, out Affix obj)
        {
            // Try and read the affix template ID.
            if (UInt32Serializer.Singleton.TryDeserialize(readerInProgress, out uint templateID))
            {
                // Try and find the affix template in the repo.
                if (AffixEnvironment.Instance.AffixTemplateRepo.TryGetObject(templateID, out AffixTemplate template))
                {
                    // Try and read the list of mods.
                    if (_modListSerializer.TryDeserialize(readerInProgress, out IReadOnlyList<Mod> modList))
                    {
                        obj = new Affix(template, modList);
                        return true;
                    }
                }
            }

            obj = default;
            return false;
        }
    }
}
