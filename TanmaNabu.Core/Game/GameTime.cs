using SFML.System;

namespace TanmaNabu.Core.Game;

public class GameTime
{
    private readonly Clock _clock = new();

    public Time ElapsedTime { get; set; }

    internal void Restart()
    {
        ElapsedTime = _clock.Restart();
    }

    internal void SetTime(Time time)
    {
        ElapsedTime = time;
    }
}