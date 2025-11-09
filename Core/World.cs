using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperOtto.Core;

/// <summary>
/// Represents a tile in the game world.
/// </summary>
public enum TileType
{
    Grass,
    Dirt,
    TilledSoil,
    WateredSoil,
    Stone,
    Water
}

/// <summary>
/// Represents the game world with a tile-based grid.
/// </summary>
public class World
{
    public const int TileSize = 32;
    
    private TileType[,] _tiles;
    private Crop[,] _crops;
    
    public int Width { get; private set; }
    public int Height { get; private set; }

    public World(int width, int height)
    {
        Width = width;
        Height = height;
        _tiles = new TileType[width, height];
        _crops = new Crop[width, height];
        
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        var random = new Random(12345); // Fixed seed for reproducible world
        
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                // Create a simple world with mostly grass and some dirt
                double noise = random.NextDouble();
                
                if (noise < 0.7)
                    _tiles[x, y] = TileType.Grass;
                else if (noise < 0.85)
                    _tiles[x, y] = TileType.Dirt;
                else if (noise < 0.95)
                    _tiles[x, y] = TileType.Stone;
                else
                    _tiles[x, y] = TileType.Water;
            }
        }
    }

    public TileType GetTile(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return TileType.Stone;
        return _tiles[x, y];
    }

    public void SetTile(int x, int y, TileType type)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
            _tiles[x, y] = type;
    }

    public bool CanTill(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return false;
        
        var tile = _tiles[x, y];
        return tile == TileType.Grass || tile == TileType.Dirt;
    }

    public void TillSoil(int x, int y)
    {
        if (CanTill(x, y))
            _tiles[x, y] = TileType.TilledSoil;
    }

    public bool CanWater(int x, int y)
    {
        if (x < 0 || x >= Width || y >= 0 && y < Height)
            return _tiles[x, y] == TileType.TilledSoil;
        return false;
    }

    public void WaterSoil(int x, int y)
    {
        if (CanWater(x, y))
            _tiles[x, y] = TileType.WateredSoil;
    }

    public bool CanPlant(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return false;
        
        var tile = _tiles[x, y];
        return (tile == TileType.TilledSoil || tile == TileType.WateredSoil) && _crops[x, y] == null;
    }

    public void PlantCrop(int x, int y, CropType cropType)
    {
        if (CanPlant(x, y))
            _crops[x, y] = new Crop(cropType);
    }

    public Crop GetCrop(int x, int y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
            return _crops[x, y];
        return null;
    }

    public bool CanHarvest(int x, int y)
    {
        var crop = GetCrop(x, y);
        return crop != null && crop.IsReadyToHarvest;
    }

    public CropType? HarvestCrop(int x, int y)
    {
        if (CanHarvest(x, y))
        {
            var cropType = _crops[x, y].Type;
            _crops[x, y] = null;
            _tiles[x, y] = TileType.TilledSoil;
            return cropType;
        }
        return null;
    }

    public void Update(GameTime gameTime)
    {
        // Update all crops
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (_crops[x, y] != null)
                {
                    bool isWatered = _tiles[x, y] == TileType.WateredSoil;
                    _crops[x, y].Update(gameTime, isWatered);
                }
                
                // Reset watered soil to tilled at end of day (handled by time manager callback)
            }
        }
    }

    public void OnNewDay()
    {
        // Reset watered soil to tilled
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (_tiles[x, y] == TileType.WateredSoil)
                    _tiles[x, y] = TileType.TilledSoil;
            }
        }
    }
}

/// <summary>
/// Represents different types of crops.
/// </summary>
public enum CropType
{
    Wheat,
    Corn,
    Tomato,
    Carrot
}

/// <summary>
/// Represents a planted crop with growth stages.
/// </summary>
public class Crop
{
    private const float GrowthPerDay = 0.2f; // 5 days to full growth
    
    public CropType Type { get; private set; }
    public float Growth { get; private set; } = 0.0f;
    public bool IsReadyToHarvest => Growth >= 1.0f;

    public Crop(CropType type)
    {
        Type = type;
    }

    public void Update(GameTime gameTime, bool isWatered)
    {
        // Only grow if watered
        if (isWatered && Growth < 1.0f)
        {
            // This will be called multiple times per day, so we scale the growth
            Growth += GrowthPerDay * (float)gameTime.ElapsedGameTime.TotalSeconds / 86400f; // 86400 seconds in a day
            if (Growth > 1.0f)
                Growth = 1.0f;
        }
    }
}
