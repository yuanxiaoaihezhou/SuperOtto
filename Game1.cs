using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperOtto.Core;
using SuperOtto.Systems;
using SuperOtto.UI;

namespace SuperOtto;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    // Core game systems
    private World _world;
    private Player _player;
    private TimeManager _timeManager;
    private Camera _camera;
    private Inventory _inventory;
    private HUD _hud;
    
    // Textures
    private Dictionary<TileType, Texture2D> _tileTextures;
    private Texture2D _playerTexture;
    private SpriteFont _font;
    
    // Input
    private KeyboardState _previousKeyState;
    private int _previousDay;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        // Set window size
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
    }

    protected override void Initialize()
    {
        // Initialize game systems
        _world = new World(50, 50);
        _player = new Player(new Vector2(400, 400));
        _timeManager = new TimeManager();
        _camera = new Camera(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        _inventory = new Inventory();
        _hud = new HUD();
        
        _previousDay = _timeManager.CurrentDay;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Generate default font texture (simple bitmap font)
        _font = CreateDefaultFont();
        
        // Generate tile textures
        _tileTextures = new Dictionary<TileType, Texture2D>
        {
            [TileType.Grass] = Graphics.TextureGenerator.CreateTileTexture(GraphicsDevice, 32, new Color(100, 180, 100), new Color(80, 160, 80)),
            [TileType.Dirt] = Graphics.TextureGenerator.CreateTileTexture(GraphicsDevice, 32, new Color(139, 90, 43), new Color(120, 75, 35)),
            [TileType.TilledSoil] = Graphics.TextureGenerator.CreateTileTexture(GraphicsDevice, 32, new Color(100, 60, 30), new Color(90, 50, 25)),
            [TileType.WateredSoil] = Graphics.TextureGenerator.CreateTileTexture(GraphicsDevice, 32, new Color(70, 50, 30), new Color(60, 40, 25)),
            [TileType.Stone] = Graphics.TextureGenerator.CreateSolidTexture(GraphicsDevice, 32, 32, Color.Gray),
            [TileType.Water] = Graphics.TextureGenerator.CreateSolidTexture(GraphicsDevice, 32, 32, new Color(100, 150, 255))
        };
        
        // Generate player texture
        _playerTexture = Graphics.TextureGenerator.CreatePlayerSprite(GraphicsDevice, 32);
        
        // Load HUD
        _hud.LoadContent(GraphicsDevice, _font);
    }

    private SpriteFont CreateDefaultFont()
    {
        // Create a simple default font since we don't have Content files yet
        // This is a workaround - in a real game you'd use a .spritefont file
        // For now, we'll return null and handle it in drawing
        return null;
    }

    protected override void Update(GameTime gameTime)
    {
        var keyState = Keyboard.GetState();
        
        if (keyState.IsKeyDown(Keys.Escape))
            Exit();

        // Update player
        _player.Update(gameTime);
        
        // Update world
        _world.Update(gameTime);
        
        // Update time
        _timeManager.Update(gameTime);
        
        // Check for new day
        if (_timeManager.CurrentDay != _previousDay)
        {
            _world.OnNewDay();
            _previousDay = _timeManager.CurrentDay;
        }
        
        // Update camera to follow player
        _camera.Follow(_player.Position);
        
        // Handle inventory selection
        HandleInventoryInput(keyState);
        
        // Handle farming actions
        HandleFarmingActions(keyState);

        _previousKeyState = keyState;
        base.Update(gameTime);
    }

    private void HandleInventoryInput(KeyboardState keyState)
    {
        // Number keys to select inventory slots
        for (int i = 0; i < 10; i++)
        {
            Keys key = Keys.D0 + i; // D0 = 0, D1 = 1, etc.
            if (keyState.IsKeyDown(key) && !_previousKeyState.IsKeyDown(key))
            {
                _inventory.SelectedSlot = i == 0 ? 9 : i - 1; // 0 key = slot 9
            }
        }
    }

    private void HandleFarmingActions(KeyboardState keyState)
    {
        var tilePos = _player.GetFacingTilePosition();
        var selectedItem = _inventory.GetSelectedItem();
        
        // Space bar to use tool/plant
        if (keyState.IsKeyDown(Keys.Space) && !_previousKeyState.IsKeyDown(Keys.Space))
        {
            if (selectedItem != null)
            {
                const float toolEnergyCost = 5f; // Energy cost for tool usage
                
                switch (selectedItem.Name)
                {
                    case "Hoe":
                        if (_player.ConsumeEnergy(toolEnergyCost))
                        {
                            _world.TillSoil(tilePos.X, tilePos.Y);
                        }
                        break;
                    
                    case "Watering Can":
                        if (_player.ConsumeEnergy(toolEnergyCost))
                        {
                            _world.WaterSoil(tilePos.X, tilePos.Y);
                        }
                        break;
                    
                    case "Wheat Seeds":
                        if (_player.ConsumeEnergy(toolEnergyCost) && 
                            _world.CanPlant(tilePos.X, tilePos.Y) && 
                            _inventory.RemoveItem("Wheat Seeds", 1))
                        {
                            _world.PlantCrop(tilePos.X, tilePos.Y, CropType.Wheat);
                        }
                        break;
                    
                    case "Corn Seeds":
                        if (_player.ConsumeEnergy(toolEnergyCost) && 
                            _world.CanPlant(tilePos.X, tilePos.Y) && 
                            _inventory.RemoveItem("Corn Seeds", 1))
                        {
                            _world.PlantCrop(tilePos.X, tilePos.Y, CropType.Corn);
                        }
                        break;
                }
            }
        }
        
        // E key to harvest
        if (keyState.IsKeyDown(Keys.E) && !_previousKeyState.IsKeyDown(Keys.E))
        {
            const float harvestEnergyCost = 3f; // Energy cost for harvesting
            
            if (_player.ConsumeEnergy(harvestEnergyCost))
            {
                var cropType = _world.HarvestCrop(tilePos.X, tilePos.Y);
                if (cropType.HasValue)
                {
                    string cropName = cropType.Value.ToString();
                    _inventory.AddItem(new Item(cropName, ItemType.Crop, 1));
                }
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(135, 206, 235)); // Sky blue

        // Draw world
        _spriteBatch.Begin(transformMatrix: _camera.GetTransformMatrix(), samplerState: SamplerState.PointClamp);
        DrawWorld();
        DrawPlayer();
        _spriteBatch.End();
        
        // Draw UI (no camera transform)
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        if (_font != null)
        {
            _hud.Draw(_spriteBatch, _timeManager, _inventory, _player, GraphicsDevice, 
                _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        }
        else
        {
            // Fallback if font isn't loaded
            DrawSimpleHUD();
        }
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawWorld()
    {
        // Calculate visible tile range
        int startX = Math.Max(0, (int)(_camera.Position.X / World.TileSize) - 1);
        int startY = Math.Max(0, (int)(_camera.Position.Y / World.TileSize) - 1);
        int endX = Math.Min(_world.Width, startX + (_graphics.PreferredBackBufferWidth / World.TileSize) + 2);
        int endY = Math.Min(_world.Height, startY + (_graphics.PreferredBackBufferHeight / World.TileSize) + 2);

        // Draw tiles
        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                var tileType = _world.GetTile(x, y);
                var texture = _tileTextures[tileType];
                var position = new Vector2(x * World.TileSize, y * World.TileSize);
                
                // Apply day/night lighting
                float lightIntensity = _timeManager.GetLightIntensity();
                Color tint = new Color(lightIntensity, lightIntensity, lightIntensity);
                
                _spriteBatch.Draw(texture, position, tint);
                
                // Draw crop if present
                var crop = _world.GetCrop(x, y);
                if (crop != null)
                {
                    var cropTexture = Graphics.TextureGenerator.CreateCropSprite(GraphicsDevice, 32, crop.Growth);
                    _spriteBatch.Draw(cropTexture, position, tint);
                }
            }
        }
    }

    private void DrawPlayer()
    {
        float lightIntensity = _timeManager.GetLightIntensity();
        Color tint = new Color(lightIntensity, lightIntensity, lightIntensity);
        _spriteBatch.Draw(_playerTexture, _player.Position, tint);
    }

    private void DrawSimpleHUD()
    {
        // Simple fallback HUD without font
        var hudBg = Graphics.TextureGenerator.CreateSolidTexture(GraphicsDevice, 200, 100, new Color(0, 0, 0, 150));
        _spriteBatch.Draw(hudBg, new Vector2(10, 10), Color.White);
    }
}
