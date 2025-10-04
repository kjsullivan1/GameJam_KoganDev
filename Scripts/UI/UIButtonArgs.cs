using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameJam_KoganDev.Scripts.UI
{
    internal class UIButtonArgs : System.EventArgs
    {
        public Vector2 Location { get; private set; }
        public string ID { get; private set; }

        public UIButtonArgs(string id, Vector2 location)
        {
            ID = id;
            Location = location;
        }
    }
}
