using Godot;

namespace Arphros;

[Tool]
public partial class CameraPivot : Node3D
{
	[Export] public Node3D Target { get; set; }
	[Export] public Vector3 PivotRotation { get; set; } = new Vector3(45f, 45f, 0);
	[Export] public Vector3 PivotOffset { get; set; } = new Vector3(5, 0, 5);
    [Export] public Vector3 InternalOffset { get; set; } = new Vector3(0, 0, 0);
    [Export] public Vector3 InternalRotation { get; set; } = new Vector3(0, 0, 0);
    [Export] public float Distance { get; set; } = 40f;
	[Export] public float LerpSpeed { get; set; } = 0.02f;

    private Camera3D _camera;

	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera3D");
	}

	public override void _Process(double delta)
	{
		if (_camera == null || Target == null) return;

		if (!Engine.IsEditorHint())
			Position = Position.Lerp(Target.Position + PivotOffset.ConvertGUniPosition(), (float)(LerpSpeed * delta));
		else
			Position = Target.Position + PivotOffset.ConvertGUniPosition();

		RotationDegrees = PivotRotation.ConvertGUniEuler();

		_camera.Position = InternalOffset.ConvertGUniPosition() + new Vector3(0, 0, Distance);
		_camera.Rotation = InternalRotation.ConvertGUniEuler();
	}
}