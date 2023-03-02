using Godot;

namespace Components.Logic;

public partial class FollowerComponent : Node
{
    [Export] public Node2D Follower { get; set; }
    [Export] public Node2D Followee { get; set; }
    [Export] public bool UpdateSelf { get; set; }

    public FollowerComponent(Node2D follower, Node2D followee, bool updateSelf)
    {
        Follower = follower;
        Followee = followee;
        UpdateSelf = updateSelf;
    }

    // Added so that editor can spawn this component, without running into issues.
    public FollowerComponent()
    {
        Follower = null;
        Followee = null;
        UpdateSelf = false;
    }

    public override void _Process(double delta)
    {
        if (UpdateSelf)
            Update();
    }

    public void Update()
    {
        Follower.GlobalPosition = Followee.GlobalPosition;
    }
}