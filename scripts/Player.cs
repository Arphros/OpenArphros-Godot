using Godot;
using System.Collections.Generic;

namespace Arphros;

[Tool]
public partial class Player : CharacterBody3D
{
    [Signal]
    public delegate void OnLineStartEventHandler();
    [Signal]
    public delegate void OnLineResetEventHandler();

    // Nodes
    [ExportGroup("Nodes")]
    [Export] public AudioStreamPlayer Source { get; set; }
    [Export] public AnimationPlayer AnimationPlayer { get; set; }
    [Export] public StringName AnimationName { get; set; }
    [Export] public Node3D Camera { get; set; }
    [Export] public Node3D TailParent { get; set; }
    [Export] public PackedScene Tail { get; set; }
    [Export] public PackedScene DiePiece { get; set; }

    // Properties
    private Color _color;
    [ExportGroup("Properties")]
    [Export]
    public Color Color
    {
        get => GetColor();
        set => SetColor(value);
    }

    [Export]
    public Godot.Collections.Array<Vector3> Rotations { get; set; } = new(){ new Vector3(0, 0, 0), new Vector3(0, 90, 0) };
    [Export]
    public int RotationIndex { get; set; } = 0;
    [Export]
    public float Speed { get; set; } = 12.0f;

    private MeshInstance3D _meshInstance;

    private List<Node3D> _tails = new();
    private Node3D _currentTail;

    private bool _isStarted = false;
    private bool _isAlive = true;
    private bool _isGrounded = true;
    private bool _wasFlying = false;
    private bool _isHoveringUI = false;

    private float _customVelocity = 0;

    public override void _Ready()
    {
        if (_meshInstance == null)
        {
            _meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
        }
    }

    public override void _Process(double delta)
    {
        if (!Engine.IsEditorHint())
        {
            _isGrounded = IsGrounded();
            if (_isAlive)
            {
                if (_isGrounded)
                {
                    if (Input.IsActionJustPressed("line_turn_key") || (Input.IsActionJustPressed("line_turn_mouse") && !_isHoveringUI))
                    {
                        TurnLine();
                    }
                }

                if (_isStarted)
                {
                    TranslateLine((float)delta);
                    if (_isGrounded)
                    {
                        TranslateTail((float)delta);
                        if (_wasFlying)
                        {
                            CreateTail();
                            _wasFlying = false;
                        }
                    }
                    else
                    {
                        _wasFlying = true;
                    }
                }
            }

            if (Input.IsActionJustPressed("game_restart"))
            {
                GetTree().ReloadCurrentScene();
            }
        }
    }

    Vector3 customVelocity = new();
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (Engine.IsEditorHint()) return;

        if (!IsGrounded())
            customVelocity += GetGravity() * (float)delta;
        else
            customVelocity = new Vector3();

        Velocity = customVelocity;
        MoveAndSlide();
    }

    private bool IsGrounded()
    {
        var spaceState = GetWorld3D().DirectSpaceState;
        var start = Transform.Origin;
        var end = start + Vector3.Down * 0.53f;
        var query = new PhysicsRayQueryParameters3D
        {
            From = start,
            To = end,
            CollideWithAreas = true
        };

        var result = spaceState.IntersectRay(query);
        return result.Count > 0 && result.ContainsKey("rid");
    }

    private void TurnLine()
    {
        if (_isStarted)
        {
            RotationIndex += 1;
            if (RotationIndex >= Rotations.Count)
            {
                RotationIndex = 0;
            }
        }
        else
        {
            RotationIndex = 0;
            Source.Play();
            if (AnimationPlayer != null)
                AnimationPlayer.Play(AnimationName);
            EmitSignal(SignalName.OnLineStart);
            _isStarted = true;
        }

        RotateBase();
        CreateTail();
    }

    private void RotateBase()
    {
        RotationDegrees = Rotations[RotationIndex].ConvertGUniEuler();
    }

    private void CreateTail()
    {
        var instance = Tail.Instantiate<Node3D>();
        instance.Position = Position;
        instance.Transform = Transform;
        _currentTail = instance;
        _tails.Add(instance);
        if (TailParent != null)
            TailParent.AddChild(instance);
    }

    private void TranslateLine(float delta)
    {
        Position += Transform.Basis.Z * delta * -Speed;
    }

    private void TranslateTail(float delta)
    {
        if (_currentTail != null)
        {
            var tailDelta = new Vector3(0, 0, delta * Speed);
            _currentTail.Position += Transform.Basis.Z / 2 * delta * -Speed;
            _currentTail.Scale = _currentTail.Scale + tailDelta;
        }
    }

    private void SetColor(Color value)
    {
        if (_meshInstance != null)
        {
            _meshInstance.GetActiveMaterial(0).Set("albedo_color", value);
        }
    }

    private Color GetColor()
    {
        if (_meshInstance != null)
        {
            return (Color)_meshInstance.GetActiveMaterial(0).Get("albedo_color");
        }
        return new Color(0, 0, 0);
    }

    private void OnObstacleFinderBodyShapeEntered(Rid bodyRid, Node3D body, int bodyShapeIndex, int localShapeIndex)
    {
        if (body != null)
        {
            GD.Print(body.Name);
            if (body.Name.ToString().StartsWith("Obstacle_"))
            {
                Die();
            }
        }
    }

    private void Die()
    {
        if (_isAlive)
        {
            for (int i = 0; i < 3; i++)
            {
                CreateDiePiece(RandOnUnitCircle());
            }
            _isAlive = false;
        }
    }

    private void CreateDiePiece(Vector3 offset)
    {
        var instance = DiePiece.Instantiate<RigidBody3D>();
        instance.Position = Position + offset;
        instance.Transform = Transform;
        _tails.Add(instance);
        TailParent.AddChild(instance);
    }

    private Vector3 RandOnUnitCircle()
    {
        return new Vector3(
            (float)GD.RandRange(-1.0f, 1.0f),
            (float)GD.RandRange(0.0f, 1.0f),
            (float)GD.RandRange(-1.0f, 1.0f)
        );
    }

    // UI handlers
    private void _on_MainGameUIMouseEntered()
    {
        _isHoveringUI = false;
    }

    private void _on_MainGameUIMouseExited()
    {
        _isHoveringUI = true;
    }
}
