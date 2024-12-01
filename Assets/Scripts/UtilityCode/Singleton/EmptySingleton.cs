namespace UtilityCode.Singleton
{
    public class EmptySingleton<T> where T : class, new()
    {
        static EmptySingleton() //构造函数
        {
            Instance = new T();
        }

        public static T Instance { get; }
    }
}