using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SuperOtto.Core;

/// <summary>
/// Represents the player character with movement and interactions.
/// </summary>
public class Player
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; private set; }
    
    private const float MoveSpeed = 150f;
    private const float Size = 32f;

    public Player(Vector2 startPosition)
    {
        Position = startPosition;
    }

    public void Update(GameTime gameTime)
    {
        var keyState = Keyboard.GetState();
        var movement = Vector2.Zero;

        if (keyState.IsKeyDown(Keys.W) || keyState.IsKeyDown(Keys.Up))
            movement.Y -= 1;
        if (keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down))
            movement.Y += 1;
        if (keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.Left))
            movement.X -= 1;
        if (keyState.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.Right))
            movement.X += 1;

        if (movement != Vector2.Zero)
        {
            movement.Normalize();
            Velocity = movement * MoveSpeed;
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        else
        {
            Velocity = Vector2.Zero;
        }
    }

    public Rectangle GetBounds()
    {
        return new Rectangle((int)Position.X, (int)Position.Y, (int)Size, (int)Size);
    }

    public Point GetTilePosition()
    {
        return new Point((int)(Position.X / World.TileSize), (int)(Position.Y / World.TileSize));
    }

    public Point GetFacingTilePosition()
    {
        var tilePos = GetTilePosition();
        
        // Determine facing direction based on last movement
        if (Math.Abs(Velocity.X) > Math.Abs(Velocity.Y))
        {
            if (Velocity.X > 0)
                tilePos.X++;
            else if (Velocity.X < 0)
                tilePos.X--;
        }
        else if (Velocity.Y != 0)
        {
            if (Velocity.Y > 0)
                tilePos.Y++;
            else if (Velocity.Y < 0)
                tilePos.Y--;
        }
        
        return tilePos;
    }
}
