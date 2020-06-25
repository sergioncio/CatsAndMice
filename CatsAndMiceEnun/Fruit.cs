using CatsAndMice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatsAndMiceEnun
{
    class Fruit: GameItem
    {
        public Fruit() : base(null, 0)
        {
        }

        public Fruit(ItemCoordinates coords, int myValue):base(coords, myValue)
        {
        }

        public override string ToString()
        {
            return "[" + "Fruit," + this.Coords.Fila + ", " + this.Coords.Columna + "]";
        }
    }
}
