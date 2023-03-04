using Godot;

namespace Game.Managers;

public partial class SceneManager : Node
{
    private static SceneManager _instance;

    public static Node2D SceneRoot { get; private set; }
    // TODO(calco): Transitioning lmao

    public override void _EnterTree()
    {
        _instance = this;
    }

    public static void LoadScene(PackedScene scene)
    {
        if (SceneRoot != null)
            _instance.RemoveChild(SceneRoot);

        SceneRoot = scene.Instantiate<Node2D>();
        _instance.AddChild(SceneRoot);
    }

    public static void LoadScene(Node2D node)
    {
        if (SceneRoot != null)
            _instance.RemoveChild(SceneRoot);

        SceneRoot = node;
        _instance.AddChild(SceneRoot);
    }

    public static void SetScene
}