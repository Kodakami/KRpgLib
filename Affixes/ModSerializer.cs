using KRpgLib.Utility;
using KRpgLib.Utility.Serialization;
using System;
using System.Collections.Generic;

namespace KRpgLib.Affixes
{
    public abstract class ModSerializer : Serializer<Mod>
    {
        // Layout:
        // Uint32 ModTemplate UniqueID Little-Endian    (0 ~ 3)
        // [unknown bytes from subclass]                (4 ~ ?)

        protected override bool TrySerialize_Internal(Mod modInstance, ByteWriter writerInProgress)
        {
            if (modInstance != null)
            {
                // Try and get the mod template's unique ID from the repo.
                if (AffixEnvironment.Instance.ModTemplateRepo.TryGetUniqueID(modInstance.Template, out uint templateID))
                {
                    // Convert to bytes.
                    var templateIdBytes = BitConverter.GetBytes(templateID);

                    // Add the bytes.
                    writerInProgress.WriteBytes(templateIdBytes, reverseIfBigEndian: true);

                    // Serialize the subclass's data and return the result.
                    return TrySerializeSubclass(modInstance, writerInProgress);
                }
            }

            // The provided mod was NULL, the repo did not contain the mod template, or the subclass failed to serialize.
            return false;
        }
        protected abstract bool TrySerializeSubclass(Mod modInstance, ByteWriter writerInProgress);

        protected override bool TryDeserialize_Internal(ByteReader readerInProgress, out Mod modInstance)
        {
            // Always 4.
            const int uintSize = sizeof(uint);

            if (readerInProgress.Remaining >= uintSize)
            {
                var templateUniqueIdBytes = readerInProgress.GetNextAndAdvance(uintSize, reverseIfBigEndian: true);

                // Get the unique ID of the mod template.
                uint templateUniqueID = BitConverter.ToUInt32(templateUniqueIdBytes, 0);

                // If the repo has a template for the ID.
                if (AffixEnvironment.Instance.ModTemplateRepo.TryGetObject(templateUniqueID, out ModTemplate template))
                {
                    // Deserialize the subclass's data and return the result.
                    return TryDeserializeSubclass(readerInProgress, template, out modInstance);
                }
            }

            // Not enough bytes, mod template not found, or subclass deserialization failed.
            modInstance = default;
            return false;
        }
        protected abstract bool TryDeserializeSubclass(ByteReader readerInProgress, ModTemplate template, out Mod modInstance);
    }
    public sealed class ModSerializer<TArg> : ModSerializer
    // TArg is any type. Matched with Mod<TArg>'s TArg type.
    {
        protected override bool TrySerializeSubclass(Mod modInstance, ByteWriter writerInProgress)
        {
            // Base class serializes the template unique ID.

            // Cast and check for success. Return the result of the provided arg serializer.
            return modInstance is Mod<TArg> cast && cast.Template.GetArgSerializer().TrySerialize(cast.Arg, writerInProgress);
        }

        protected override bool TryDeserializeSubclass(ByteReader readerInProgress, ModTemplate template, out Mod modInstance)
        {
            // Base class deserializes the template unique ID.

            // If the template type is congruent with this arg type AND bytes deserialize to an arg value,
            if (template is ModTemplate<TArg> castTemplate && castTemplate.GetArgSerializer().TryDeserialize(readerInProgress, out TArg arg))
            {
                // Create the new mod.
                modInstance = new Mod<TArg>(castTemplate, arg);
                return true;
            }

            // Template was not a ModTemplate<TArg> with the same TArg type OR arg did not deserialize correctly.
            modInstance = default;
            return false;
        }
    }
}