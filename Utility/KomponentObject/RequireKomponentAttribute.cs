using System;

namespace KRpgLib.Utility.KomponentObject
{
    /// <summary>
    /// If a komponent has this attribute, then a komponent object must already have at least one registered instance of the provided komponent type before this komponent can be registered.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class RequireKomponentAttribute : Attribute
    {
        public Type RequiredType { get; }

        public RequireKomponentAttribute(Type requiredType)
        {
            if (requiredType == null)
            {
                throw new ArgumentNullException(nameof(requiredType));
            }
            if (!requiredType.IsSubclassOf(typeof(Komponent)))
            {
                throw new ArgumentException("Argument must be a subclass of Komponent.", nameof(requiredType));
            }

            RequiredType = requiredType;
        }
    }
}
