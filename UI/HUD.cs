using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SuperOtto.Core;
using SuperOtto.Systems;

namespace SuperOtto.UI;

/// <summary>
/// Renders the game's heads-up display.
/// </summary>
public class HUD
{
    private SpriteFont _font;
    private Texture2D _panelTexture;
    private Texture2D _slotTexture;

    public void LoadContent(GraphicsDevice graphicsDevice, SpriteFont font)
    {
        _font = font;
        _panelTexture = Graphics.TextureGenerator.CreateBorderedRectangle(
            graphicsDevice, 300, 150, new Color(40, 40, 40, 200), Color.White, 2);
        _slotTexture = Graphics.TextureGenerator.CreateBorderedRectangle(
            graphicsDevice, 40, 40, new Color(60, 60, 60, 200), Color.White, 2);
    }

    public void Draw(SpriteBatch spriteBatch, TimeManager timeManager, Inventory inventory, 
        Player player, GraphicsDevice graphicsDevice, int screenWidth, int screenHeight)
    {
        // Draw time panel
        DrawTimePanel(spriteBatch, timeManager, player, 10, 10);

        // Draw inventory hotbar
        DrawInventoryHotbar(spriteBatch, inventory, graphicsDevice, 
            screenWidth / 2 - 250, screenHeight - 60);
    }

    private void DrawTimePanel(SpriteBatch spriteBatch, TimeManager timeManager, Player player, int x, int y)
    {
        spriteBatch.Draw(_panelTexture, new Vector2(x, y), Color.White);

        string timeText = timeManager.GetFormattedTime();
        string dateText = $"{timeManager.GetSeasonName()} {timeManager.CurrentDay}, Year {timeManager.CurrentYear}";

        spriteBatch.DrawString(_font, timeText, new Vector2(x + 20, y + 20), Color.White);
        spriteBatch.DrawString(_font, dateText, new Vector2(x + 20, y + 50), Color.LightGreen);
        
        // Draw energy bar (actual player energy)
        DrawBar(spriteBatch, x + 20, y + 90, 260, 15, Color.Yellow, player.GetEnergyPercentage(), "Energy");
        DrawBar(spriteBatch, x + 20, y + 115, 260, 15, Color.Red, 1.0f, "Health");
    }

    private void DrawBar(SpriteBatch spriteBatch, int x, int y, int width, int height, Color color, float value, string label)
    {
        // Background
        var bgTexture = Graphics.TextureGenerator.CreateSolidTexture(
            spriteBatch.GraphicsDevice, width, height, new Color(30, 30, 30));
        spriteBatch.Draw(bgTexture, new Vector2(x, y), Color.White);

        // Fill
        var fillTexture = Graphics.TextureGenerator.CreateSolidTexture(
            spriteBatch.GraphicsDevice, (int)(width * value), height, color);
        spriteBatch.Draw(fillTexture, new Vector2(x, y), Color.White);

        // Border
        var borderTexture = Graphics.TextureGenerator.CreateBorderedRectangle(
            spriteBatch.GraphicsDevice, width, height, Color.Transparent, Color.White, 1);
        spriteBatch.Draw(borderTexture, new Vector2(x, y), Color.White);

        // Label
        spriteBatch.DrawString(_font, label, new Vector2(x - 60, y), Color.White);
    }

    private void DrawInventoryHotbar(SpriteBatch spriteBatch, Inventory inventory, 
        GraphicsDevice graphicsDevice, int x, int y)
    {
        var items = inventory.GetItems();
        
        for (int i = 0; i < Inventory.MaxSlots; i++)
        {
            int slotX = x + i * 50;
            
            // Draw slot background
            Color slotColor = i == inventory.SelectedSlot ? Color.Yellow : Color.White;
            spriteBatch.Draw(_slotTexture, new Vector2(slotX, y), slotColor);

            // Draw item if present
            if (i < items.Count)
            {
                var item = items[i];
                Color itemColor = item.Type switch
                {
                    ItemType.Seed => Color.Brown,
                    ItemType.Crop => Color.Green,
                    ItemType.Tool => Color.Gray,
                    _ => Color.White
                };

                var itemIcon = Graphics.TextureGenerator.CreateItemIcon(graphicsDevice, 30, itemColor);
                spriteBatch.Draw(itemIcon, new Vector2(slotX + 5, y + 5), Color.White);

                // Draw quantity
                if (item.Quantity > 1)
                {
                    spriteBatch.DrawString(_font, item.Quantity.ToString(), 
                        new Vector2(slotX + 25, y + 25), Color.White);
                }
            }

            // Draw slot number
            spriteBatch.DrawString(_font, ((i + 1) % 10).ToString(), 
                new Vector2(slotX + 2, y - 18), Color.White);
        }
    }
}
