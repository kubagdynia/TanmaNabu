using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace TanmaNabu.Core
{
    public class GameTime
    {
        private Clock _clock;

        public Time ElapsedTime { get; set; }

        public GameTime()
        {
            _clock = new Clock();
        }

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
