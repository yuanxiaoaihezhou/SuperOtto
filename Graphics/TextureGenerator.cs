using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperOtto.Graphics;

/// <summary>
/// Generates procedural textures for game assets.
/// This allows the game to run without external asset files.
/// TODO: Replace with actual sprite sheets when available.
/// </summary>
public static class TextureGenerator
{
    /// <summary>
    /// Creates a solid color texture.
    /// </summary>
    public static Texture2D CreateSolidTexture(GraphicsDevice graphicsDevice, int width, int height, Color color)
    {
        var texture = new Texture2D(graphicsDevice, width, height);
        var data = new Color[width * height];
        for (int i = 0; i < data.Length; i++)
            data[i] = color;
        texture.SetData(data);
        return texture;
    }

    /// <summary>
    /// Creates a textured tile (grass, dirt, etc.)
    /// </summary>
    public static Texture2D CreateTileTexture(GraphicsDevice graphicsDevice, int size, Color baseColor, Color accentColor)
    {
        var texture = new Texture2D(graphicsDevice, size, size);
        var data = new Color[size * size];
        var random = new Random(baseColor.GetHashCode());

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = y * size + x;
                // Add some random variation
                if (random.NextDouble() < 0.1)
                    data[index] = accentColor;
                else
                    data[index] = baseColor;
            }
        }

        texture.SetData(data);
        return texture;
    }

    /// <summary>
    /// Creates a bordered rectangle texture (for UI elements).
    /// </summary>
    public static Texture2D CreateBorderedRectangle(GraphicsDevice graphicsDevice, int width, int height, 
        Color fillColor, Color borderColor, int borderWidth = 2)
    {
        var texture = new Texture2D(graphicsDevice, width, height);
        var data = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                bool isBorder = x < borderWidth || x >= width - borderWidth || 
                               y < borderWidth || y >= height - borderWidth;
                data[index] = isBorder ? borderColor : fillColor;
            }
        }

        texture.SetData(data);
        return texture;
    }

    /// <summary>
    /// Creates a simple player character sprite.
    /// TODO: Replace with proper character sprite sheet.
    /// </summary>
    public static Texture2D CreatePlayerSprite(GraphicsDevice graphicsDevice, int size)
    {
        var texture = new Texture2D(graphicsDevice, size, size);
        var data = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = y * size + x;
                // Simple stick figure
                // Head
                if (y < size / 3 && x > size / 4 && x < 3 * size / 4)
                    data[index] = new Color(255, 220, 180); // Skin tone
                // Body
                else if (y >= size / 3 && y < 2 * size / 3 && x > size / 3 && x < 2 * size / 3)
                    data[index] = new Color(100, 100, 200); // Blue shirt
                // Legs
                else if (y >= 2 * size / 3 && (x < size / 2 - 2 || x > size / 2 + 2) && x > size / 4 && x < 3 * size / 4)
                    data[index] = new Color(50, 50, 100); // Blue pants
                else
                    data[index] = Color.Transparent;
            }
        }

        texture.SetData(data);
        return texture;
    }

    /// <summary>
    /// Creates a crop sprite at different growth stages.
    /// TODO: Replace with actual crop sprites.
    /// </summary>
    public static Texture2D CreateCropSprite(GraphicsDevice graphicsDevice, int size, float growthStage)
    {
        var texture = new Texture2D(graphicsDevice, size, size);
        var data = new Color[size * size];

        int stemHeight = (int)(size * growthStage * 0.8f);
        int leafSize = (int)(size * growthStage * 0.3f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = y * size + x;
                // Stem
                if (x == size / 2 && y >= size - stemHeight)
                    data[index] = new Color(100, 200, 100);
                // Leaves (simple triangle shape)
                else if (growthStage > 0.3f && y >= size - stemHeight && y < size - stemHeight / 2 && 
                         Math.Abs(x - size / 2) < leafSize)
                    data[index] = new Color(50, 180, 50);
                else
                    data[index] = Color.Transparent;
            }
        }

        texture.SetData(data);
        return texture;
    }

    /// <summary>
    /// Creates an item icon texture.
    /// TODO: Replace with actual item icons.
    /// </summary>
    public static Texture2D CreateItemIcon(GraphicsDevice graphicsDevice, int size, Color itemColor)
    {
        var texture = new Texture2D(graphicsDevice, size, size);
        var data = new Color[size * size];

        int padding = size / 4;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = y * size + x;
                if (x >= padding && x < size - padding && y >= padding && y < size - padding)
                {
                    data[index] = itemColor;
                }
                else
                {
                    data[index] = Color.Transparent;
                }
            }
        }

        texture.SetData(data);
        return texture;
    }
}
