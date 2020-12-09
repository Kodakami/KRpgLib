using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Mods.ModTemplates
{
    public interface IModTemplate<TRolledResult>
    {
        TRolledResult GetNewRolledResult();
    }
}
