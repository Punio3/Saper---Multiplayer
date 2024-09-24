using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace multigame.GameLogic
{
    public abstract class GrassAndFlags : Block
    {
        public abstract bool isFlagged { set; get; }
        public abstract bool isClicked { set; get; }
    }
}
