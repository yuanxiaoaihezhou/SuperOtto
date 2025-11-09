using Microsoft.Xna.Framework;

namespace SuperOtto.Systems;

/// <summary>
/// Camera system for following the player and viewing the world.
/// </summary>
public class Camera
{
    public Vector2 Position { get; set; }
    public float Zoom { get; set; } = 1.0f;
    
    private readonly int _viewportWidth;
    private readonly int _viewportHeight;

    public Camera(int viewportWidth, int viewportHeight)
    {
        _viewportWidth = viewportWidth;
        _viewportHeight = viewportHeight;
    }

    public void Follow(Vector2 targetPosition)
    {
        Position = targetPosition - new Vector2(_viewportWidth / 2f / Zoom, _viewportHeight / 2f / Zoom);
    }

    public Matrix GetTransformMatrix()
    {
        return Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
               Matrix.CreateScale(Zoom, Zoom, 1);
    }

    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        return Vector2.Transform(screenPosition, Matrix.Invert(GetTransformMatrix()));
    }
}
