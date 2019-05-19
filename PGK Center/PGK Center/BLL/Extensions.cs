using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PGK_Center.BLL
{
    public static class Extensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(
            this IEnumerable<T> collection) => new ObservableCollection<T>(collection);

        public static void CopyTo<T>(this T itemFrom, T itemTo)
        {
            foreach (var property in itemTo.GetType().GetProperties()
                .Where(a => a.CanWrite))
            {
                var newValue = property.GetValue(itemFrom);

                if (!Equals(property.GetValue(itemTo), newValue))
                    property.SetValue(itemTo, newValue);
            }
        }

        public static bool IsTextContains(this string text, string searchText)
        {
            return text?.ToLower()?.Contains(searchText?.ToLower()) ?? false;
        }
    }
}
