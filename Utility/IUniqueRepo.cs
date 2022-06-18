using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLib.Utility
{
    public interface IUniqueRepo<T>
    {
        bool TryGetObject(uint uniqueID, out T obj);
        bool TryGetUniqueID(T obj, out uint uniqueID);
    }
}
