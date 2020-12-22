using KRpgLib.Affixes.ModTemplates;

namespace KRpgLib.Affixes
{
    public interface IMod<TModTemplate, TRolledResult>
        where TModTemplate : IModTemplate<TRolledResult>
    {
        TModTemplate Template { get; }
        TRolledResult RolledResult { get; }
    }
}
