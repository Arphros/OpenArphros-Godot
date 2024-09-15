using Godot;
using Godot.Collections;

namespace Arphros;

public partial class References : Node3D
{
	public static Dictionary<string, Node> Source = new();

	[Export] public Array<string> Names = new();
	[Export] public Array<Node> Scripts = new();

	bool isAssigned = false;

    public override bool _Set(StringName property, Variant value)
    {
        AssignAll();
        return base._Set(property, value);
    }

	public void AssignAll()
    {
		if (isAssigned) return;

        for (int i = 0; i < Scripts.Count; i++)
            Source.Add(Names[i], Scripts[i]);
		isAssigned = true;
    }
	public void ResetAll() => Source.Clear();

	public static T Get<T>(string name) where T : Node
	{
		var node = Source[name];
		return node != null ? node as T : null;
	}
}
