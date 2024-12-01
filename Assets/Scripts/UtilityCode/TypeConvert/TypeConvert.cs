namespace UtilityCode.TypeConvert
{
    public class TypeConvert
    {
        public static bool TryConvert<T>(object obj, out T result) where T : class
        {
            result = default;
            if (obj is not T)
            {
                return false;
            }

            result = obj as T;
            return true;
        }
    }
}