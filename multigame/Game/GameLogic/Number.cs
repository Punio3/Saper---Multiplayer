using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace multigame.GameLogic
{
     class Number : BombsAndNumbers
    {
        public override BlockType Type => BlockType.number;

        private int _amountOfBombs;

        public override int AmountOfBombs
        {
            get { return _amountOfBombs; }
            set { _amountOfBombs = value; }
        }
        public Number()
        {
            _amountOfBombs = 0;
        }
    }
}
