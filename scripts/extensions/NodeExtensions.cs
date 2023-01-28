using System.Collections;
using System.Collections.Generic;
using Components.Logic;
using Godot;
using Managers;

namespace Extensions;

public static class NodeExtensions
{
    #region Coroutines

    private static readonly List<CoroutineComponent> ActiveCoroutines = new List<CoroutineComponent>();

    public static CoroutineComponent StartCoroutine(IEnumerator function, bool removeOnComplete = true,
        bool updateSelf = false)
    {
        CoroutineComponent c = new CoroutineComponent(function, removeOnComplete, updateSelf);
        GameManager.Root.AddChild(c);

        ActiveCoroutines.Add(c);
        return c;
    }

    public static void StopCoroutine(CoroutineComponent coroutine, bool markAsFinished = false)
    {
        if (markAsFinished)
            coroutine.Finish();

        ActiveCoroutines.Remove(coroutine);
    }

    #endregion

    #region Timers

    private static readonly List<TimerComponent> ActiveTimers = new List<TimerComponent>();

    public static TimerComponent StartTimr(float time, bool removeOnComplete = true,
        bool updateSelf = false)
    {
        TimerComponent t = new TimerComponent(time, updateSelf, removeOnComplete);
        GameManager.Root.AddChild(t);

        if (t.RemoveOnComplete)
            t.OnQueueFreed += () => ActiveTimers.Remove(t);

        return t;
    }

    public static void StopTimer(TimerComponent timer, bool markAsFinished = false)
    {
        if (markAsFinished)
            timer.SetTime(0f);

        ActiveTimers.Remove(timer);
    }

    #endregion
}