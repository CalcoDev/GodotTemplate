using Godot;
using Utils;

namespace Components.Logic;

public partial class TimerComponent : Node
{
    [Export] public bool UpdateSelf { get; set; } = false;
    [Export] public bool RemoveOnComplete { get; set; } = false;

    public float Time { get; private set; }

    public bool HasFinished => Calc.FloatEquals(Time, 0f);
    public bool IsRunning => Time > 0f;

    [Signal]
    public delegate void OnTimeoutEventHandler();
    [Signal]
    public delegate void OnQueueFreedEventHandler();

    private bool _triggeredEvent;

    public TimerComponent(float time, bool updateSelf, bool removeOnComplete)
    {
        Time = time;
        UpdateSelf = updateSelf;
        RemoveOnComplete = removeOnComplete;
    }

    public override void _Process(double delta)
    {
        if (UpdateSelf)
            Update((float)delta);
    }

    public void Update(float deltaTime)
    {
        Time = Mathf.Max(Time - deltaTime, 0f);
        if (HasFinished && !_triggeredEvent)
        {
            _triggeredEvent = true;
            EmitSignal(SignalName.OnTimeout);

            if (RemoveOnComplete)
            {
                EmitSignal(SignalName.OnQueueFreed);
                QueueFree();
            }
        }
    }

    public void Start(float time)
    {
        UpdateSelf = true;
        Time = time;
    }

    public void Pause()
    {
        UpdateSelf = false;
    }

    public void SetTime(float time)
    {
        _triggeredEvent = Calc.FloatEquals(time, 0f);
        Time = time;
    }
}