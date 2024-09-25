using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace multigame.GameLogic
{
    public class BackGround
    {
        public int size;
        public Block[,] board;
        public BackGround(int Size)
        {
            size = Size;
            board = new Block[Size, Size];
        }

        public Block this[int row, int col]
        {
            get { return board[row, col]; }
            set { board[row, col] = value; }
        }

        public void InitializeBackGround()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if ((i + j) % 2 == 0) board[i, j] = new BackGround_Light();
                    else board[i, j] = new BackGround_Dark();
                }
            }
        }
    }
}