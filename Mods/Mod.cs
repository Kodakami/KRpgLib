using KRpgLib.Affixes.ModTemplates;
using System;

namespace KRpgLib.Affixes
{
    public sealed class Mod<TModTemplate, TRolledResult>
        where TModTemplate : class, IModTemplate<TRolledResult>
    {
        public TModTemplate Template { get; }
        public TRolledResult RolledResult { get; }

        public Mod(TModTemplate template, TRolledResult rolledResult)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
            RolledResult = rolledResult;
        }
    }
}
