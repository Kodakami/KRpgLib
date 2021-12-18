using KRpgLib.Utility;
using KRpgLib.Utility.Serialization;
using System;

namespace KRpgLib.Affixes
{
    public sealed class AffixSerializer : Serializer<Affix>
    {
        // Layout:
        // Uint32 AffixTemplate Unique ID Little-Endian     (0 ~ 3)
        // List of ModTemplates                             (4 ~ ?)

        private readonly UInt32Serializer _uniqueIdSerializer = new UInt32Serializer();
        private readonly List

        protected override bool TrySerialize_Internal(Affix affix, ByteWriter writerInProgress)
        {
            // If the affix template has a unique ID in the repo.
            if (AffixEnvironment.Instance.AffixTemplateRepo.TryGetUniqueID(affix.Template, out uint templateID))
            {
                // Try and write the affix unique ID.
                if (_uniqueIdSerializer.TrySerialize(templateID, writerInProgress))
                {

                }
            }
        }

        protected override bool TryDeserialize_Internal(ByteReader readerInProgress, out Affix obj)
        {

        }
    }
    public sealed class ModListSerializer : ListSerializer<Mod>
    {
        protected override bool TrySerializeSingleValue(Mod value, ByteWriter writerInProgress)
        {
            // Get the mod serializer from the mod and toss it back.
            return value.Template.GetModSerializer().TrySerialize(value, writerInProgress);
        }
        protected override bool TryDeserializeSingleValue(ByteReader readerInProgress, out Mod singleValue)
        {
            var templateIdPeek = readerInProgress.GetNextButDoNotAdvance()
            
            // Get the mod serializer from the mod and toss it back.
            return value.Template.GetModSerializer().TryDeserialize(readerInProgress, out singleValue);
        }
    }
}
