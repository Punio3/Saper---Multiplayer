using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
     class GameState
    {
        public Point LastMove {  get; set; }
        public string Name { get; set; }    

        public GameState(Point x) 
        {
            LastMove = x;
        }

    }
}
