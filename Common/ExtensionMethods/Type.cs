using System;
using System.Linq;
using System.Text;

namespace System.ExtensionMethods
{
    /// <summary>
    ///     Class containing extension methods for System.Type
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        ///     Checks whether a System.Type is the same or a subclass of this System.Type
        /// </summary>
        /// <param name="descendantType">The type that is a potential derived type</param>
        /// <param name="baseType">The type that is a potential base type</param>
        /// <returns>True if descendantType is the same or a derived type of baseClass, false otherwise</returns>
        public static bool IsSameOrSubclass(this Type descendantType, Type baseType)
        {
            if (baseType.IsInterface)
                return baseType.IsAssignableFrom(descendantType);

            return descendantType.IsSubclassOf(baseType) || descendantType == baseType;
        }
    }
}
