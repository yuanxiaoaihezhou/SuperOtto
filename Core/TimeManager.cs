using Microsoft.Xna.Framework;

namespace SuperOtto.Core;

/// <summary>
/// Manages the in-game time system (days, seasons, time of day).
/// </summary>
public class TimeManager
{
    private const int MinutesPerDay = 1440; // 24 hours * 60 minutes
    private const float RealSecondsPerGameMinute = 0.05f; // Speed up time for gameplay

    private float _elapsedTime;
    
    public int CurrentDay { get; private set; } = 1;
    public int CurrentSeason { get; private set; } = 0; // 0=Spring, 1=Summer, 2=Fall, 3=Winter
    public int CurrentYear { get; private set; } = 1;
    public int CurrentMinute { get; private set; } = 360; // Start at 6:00 AM
    
    public string[] SeasonNames { get; } = { "Spring", "Summer", "Fall", "Winter" };
    
    /// <summary>
    /// Gets the current hour (0-23).
    /// </summary>
    public int CurrentHour => CurrentMinute / 60;
    
    /// <summary>
    /// Gets formatted time string (HH:MM AM/PM).
    /// </summary>
    public string GetFormattedTime()
    {
        int hour = CurrentHour;
        int minute = CurrentMinute % 60;
        string ampm = hour >= 12 ? "PM" : "AM";
        int displayHour = hour % 12;
        if (displayHour == 0) displayHour = 12;
        return $"{displayHour:D2}:{minute:D2} {ampm}";
    }

    /// <summary>
    /// Gets the current season name.
    /// </summary>
    public string GetSeasonName() => SeasonNames[CurrentSeason];

    /// <summary>
    /// Updates the time system.
    /// </summary>
    public void Update(GameTime gameTime)
    {
        _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_elapsedTime >= RealSecondsPerGameMinute)
        {
            _elapsedTime -= RealSecondsPerGameMinute;
            CurrentMinute++;

            // New day
            if (CurrentMinute >= MinutesPerDay)
            {
                CurrentMinute = 360; // Reset to 6:00 AM
                CurrentDay++;

                // New season (28 days per season)
                if (CurrentDay > 28)
                {
                    CurrentDay = 1;
                    CurrentSeason++;

                    // New year
                    if (CurrentSeason > 3)
                    {
                        CurrentSeason = 0;
                        CurrentYear++;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the light intensity based on time of day (0.0 to 1.0).
    /// </summary>
    public float GetLightIntensity()
    {
        // Sunrise at 6:00 AM (360 minutes)
        // Sunset at 8:00 PM (1200 minutes)
        if (CurrentMinute < 360) return 0.3f; // Night
        if (CurrentMinute < 420) return MathHelper.Lerp(0.3f, 1.0f, (CurrentMinute - 360) / 60.0f); // Dawn
        if (CurrentMinute < 1140) return 1.0f; // Day
        if (CurrentMinute < 1200) return MathHelper.Lerp(1.0f, 0.3f, (CurrentMinute - 1140) / 60.0f); // Dusk
        return 0.3f; // Night
    }
}
