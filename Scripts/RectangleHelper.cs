﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameJam_KoganDev.Scripts
{
    static class RectangleHelper
    {
        public static bool TouchTopOf(this Rectangle r1, Rectangle r2)
        {
            return (r1.Bottom >= r2.Top - 1
                           && r1.Bottom <= r2.Top + (r2.Height / 2)
                           && r1.Right >= r2.Left + (r2.Width / 6f)
                           && r1.Left <= r2.Right - (r2.Width / 6f));
        }
        public static bool TouchBottomOf(this Rectangle r1, Rectangle r2)
        {
            return (r1.Top <= r2.Bottom
                          && r1.Top >= r2.Top + (r2.Height / 2)
                          && r1.Right >= r2.Left + (r2.Width / 6f)
                          && r1.Left <= r2.Right - (r2.Width / 6f));
        }

        public static bool TouchLeftOf(this Rectangle r1, Rectangle r2)
        {
            return (r1.Right <= r2.Right &&
                       r1.Right >= r2.Left &&
                       r1.Top <= r2.Bottom - (r2.Width / 2) &&
                       r1.Bottom >= r2.Top + (r2.Width / 4));
        }

        public static bool TouchRightOf(this Rectangle r1, Rectangle r2)
        {
            return (r1.Left >= r2.Left &&
                          r1.Left <= r2.Right &&
                          r1.Top <= r2.Bottom - (r2.Width / 2) &&
                          r1.Bottom >= r2.Top + (r2.Width / 4));
        }
    }
}
