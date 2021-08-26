using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Affixes
{
    public interface IModTemplate<TRolledResult>
    {
        TRolledResult GetNewRolledResult();
    }
}
