using CatsAndMice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatsAndMiceEnun
{
    class Cat : GameItem, IMovableItem
    {
        public Cat() : base(null, 0)
        {
        }

        public Cat(ItemCoordinates coords, int myValue) : base(coords, myValue)
        {
        }

        public override string ToString()
        {
            return "[" + "Cat," + this.Coords.Fila + ", " + this.Coords.Columna + "]";
        }

        public Direction CurrentDirection { get; set; }
        Random rnd = new Random();
        public void ChangeDirection()
        {
            
            int aleatorio = rnd.Next(0,10);
            if (aleatorio >= 7)
            {
                this.CurrentDirection = Direction.Left;
            }
            else if (aleatorio >= 4)
            {
                this.CurrentDirection = Direction.Right;
            }
            else
            {
                this.CurrentDirection = Direction.Down;
            }
        }
    }
}
