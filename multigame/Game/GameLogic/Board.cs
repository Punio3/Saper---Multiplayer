using multigame.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace multigame
{
    public class Board
    {
        public int size;
        public int AmountOfBombs=0;
        public GrassAndFlags[,] board;
        public BombsAndNumbers[,] BombsAndNumbers;
        public int DiscoveredBlocks;
        public GameEndsOption GameEnd;
        public Position LastMove;
        public Board(int Size)
        {
            size = Size;
            board = new GrassAndFlags[Size, Size];
            BombsAndNumbers=new BombsAndNumbers[Size,Size];
            GameEnd = GameEndsOption.running;
            InitializeBoard();
            AddBombs(10);
            CalculateBombs(size);
        }

        public GrassAndFlags this[int row, int col]
        {
            get { return board[row, col]; }
            set { board[row, col] = value; }
        }

        public void InitializeBoard()
        {
            for(int i=0;i<size; i++)
            {
                for(int j=0;j<size; j++)
                {
                    if ((i + j) % 2 == 0) board[i, j] = new GrassLight();
                    else board[i, j] = new GrassDark();

                    BombsAndNumbers[i,j] = new Number();
                }
            }

        }

        private void AddBombs(int amountofbombs)
        {
            Random random = new Random();
            int x, y;
            while (AmountOfBombs < amountofbombs)
            {
                x = random.Next(0, 10);
                y = random.Next(0, 10);
                if (BombsAndNumbers[x, y].Type != BlockType.bomb)
                {
                    BombsAndNumbers[x, y] = new Bomb();
                    AmountOfBombs++;
                }
            }
        }

        private void CalculateBombs(int width)
        {

            for (int k = 0; k < width; k++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (BombsAndNumbers[k, i].Type != BlockType.bomb)
                    {
                        for (int z = k - 1; z <= k + 1; z++)
                        {
                            for (int p = i - 1; p <= i + 1; p++)
                            {
                                if (z != k || p != i)
                                {
                                    if (IsInBoardSize(z, p))
                                    {
                                        if (BombsAndNumbers[z, p].Type == BlockType.bomb) BombsAndNumbers[k, i].AmountOfBombs++;
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }

        public bool IsInBoardSize(int x, int y)
        {

            if (x < 0 || x > size-1) return false;
            else if (y < 0 || y > size-1) return false;
            else return true;

        }

        public void DiscoverBlock(Position pos)
        {
            if (board[pos.x, pos.y].isClicked == false && board[pos.x, pos.y].isFlagged == false)
            {
                BlockType tmp = BombsAndNumbers[pos.x, pos.y].Type;
                
                if (tmp == BlockType.bomb)
                {
                    board[pos.x, pos.y].isClicked = true;
                    GameEnd = GameEndsOption.lose;
                }
                else if (tmp == BlockType.number)
                {                    
                    DiscoveredBlocks++;
                    board[pos.x, pos.y].isClicked = true;
                    if (BombsAndNumbers[pos.x, pos.y].AmountOfBombs == 0)
                    {
                        for (int z = pos.x - 1; z <= pos.x + 1; z++)
                        {
                            for (int p = pos.y - 1; p <= pos.y + 1; p++)
                            {
                                if (z != pos.x || p != pos.y)
                                {
                                    if (IsInBoardSize(z, p))
                                    {
                                        DiscoverBlock(new Position(z, p));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void AddFlag(Position pos)
        {
            GrassAndFlags copy = board[pos.x, pos.y];

            if (copy.isClicked == false && copy.isFlagged == false)
            {
                copy.isFlagged = true;
            }
            else
            {
                copy.isFlagged = false;
            }
        }

        public void CheckWin()
        {
            if (size * size - DiscoveredBlocks == AmountOfBombs)
            {
                for (int k = 0; k < size; k++)
                {
                    for (int i = 0; i < size; i++)
                    {
                        board[k, i].isClicked = true;
                        board[k, i].isFlagged = false;
                    }
                }
                GameEnd = GameEndsOption.win;
                return;
            }
            GameEnd = GameEndsOption.running;
        }

    }
}
