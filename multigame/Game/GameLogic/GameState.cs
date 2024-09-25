using multigame.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace multigame
{
    public class GameState
    {
        public Board board { get; set; }
        public BackGround BackGround { get; set; }
        public bool CanMove { get; set; }

        public GameState(int size) 
        { 
            board = new Board(size);
            BackGround=new BackGround(size);
            BackGround.InitializeBackGround();
        }

        public void WhoMovesUpdate(bool change)
        {
            CanMove = change;
        }
    }
}
