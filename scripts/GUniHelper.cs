using Godot;

namespace Arphros;

/// <summary>
/// This class is used to convert between Godot and Unity coordinates.
/// </summary>
public static class GUniHelper
{
    /// <summary>
    /// Inverts the Z axis.
    /// </summary>
    public static Vector3 ConvertGUniPosition(this Vector3 position) => new(position.X, position.Y, -position.Z);

    /// <summary>
    /// Inverting the Yaw (Y axis rotation) and Roll (Z axis rotation)
    /// </summary>
    public static Vector3 ConvertGUniEuler(this Vector3 eulerAngles) => new(-eulerAngles.X, -eulerAngles.Y, -eulerAngles.Z);
}