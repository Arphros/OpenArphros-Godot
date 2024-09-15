using Godot;
using System;
using System.IO;

namespace Arphros;

public partial class LevelManager : Node3D
{
	[Export] public string DirectoryPath { get; set; }
	[Export] public string FilePath { get; set; }

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void OpenProject(string directoryPath)
	{
		if (Directory.Exists(directoryPath))
		{
		}
	}
}
