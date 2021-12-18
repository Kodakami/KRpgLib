using System.Collections.Generic;
using KRpgLib.Affixes.AffixTypes;
using KRpgLib.Stats;
using KRpgLib.Flags;

namespace KRpgLib.Affixes
{
    public class AffixTemplateBuilder
    {
        private readonly List<ModTemplate> _rawModTemplates = new List<ModTemplate>();
        private readonly List<StatDeltaValueBounds> _statDeltaValueBounds = new List<StatDeltaValueBounds>();
        private readonly FlagCollection _flags = new FlagCollection();

        public AffixType AffixType { get; set; }

        public void AddRaw(ModTemplate modTemplate)
        {
            _rawModTemplates.Add(modTemplate);

            // TODO: Fix this class so it returns mod templates as they're created (so the client coder can keep handles).
        }

        public void AddStatDelta(Stat stat, DeltaType deltaType, int minRoll, int maxRoll, int precision = 1)
        {
            _statDeltaValueBounds.Add(new StatDeltaValueBounds(stat, deltaType, minRoll, maxRoll, precision));
        }
        public void AddFlag(Flag flag)
        {
            _flags.AddIfNotPresent(flag);
        }
        public AffixTemplate Build()
        {
            List<ModTemplate> templates = new List<ModTemplate>(_rawModTemplates);
            foreach (var sdvb in _statDeltaValueBounds)
            {
                templates.Add(new StatDeltaModTemplate(sdvb));
            }
            foreach (var flag in _flags)
            {
                templates.Add(new FlagModTemplate(flag));
            }
            var yourMods = BuildYourModTemplatesHere();
            if (yourMods != null)
            {
                templates.AddRange(yourMods);
            }

            return new AffixTemplate(AffixType, templates);
        }
        protected virtual IEnumerable<ModTemplate> BuildYourModTemplatesHere() => null;
    }
}
