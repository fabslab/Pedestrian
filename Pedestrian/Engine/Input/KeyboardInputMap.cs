﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Pedestrian.Engine.Input
{
    public static class KeyboardInputMap
    {
        public static Dictionary<InputCommand, Keys> GetInputMap(PlayerIndex playerIndex)
        {
            if (playerIndex == PlayerIndex.One)
            {
                return Primary;
            }
            else
            {
                return Secondary;
            }
        }

        public static Dictionary<InputCommand, Keys> Primary = new Dictionary<InputCommand, Keys>
        {
            { InputCommand.Forward, Keys.Up },
            { InputCommand.Right, Keys.Right },
            { InputCommand.Reverse, Keys.Down },
            { InputCommand.Left, Keys.Left },
        };

        public static Dictionary<InputCommand, Keys> Secondary = new Dictionary<InputCommand, Keys>
        {
            { InputCommand.Forward, Keys.W },
            { InputCommand.Right, Keys.D },
            { InputCommand.Reverse, Keys.S },
            { InputCommand.Left, Keys.A },
        };

        public static Dictionary<InputCommand, Keys> Global = new Dictionary<InputCommand, Keys>
        {
            { InputCommand.Up, Keys.Up },
            { InputCommand.Down, Keys.Down },
            { InputCommand.Left, Keys.Left },
            { InputCommand.Right, Keys.Right },
            { InputCommand.Enter, Keys.Enter },
            { InputCommand.Exit, Keys.Escape },
            { InputCommand.Pause, Keys.Escape },
        };
    }
}
