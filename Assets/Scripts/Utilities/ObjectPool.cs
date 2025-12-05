using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// A Generic, non-MonoBehaviour Object Pool.
/// </summary>
/// <typeparam name="T">The Component type to pool (e.g., BaseProjectile, Enemy).</typeparam>
public class ObjectPool<T> where T : Component
{
    protected T prefab;
    protected Transform parent;
    protected Queue<T> poolQueue = new Queue<T>();
    protected Action<T> onGet; 

    public ObjectPool(T prefab, int initialSize, Transform parent = null, Action<T> onGet = null)
    {
        this.prefab = prefab;
        this.parent = parent;
        this.onGet = onGet;

        for (int i = 0; i < initialSize; i++)
        {
            CreateNewInstance();
        }
    }

    protected virtual T CreateNewInstance()
    {
        T obj = UnityEngine.Object.Instantiate(prefab, parent);
        obj.gameObject.SetActive(false);
        poolQueue.Enqueue(obj);
        return obj;
    }

    /// <summary>
    /// Retrieves an object from the pool.
    /// Virtual to allow custom logic in derived pools (e.g., custom initialization).
    /// </summary>
    public virtual T Get()
    {
        T obj;
        if (poolQueue.Count > 0)
        {
            obj = poolQueue.Dequeue();
        }
        else
        {
            obj = CreateNewInstance(); 
            poolQueue.Dequeue();
        }

        obj.gameObject.SetActive(true);
        onGet?.Invoke(obj);
        return obj;
    }

    /// <summary>
    /// Returns an object to the pool.
    /// </summary>
    public virtual void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        poolQueue.Enqueue(obj);
    }
}

/// <summary>
/// Static Factory class to easily create object pools.
/// </summary>
public static class ObjectPooler
{
    /// <summary>
    /// Creates a new ObjectPool for a specific component type.
    /// </summary>
    /// <typeparam name="T">The component type (e.g., BaseProjectile).</typeparam>
    /// <param name="prefab">The prefab to spawn.</param>
    /// <param name="initialSize">Initial number of objects.</param>
    /// <param name="parent">Optional transform parent for organization.</param>
    /// <param name="onGet">Optional action to run when an object is retrieved.</param>
    /// <returns>A new ObjectPool instance.</returns>
    public static ObjectPool<T> CreatePool<T>(T prefab, int initialSize, Transform parent = null, Action<T> onGet = null) where T : Component
    {
        if (prefab == null)
        {
            Debug.LogError("[ObjectPooler] Cannot create pool: Prefab is null.");
            return null;
        }

        if (parent == null)
        {
            GameObject poolObj = new GameObject($"Pool_{typeof(T).Name}_{prefab.name}");
            parent = poolObj.transform;
        }

        return new ObjectPool<T>(prefab, initialSize, parent, onGet);
    }
}