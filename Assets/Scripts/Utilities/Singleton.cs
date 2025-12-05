using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public bool _DontDestroyOnLoad = true;
    private static T mInstance;
    public static T Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = (T)FindAnyObjectByType(typeof(T));
            }
            return mInstance;
        }
    }

    protected virtual void Awake()
    {
        if (!mInstance)
        {
            mInstance = GetComponent<T>();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        if (_DontDestroyOnLoad)
            DontDestroyOnLoad(Instance);
    }
}
