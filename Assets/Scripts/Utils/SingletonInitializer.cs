/// <summary>
/// A typical singleton pattern implementation.
/// Provides additional step of code execution on initialization.
/// Inherited classes must implement the IInitializer interface.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingletonInitializer<T> where T : class, IInitializer, new()
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
                _instance.Initialize();
            }
            return _instance;
        }
    }
}
