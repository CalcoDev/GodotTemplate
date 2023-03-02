using System;
using System.Collections;
using Game.Components.Logic.Coroutines;
using Godot;

namespace Components.Logic;

public partial class StateMachineComponent : Node
{
    [Export] public bool UpdateSelf { get; set; } = false;

    [ExportGroup("Debug")]
    [Export] private int _state;
    [Export] private int _previousState;

    public int State => _state;

    private int _stateCount;

    private readonly Action[] _enters;
    private readonly Func<int>[] _updates;
    private readonly Action[] _exits;

    private readonly Func<IEnumerator>[] _coroutines;
    private readonly CoroutineComponent _currentCoroutine;

    public StateMachineComponent(int maxStates, int defaultState)
    {
        _stateCount = maxStates;

        _enters = new Action[maxStates];
        _updates = new Func<int>[maxStates];
        _exits = new Action[maxStates];

        _coroutines = new Func<IEnumerator>[maxStates];
        _currentCoroutine = new CoroutineComponent(null, false, false);
        this.AddChild(_currentCoroutine);

        _previousState = _state = defaultState;
    }

    // Added so that editor can spawn this component, without running into issues.
    public StateMachineComponent()
    {
        _stateCount = 0;

        _enters = Array.Empty<Action>();
        _updates = Array.Empty<Func<int>>();
        _exits = Array.Empty<Action>();

        _coroutines = Array.Empty<Func<IEnumerator>>();
        _currentCoroutine = new CoroutineComponent(null, false, false);
        this.AddChild(_currentCoroutine);

        _previousState = _state = -1;
    }

    public override void _Process(double delta)
    {
        if (UpdateSelf)
        {
            int newState = Update();
            SetState(newState);
        }

        _currentCoroutine?.Update((float)delta);
    }

    public void SetState(int state)
    {
        if (_state == state)
            return;

        if (state < 0 || state >= _stateCount)
            throw new ArgumentOutOfRangeException(nameof(state), "StateMachineComponent: State out of range.");

        _previousState = _state;
        _state = state;

        if (_previousState != -1 && _exits[_previousState] != null)
            _exits[_previousState].Invoke();

        _enters[_state]?.Invoke();

        if (_coroutines[_state] != null)
        {
            if (!_currentCoroutine.Finished)
                _currentCoroutine.Cancel();

            _currentCoroutine.Replace(_coroutines[_state].Invoke());
        }
    }

    public int Update()
    {
        if (_updates[_state] != null)
            return _updates[_state].Invoke();

        return _state;
    }

    public void SetCallbacks(int index, Func<int> update, Action enter = null, Action exit = null, Func<IEnumerator> coroutine = null)
    {
        _enters[index] = enter;
        _updates[index] = update;
        _exits[index] = exit;

        _coroutines[index] = coroutine;
    }
}