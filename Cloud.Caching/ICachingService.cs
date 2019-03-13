namespace Cloud.Caching
{
    public interface ICachingService
    {
        bool TryGet<T>(string key, out T value)
            where T : class;

        void Set<T>(string key, T value)
            where T : class;

        void DeleteKey(string key);
    }
}
