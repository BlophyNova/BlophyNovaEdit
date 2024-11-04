using System.Text.RegularExpressions;
using UnityEditor;

namespace Smart
{
	public partial class Inspector
    {
        //
        // Property Search
        //

        public static string filter = "";

        static bool Filter(Editor editor)
        {
            if (null == editor) { return false; }

            if (filter == "") { return false; }

            string[] parts = filter.Split('.');

            string filterComponent = parts[0];

            string name = editor.target.GetType().Name;

            if (matchWord.Value)
            {
                return !Regex.IsMatch(filterComponent, string.Format(@"\b{0}\b", name));
            }

            name = name.ToLower();

            return !name.Contains(filterComponent.ToLower());
        }

        static bool FilterProperty(string propertyName)
        {
            string[] parts = filter.Split('.');

            string filterProperty = parts[parts.Length - 1];

            if (string.IsNullOrWhiteSpace(filterProperty)) { return false; }

            propertyName = propertyName.Replace(" ", string.Empty);

            if (matchWord.Value)
            {
                return !Regex.IsMatch(filterProperty, string.Format(@"\b{0}\b", propertyName));
            }

            propertyName = propertyName.ToLower();

            return !propertyName.Contains(filterProperty.ToLower());
        }

        static bool searchingProperty
        {
            get => filter.Contains(".");
        }

        static bool searching
        {
            get => !string.IsNullOrWhiteSpace(filter);
        }
    }
}
