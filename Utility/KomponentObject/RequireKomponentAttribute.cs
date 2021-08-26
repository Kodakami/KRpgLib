using System;

namespace KRpgLib.Utility.KomponentObject
{
    /// <summary>
    /// If a komponent has this attribute, then a komponent object must already have at least one registered instance of the provided komponent type before this komponent can be registered.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class RequireKomponentAttribute : Attribute
    {
        private const string IKOMPONENT_TYPE_NAME = "KRpgLib.Utility.KomponentObject.IKomponent";

        public Type RequiredType { get; }

        public RequireKomponentAttribute(Type requiredType)
        {
            if (requiredType == null)
            {
                throw new ArgumentNullException(nameof(requiredType));
            }

            // If the required type does not implement the IKomponent interface.
            if (requiredType.FindInterfaces((typeObj, comparitiveObject) => typeObj.ToString().Equals(comparitiveObject.ToString(), StringComparison.Ordinal), IKOMPONENT_TYPE_NAME).Length == 0)
            {
                throw new ArgumentException("Argument must implement interface IKomponent.", nameof(requiredType));
            }

            RequiredType = requiredType;
        }
    }
}
