using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Affixes.ModTemplates
{
    public interface IModTemplate<TRolledResult>
    {
        TRolledResult GetNewRolledResult();
    }
}
