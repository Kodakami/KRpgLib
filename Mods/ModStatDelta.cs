using System;
using KRpgLib.Mods.ModTemplates;
using KRpgLib.Flags;

namespace KRpgLib.Mods
{
    public struct ModStatDelta<TValue> : IMod<MTStatDelta<TValue>, TValue> where TValue : struct
    {
        public MTStatDelta<TValue> Template { get; }
        public TValue RolledResult { get; }

        public ModStatDelta(MTStatDelta<TValue> template, TValue rolledValue)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
            RolledResult = rolledValue;
        }
        public ModStatDelta(MTStatDelta<TValue> template, Random rng)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
            RolledResult = template.GetNewRolledResult(rng ?? throw new ArgumentNullException(nameof(rng)));
        }
    }
    public struct ModFlag : IMod<AbstractMTFlag, Flag>
    {
        public AbstractMTFlag Template { get; }
        public Flag RolledResult { get; }
        
        public ModFlag(AbstractMTFlag template, Flag rolledValue)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
            RolledResult = rolledValue;
        }
        public ModFlag(AbstractMTFlag template, Random rng)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
            RolledResult = template.GetNewRolledResult(rng ?? throw new ArgumentNullException(nameof(rng)));
        }
    }
}
