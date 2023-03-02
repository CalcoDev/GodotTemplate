using System.Collections;
using System.Collections.Generic;
using Game.Components.Logic.Coroutines;
using Godot;

namespace Game.Managers;

public partial class CoroutineManager : Node
{
    private static readonly List<CoroutineComponent> ActiveCoroutines = new List<CoroutineComponent>();
    private static CoroutineManager _instance;

    public override void _EnterTree()
    {
        _instance = this;
    }

    public static CoroutineComponent StartCoroutine(IEnumerator function, bool removeOnComplete = true,
        bool updateSelf = false)
    {
        CoroutineComponent c = new CoroutineComponent(function, removeOnComplete, updateSelf);
        _instance.AddChild(c);

        ActiveCoroutines.Add(c);
        return c;
    }

    public static void StopCoroutine(CoroutineComponent coroutine, bool markAsFinished = false)
    {
        if (markAsFinished)
            coroutine.Finish();

        ActiveCoroutines.Remove(coroutine);
    }
}