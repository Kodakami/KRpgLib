using KRpgLib.Flags;
using KRpgLib.Utility.Serialization;
using System;

namespace KRpgLib.Flags
{
    public sealed class FlagSerializer : Serializer<Flag>
    {
        // Layout:
        // Uint32 FlagTemplate Unique ID Little-Endian      (0 ~ 3)
        // Int32 Variant Index Little-Endian                (4 ~ 7)

        public static readonly FlagSerializer Singleton = new FlagSerializer();

        protected override bool TrySerialize_Internal(Flag flag, ByteWriter writerInProgress)
        {
            // Try and get the flag template ID from the repo.
            if (FlagEnvironment.Instance.FlagTemplateRepo.TryGetUniqueID(flag.Template, out uint templateID))
            {
                // Template ID.
                UInt32Serializer.Singleton.SerializeImmediate(templateID, writerInProgress);

                // Variant Index.
                Int32Serializer.Singleton.SerializeImmediate(flag.VariantIndex, writerInProgress);

                return true;
            }

            // Repo did not contain the flag template.
            return false;
        }

        protected override bool TryDeserialize_Internal(ByteReader readerInProgress, out Flag flag)
        {
            // Big if.

            // If the flag template ID deserializes correctly,
            if (UInt32Serializer.Singleton.TryDeserialize(readerInProgress, out uint templateID)

                // AND the unique ID corresponds to a valid flag template,
                && FlagEnvironment.Instance.FlagTemplateRepo.TryGetObject(templateID, out IFlagTemplate flagTemplate)

                // AND the variant index deserializes correctly,
                && Int32Serializer.Singleton.TryDeserialize(readerInProgress, out int variantIndex)

                // AND the variant index is in range,
                && variantIndex > 0 && variantIndex < flagTemplate.VariantCount)
            {
                // Create the flag.
                flag = Flag.Create(flagTemplate, variantIndex);

                // Deserialization successful.
                return true;
            }

            // Any of the above failed.
            flag = default;
            return false;
        }
    }
}