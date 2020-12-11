using System;

namespace KRpgLib.Utility.KomponentObject
{
    /// <summary>
    /// If a komponent has this attribute, then a komponent object can have multiple instances of the komponent.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class AllowMultipleKomponentInstancesAttribute : Attribute { }
}
