using multigame.MVVM.View;
using multigame.MVVM.ViewModel;
using System.Windows;

namespace multigame
{
    public partial class MainWindow : Window
    {
        private MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();

            // Inicjalizacja MainViewModel raz
            _mainViewModel = new MainViewModel();

            // Przypisanie MainViewModel jako DataContext dla MainWindow
            this.DataContext = _mainViewModel;
        }

        public void StartGame(int[,] board,int size)
        {
            var gameView = new Game2(board,size);

            // Przekazanie MainViewModel do Game2
            gameView.DataContext = _mainViewModel;

            MenuContainer.Content = gameView;
            ((App)Application.Current).CurrentGame = gameView;
        }

        public void StartQueue()
        {
            MenuContainer.Content = null;
        }
    }
}