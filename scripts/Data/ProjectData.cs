using Godot;
using System;
using System.Collections.Generic;

namespace Arphros.Data;

[Serializable]
public class ProjectData
{
    public LevelInfo info = new LevelInfo();

    public ObjectData player = new ObjectData();
    public ObjectData directionalLight = new ObjectData();
    public ObjectData mainCamera = new ObjectData();

    public List<MeshData> meshes = new List<MeshData>();
    public List<MaterialData> materials = new List<MaterialData>();
    public List<SpriteData> sprites = new List<SpriteData>();
    public List<ObjectData> objects = new List<ObjectData>();
}

[Serializable]
public class LevelInfo
{
    public int id;

    public string levelName = "Untitled";
    public string description;
    public string difficulty = "Auto";
    public string theme = "Other";
    public string genre = "Other";

    public string musicName = "Untitled";
    public string musicAuthor = "Unknown";

    public string gameVersion;
    public int levelVersion;
    public string editedTime;

    public int finishedLevelVersion;
    public bool isFinished;

    public CamType cameraType = CamType.StableCamera;
    public EnvironmentData environment = new EnvironmentData();
}

[Serializable]
public class EnvironmentData
{
    public SkyboxType skybox = SkyboxType.Color;
    public Color backgroundColor = new(1f, 1f, 1f);
    public bool enableFog;
    public float fogDensity = 0.01f;
    public Color fogColor = new(1f, 1f, 1f);
    public Color ambientColor = new(1f, 1f, 1f);

    public void Apply()
    {
        /*LevelManager.Instance.ChangeSkybox(skybox);
        LevelManager.Instance.ChangeBackgroundColor(backgroundColor);
        References.Editor.ChangeFogState(enableFog);
        RenderSettings.fogDensity = fogDensity;
        RenderSettings.fogColor = fogColor;
        RenderSettings.ambientLight = ambientColor;*/
    }

    public static EnvironmentData Get()
    {
        /*var env = LevelManager.Instance.info.environment;
        var data = new EnvironmentData()
        {
            skybox = env.skybox,
            backgroundColor = LevelManager.Instance.GetBackgroundColor(),
            enableFog = env.enableFog,
            fogDensity = RenderSettings.fogDensity,
            fogColor = RenderSettings.fogColor,
            ambientColor = RenderSettings.ambientLight
        };
        return data;*/
        return null;
    }
}

[Serializable]
public class ObjectData
{
    public int id;
    public int parentId = -1;

    public ObjectType type = ObjectType.Model;
    public SpaceType spaceType = SpaceType.World;

    public string name = "Object";

    public Vector3 position;
    public Vector3 eulerAngles;
    public Vector3 scale = Vector3.One;

    public List<int> groupId = new();

    public ObstacleType obstacleType = ObstacleType.None;
    public VisibilityType visibility = VisibilityType.Shown;

    public string customData;
}

[Serializable]
public class MeshData
{
    public int id;
    public string fileName;
}

[Serializable]
public class MaterialData
{
    public int id;
    public string name;
    public int type;

    public Color color = new(1f, 1f, 1f);

    public float metallic;
    public float smoothness;

    public bool emission;
    public Color emissionColor = new(1f, 1f, 1f);

    // public string textureFile;
    public bool specularHighlights = true;
    public bool reflections = true;
}

public enum MaterialType
{
    Opaque = 0,
    Cutout = 1,
    Fade = 2,
    Transparent = 3
}

[Serializable]
public class SpriteData
{
    public int id;
    public string fileName;
}

public enum CamType
{
    StableCamera,
    WeirdCamera,
    OldCamera
}

public enum SkyboxType
{
    Color,
    UnitySkybox
}

public enum SpaceType
{
    World,
    Screen
}

public enum ObstacleType
{
    None,
    Wall,
    PassThrough,
    Water
}

public enum VisibilityType
{
    Shown,
    Hidden,
    Gone
}

public enum ObjectType
{
    Primitive,
    Model,
    Sprite,
    Light,
    Trigger,
    Road,
    Particle,
    Player,
    MainCamera,
    Empty,
    Text,
    Tail,
    StartPos,
    Unspecified = 256
}