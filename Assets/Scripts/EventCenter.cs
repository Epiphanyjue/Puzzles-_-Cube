using System;
using System.Collections.Generic;
using UnityEngine;

// 事件中心类
public class EventCenter : MonoBehaviour
{
    // 单例模式
    private static EventCenter instance;
    public static EventCenter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("EventCenter").AddComponent<EventCenter>();
                DontDestroyOnLoad(instance.gameObject); // 确保在场景切换时不会被销毁
            }
            return instance;
        }
    }

    // 在Awake中实例化单例
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 确保在场景切换时不会被销毁
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 如果已经有一个实例，销毁这个实例
        }
    }

    // 处理没有入参的事件
    #region

    private static Dictionary<string, Action> eventTable = new Dictionary<string, Action>();

    // 订阅事件
    public void Subscribe(string eventType, Action listener)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] += listener;
        }
        else
        {
            eventTable[eventType] = listener;
        }
    }

    // 取消订阅事件
    public void Unsubscribe(string eventType, Action listener)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] -= listener;
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }
    }

    // 触发事件
    public void TriggerEvent(string eventType)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType]?.Invoke();
        }
    }

    #endregion

    // 处理有入参的事件
    #region

    private static Dictionary<string, Delegate> delegateTable = new Dictionary<string, Delegate>();

    public void Subscribe<T>(string eventType, Action<T> listener)
    {
        if (delegateTable.ContainsKey(eventType))
        {
            delegateTable[eventType] = Delegate.Combine(delegateTable[eventType], listener);
        }
        else
        {
            delegateTable[eventType] = listener;
        }
    }

    public void Unsubscribe<T>(string eventType, Action<T> listener)
    {
        if (delegateTable.ContainsKey(eventType))
        {
            delegateTable[eventType] = Delegate.Remove(delegateTable[eventType], listener);
            if (delegateTable[eventType] == null)
            {
                delegateTable.Remove(eventType);
            }
        }
    }

    public void TriggerEvent<T>(string eventType, T param)
    {
        if (delegateTable.ContainsKey(eventType))
        {
            Action<T> action = delegateTable[eventType] as Action<T>;
            if (action != null)
            {
                action.Invoke(param);
            }
        }
    }

    #endregion

    private void OnDestroy()
    {
        // 确保在场景销毁时不会尝试销毁自己
        if (instance == this)
        {
            instance = null;
        }
    }
}