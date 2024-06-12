using SFML.System;

namespace TanmaNabu.Core
{
    public class GameTime
    {
        private readonly Clock _clock = new Clock();

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
}
