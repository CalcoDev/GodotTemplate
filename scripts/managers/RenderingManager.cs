using System.Collections.Generic;
using Components.Logic;
using Godot;

namespace Managers;

// TODO(calco): I am uncertain whether this works with Cameras. I do not think it does, but I don't have any test cases for now.
public partial class RenderingManager : Node
{
    private struct PixelatedRenderingLayer
    {
        public SubViewport SubViewport { get; set; }
        public TextureRect TextureRect { get; set; }
    }

    public static RenderingManager Instance { get; private set; }
    private readonly Dictionary<float, PixelatedRenderingLayer> _layers = new Dictionary<float, PixelatedRenderingLayer>();

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
            Instance = this;
    }

    public override void _Ready()
    {
        SetUpViewports();
    }

    /// <summary>
    /// Adds [<paramref name="node"/>] to the layer with the same ZIndex as itself. If the layer does not exist, it will be created.
    /// </summary>
    /// <param name="node">The node to add</param>
    public void TryAddNodeToLayer(Node2D node)
    {
        if (!_layers.ContainsKey(node.ZIndex))
            CreateLayer(node.ZIndex);

        node.GetParent().RemoveChild(node);
        _layers[node.ZIndex].SubViewport.AddChild(node);
    }

    public void SetUpViewports()
    {
        foreach (Node node in GetTree().GetNodesInGroup("pixelated"))
        {
            if (node is not Node2D node2d)
                continue;

            if (NodeIsEligibleForFollower(node2d))
                AddFollowerToNode(node2d);

            CallDeferred(nameof(TryAddNodeToLayer), node2d);
        }
    }

    private void CreateLayer(int zIndex)
    {
        SubViewport subViewport = new SubViewport
        {
            Size = new Vector2I(320, 180),
            TransparentBg = true,
            Disable3D = true,
            Snap2DTransformsToPixel = true,
            Snap2DVerticesToPixel = true
        };

        TextureRect textureRect = new TextureRect
        {
            Texture = subViewport.GetTexture(),
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            ZAsRelative = false,
            ZIndex = zIndex
        };

        PixelatedRenderingLayer layer = new PixelatedRenderingLayer()
        {
            SubViewport = subViewport,
            TextureRect = textureRect
        };
        _layers.Add(zIndex, layer);

        Node layerRoot = new Node
        {
            Name = $"Layer {zIndex}"
        };

        layerRoot.AddChild(subViewport);
        layerRoot.AddChild(textureRect);
        AddChild(layerRoot);
    }

    private static bool NodeIsEligibleForFollower(Node2D node)
    {
        return node.GetParent() is Node2D && node.GetParent().Name != "root" && !node.GetParent().Name.ToString().StartsWith("Scene");
    }

    private static void AddFollowerToNode(Node2D node)
    {
        FollowerComponent followerComp = new FollowerComponent(node, node.GetParent<Node2D>(), true);
        node.AddChild(followerComp);
    }
}
