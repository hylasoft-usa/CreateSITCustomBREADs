using System.Text;
using System.Reflection;
using System.Collections.Generic;

namespace System.ExtensionMethods
{
    /// <summary>
    ///     Class containing extension methods for generic collections
    /// </summary>
    public static class GenericListExtensions
    {
        /// <summary>
        ///     Prints a custom delimited string of all the values in a collection
        /// </summary>
        /// <typeparam name="T">Type of the elements in the collection</typeparam>
        /// <param name="collection">The collection to be printed as custom delimited</param>
        /// <param name="delimiter">The custom delimiter to use</param>
        /// <returns>A custom delimited representation of the collection or empty string if the collection is empty</returns>
        public static string ToDelimitedString<T>(this IList<T> collection, string delimiter)
        {
            if (collection.Count == 0)
                return string.Empty;
            else
            {
                List<string> items = new List<string>();

                foreach (T item in collection)
                    items.Add(item.ToString());

                return string.Join(delimiter, items.ToArray());
            }
        }

        /// <summary>
        ///     Prints a comma separated string of all the values in a collection
        /// </summary>
        /// <typeparam name="T">Type of the elements in the collection</typeparam>
        /// <param name="collection">The collection to be printed as comma separated string</param>
        /// <returns>A comma separated value representation of the collection or empty string if the collection is empty</returns>
        public static string ToCommaSeparatedString<T>(this IList<T> collection)
        {
            return ToDelimitedString(collection, ",");
        }

        /// <summary>
        ///     Prints a comma separated string of all the values in a collection suitable to be used between parenthesis in a SQL IN condition
        /// </summary>
        /// <typeparam name="T">Type of the elements in the collection</typeparam>
        /// <param name="collection">The collection to be printed </param>
        /// <returns>A comma separated value representation of the collection suitable to be used between parenthesis in a SQL IN condition, or empty string if the collection is empty</returns>
        public static string ToCommaSeparatedStringForSQLINCondition<T>(this IList<T> collection)
        {
            if (collection.Count == 0)
                return string.Empty;
            else
            {
                if (typeof(T).IsSameOrSubclass(typeof(string)) || typeof(T).IsSameOrSubclass(typeof(char)))
                    return string.Format("'{0}'", collection.ToDelimitedString("','"));
                else
                    return collection.ToCommaSeparatedString();
            }
        }

        /// <summary>
        ///     Scans a collection of objects and returns a second collection containing only the values of a given property
        /// </summary>
        /// <param name="collection">The source collection</param>
        /// <param name="propertyName">The property (belonging to T) whose values will form the returned collection</param>
        /// <typeparam name="ListItemsType">Type of items in the collection</typeparam>
        /// <typeparam name="PropertyType">Type of the propery being scanned</typeparam>
        /// <returns>A collection containing all the values of the given property for each element of the source collection</returns>
        public static List<PropertyType> ToPropertyList<ListItemsType, PropertyType>(this IList<ListItemsType> collection, string propertyName)
        {
            List<PropertyType> toRet = new List<PropertyType>();

            if (collection.Count > 0)
            {
                PropertyInfo pi = typeof(ListItemsType).GetProperty(propertyName);

                if (pi != null)
                {
                    toRet = new List<PropertyType>();

                    foreach (ListItemsType item in collection)
                        toRet.Add((PropertyType)pi.GetValue(item, null));
                }
            }

            return toRet;
        }

        /// <summary>
        ///     Finds all elements in a collection having a property value equal to a given value
        /// </summary>
        /// <typeparam name="T">Type of items in the collection</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="propertyName">The name of the property which value has to be checked</param>
        /// <param name="propertyValue">The value to be compared - comparison will be executed using the Equals method</param>
        /// <returns>The list of elements inside the collection having propertyName's value equals to propertyValue</returns>
        public static List<T> FindByPropertyValue<T>(this IList<T> collection, string propertyName, object propertyValue)
        {
            List<T> toRet = new List<T>();

            foreach (T item in collection)
            {
                PropertyInfo pi = typeof(T).GetProperty(propertyName);

                if (pi != null)
                {
                    object val = pi.GetValue(item, null);

                    if ((val == null) && (propertyValue == null))
                        toRet.Add(item);
                    else
                    {
                        if (val.Equals(propertyValue))
                            toRet.Add(item);
                    }
                }
            }

            return toRet;
        }
    }
}
