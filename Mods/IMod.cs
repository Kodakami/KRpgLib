using KRpgLib.Mods.ModTemplates;

namespace KRpgLib.Mods
{
    public interface IMod<TModTemplate, TRolledResult>
        where TModTemplate : IModTemplate<TRolledResult>
    {
        TModTemplate Template { get; }
        TRolledResult RolledResult { get; }
    }
}
