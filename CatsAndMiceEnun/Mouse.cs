using CatsAndMice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatsAndMiceEnun
{
    class Mouse: GameItem, IMovableItem
    {
        public Mouse():base(null, 0)
        {
        }

        public Mouse(ItemCoordinates coords, int myValue):base(coords, myValue)
        {
        }

        public override string ToString()
        {
            return "[" + "Mouse," + this.Coords.Fila + ", " + this.Coords.Columna + "]";
        }
        public Direction CurrentDirection { get; set; }
        Random rnd = new Random();
        public void ChangeDirection()
        {
            int aleatorio = rnd.Next(0, 10);
            if (aleatorio >= 8)
            {
                this.CurrentDirection = Direction.Left;
            }
            else if (aleatorio >= 7)
            {
                this.CurrentDirection = Direction.Right;
            }
            else if(aleatorio>=4)
            {
                this.CurrentDirection = Direction.Down;
            }
            else
            {
                this.CurrentDirection = Direction.Up;
            }

        }
    }
}
