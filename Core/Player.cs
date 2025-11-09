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
    
    // Energy system
    public float MaxEnergy { get; private set; } = 100f;
    public float CurrentEnergy { get; private set; }
    
    private const float MoveSpeed = 150f;
    private const float Size = 32f;
    private const float MovementEnergyCost = 2f; // Energy per second while moving
    private const float EnergyRecoveryRate = 1f; // Energy per second while idle

    public Player(Vector2 startPosition)
    {
        Position = startPosition;
        CurrentEnergy = MaxEnergy; // Start with full energy
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

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (movement != Vector2.Zero)
        {
            movement.Normalize();
            Velocity = movement * MoveSpeed;
            Position += Velocity * deltaTime;
            
            // Consume energy while moving
            ConsumeEnergy(MovementEnergyCost * deltaTime);
        }
        else
        {
            Velocity = Vector2.Zero;
            
            // Recover energy while idle
            RecoverEnergy(EnergyRecoveryRate * deltaTime);
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

    /// <summary>
    /// Consumes energy for an action. Returns true if energy was available.
    /// </summary>
    public bool ConsumeEnergy(float amount)
    {
        if (CurrentEnergy >= amount)
        {
            CurrentEnergy = Math.Max(0, CurrentEnergy - amount);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Recovers energy up to the maximum.
    /// </summary>
    public void RecoverEnergy(float amount)
    {
        CurrentEnergy = Math.Min(MaxEnergy, CurrentEnergy + amount);
    }

    /// <summary>
    /// Gets the energy as a percentage (0.0 to 1.0).
    /// </summary>
    public float GetEnergyPercentage()
    {
        return CurrentEnergy / MaxEnergy;
    }

    /// <summary>
    /// Checks if the player has enough energy for an action.
    /// </summary>
    public bool HasEnergy(float amount)
    {
        return CurrentEnergy >= amount;
    }
}
