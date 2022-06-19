using KRpgLib.Utility;
using KRpgLib.Utility.Serialization;
using System;
using System.Collections.Generic;

namespace KRpgLib.Affixes.Serialization
{
    public sealed class ModSerializer : Serializer<Mod>
    {
        // Layout:
        // Uint32 ModTemplate UniqueID Little-Endian    (0 ~ 3)
        // [unknown bytes from arg]                     (4 ~ ?)

        public static readonly ModSerializer Singleton = new ModSerializer();

        protected override bool TrySerialize_Internal(Mod modInstance, ByteWriter writerInProgress)
        {
            if (modInstance != null)
            {
                // Try and get the mod template's unique ID from the repo.
                if (AffixEnvironment.Instance.ModTemplateRepo.TryGetUniqueID(modInstance.Template, out uint templateID))
                {
                    // Write the template ID.
                    UInt32Serializer.Singleton.SerializeImmediate(templateID, writerInProgress);

                    // If there's an arg to serialize,
                    if (modInstance.HasArg)
                    {
                        // Get the mod arg type.
                        var argType = modInstance.Template.ArgType;

                        // Try and write the arg value.
                        return argType.TrySerialize(modInstance.ArgValue, writerInProgress);
                    }
                    else
                    {
                        // Mod has no arg. Serialization complete.
                        return true;
                    }
                }
            }

            // The provided mod was NULL, the repo did not contain the mod template, or the mod had an arg which failed to serialize.
            return false;
        }

        protected override bool TryDeserialize_Internal(ByteReader readerInProgress, out Mod modInstance)
        {
            // Try and read the unique ID of the mod template.
            if (UInt32Serializer.Singleton.TryDeserialize(readerInProgress, out uint templateID))
            {
                // If the repo has a template for the ID.
                if (AffixEnvironment.Instance.ModTemplateRepo.TryGetObject(templateID, out ModTemplate template))
                {
                    // If the template has an arg,
                    if (template.HasArg)
                    {
                        // Get the mod arg type.
                        var argType = template.ArgType;

                        // Try and read the arg value.
                        if (argType.TryDeserialize(readerInProgress, out object weakArgValue))
                        {
                            modInstance = new Mod(template, weakArgValue);
                            return true;
                        }
                    }
                    else
                    {
                        // No Arg for this mod template.
                        modInstance = new Mod_NoArg(template);
                        return true;
                    }
                }
            }

            // Could not read mod template unique ID, could not find template in repo, or mod has arg which did not deserialize correctly.
            modInstance = default;
            return false;
        }
    }
}