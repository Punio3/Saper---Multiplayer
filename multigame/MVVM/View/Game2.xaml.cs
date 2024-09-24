﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using multigame.GameLogic;
using multigame.MVVM.View.GameUI;

namespace multigame.MVVM.View
{
    /// <summary>
    /// Logika interakcji dla klasy Game2.xaml
    /// </summary>
    public partial class Game2 : UserControl
    {
        private readonly Image[,] BackGroundImages = new Image[10, 10];
        private readonly Image[,] GrassImages = new Image[10, 10];
        private readonly Image[,] FlagsImages = new Image[10, 10];
        private readonly Image[,] BombsAndNumbersImages = new Image[10, 10];

        private GameState game;
        public Game2()
        {
            InitializeComponent();
            game = new GameState(10);
            InitializeBoard();
            DrawBoard(game.board);
            WinText.Visibility = Visibility.Hidden;
            LoseText.Visibility = Visibility.Hidden;
        }

        private void InitializeBoard()
        {
            for (int k = 0; k < 10; k++)
            {
                for (int i = 0; i < 10; i++)
                {
                    Image image = new Image();
                    BackGroundImages[k, i] = image;
                    BlocksGrid.Children.Add(image);

                    Image image2 = new Image();
                    GrassImages[k, i] = image2;
                    GrassGrid.Children.Add(image2);

                    Image image3 = new Image();
                    FlagsImages[k, i] = image3;
                    FlagsGrid.Children.Add(image3);

                    Image image4 = new Image();
                    BombsAndNumbersImages[k, i] = image4;
                    BombsAndNumbersGrid.Children.Add(image4);
                }
            }
        }

        private void DrawBoard(Board board)
        {
            for (int k = 0; k < 10; k++)
            {
                for (int i = 0; i < 10; i++)
                {
                    Block background = game.BackGround[k, i];
                    BackGroundImages[k, i].Source = Images.GetImage(background);

                    BombsAndNumbers bombsandnumbers = game.board.BombsAndNumbers[k, i];
                    BombsAndNumbersImages[k, i].Source = Images.GetImage(bombsandnumbers);

                    GrassAndFlags block = game.board[k, i];
                    GrassImages[k, i].Source = Images.GetImage(block);

                    FlagsImages[k, i].Source = Images.GetImage(block.isFlagged);



                }
            }
        }

        private void MouseDown_Grid(object sender, MouseButtonEventArgs eventArgs)
        {


            Point point = eventArgs.GetPosition(BoardGrid);
            Position pos = ToSquarePosition(point);
            if (game.board.GameEnd == GameEndsOption.running)
            {
                if (eventArgs.ChangedButton == MouseButton.Left)
                {
                    HandleLeftClick(pos);
                }
                else if (eventArgs.ChangedButton == MouseButton.Right)
                {
                    HandleRightClick(pos);
                }
            }
        }

        private void HandleLeftClick(Position pos)
        {
            game.board.DiscoverBlock(pos);
            if (game.board.GameEnd != GameEndsOption.lose)
            {
                game.board.CheckWin();
            }
            if (game.board.GameEnd == GameEndsOption.win) WinText.Visibility = Visibility.Visible;
            else if (game.board.GameEnd == GameEndsOption.lose) LoseText.Visibility = Visibility.Visible;
            DrawBoard(game.board);
        }

        private void HandleRightClick(Position pos)
        {
            game.board.AddFlag(pos);
            DrawBoard(game.board);
        }

        private Position ToSquarePosition(Point point)
        {
            double squareSize = BoardGrid.Width / game.board.size;
            int x = (int)(point.X / squareSize);
            int y = (int)(point.Y / squareSize);
            return new Position(y, x);
        }

    }
}

