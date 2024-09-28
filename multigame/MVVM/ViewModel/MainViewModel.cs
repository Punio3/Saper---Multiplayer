using multigame.Game.GameLogic;
using multigame.MVVM.Core;
using multigame.MVVM.Model;
using multigame.MVVM.View;
using multigame.Net;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Documents;

namespace multigame.MVVM.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand ReadyToStartGameCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand SendMoveToServer { get; set; }
        public RelayCommand SendEndGameType { get; set; }
        public RelayCommand TESTBUTTON { get; set; }
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<String> Messages { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }

        private server _server;

        private string _usernameWhoMoves;
        public string UsernameWhoMoves
        {
            get { return _usernameWhoMoves; }
            set
            {
                if (_usernameWhoMoves != value)
                {
                    _usernameWhoMoves = value;
                    OnPropertyChanged(nameof(UsernameWhoMoves));
                }
            }
        }

        private string _timeSpentInGame;
        public string TimeSpentInGame
        {
            get { return _timeSpentInGame; }
            set
            {
                if (_timeSpentInGame != value)
                {
                    _timeSpentInGame = value;
                    OnPropertyChanged(nameof(TimeSpentInGame));
                }
            }
        }

        // Dodanie GameTimer jako pole w MainViewModel
        public GameTimer _gameTimer;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel()
        {
            _server = new server();
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<String>();

            // Inicjalizacja GameTimer i rejestracja do wydarzenia TimeUpdated
            _gameTimer = new GameTimer();
            _gameTimer.TimeUpdated += OnTimeUpdated;

            _server.connectedEvent += UserConnected;
            _server.msgReceivedEvent += MessageReceived;
            _server.userDisconnectEvent += userDisconnect;
            _server.StartGame += Startgame;
            _server.WhoStartGame += WhoStartGame;
            _server.Makemove += MakeMove;
            _server.ReturnToQueue += ReturnToQueueEvent;

            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username));
            SendMessageCommand = new RelayCommand(o => _server.SendMessageToServer(Message));
            SendMoveToServer = new RelayCommand(o => _server.SendGameStateToServer(((App)Application.Current).CurrentGame.game));
            SendEndGameType = new RelayCommand(o => _server.SendWinOrLoseInfoToServer(((App)Application.Current).CurrentGame.game.board.GameEnd));
            ReadyToStartGameCommand = new RelayCommand(o => _server.SendReadyToStartGame());
            TESTBUTTON = new RelayCommand(o => _server.TESTEVENT());
        }

        // Metoda aktualizująca TimeSpentInGame na podstawie GameTimer
        private void OnTimeUpdated(object sender, string time)
        {
            TimeSpentInGame = time; // Przypisanie wartości czasu do TimeSpentInGame
        }

        private async void ReturnToQueueEvent()
        {
            await Task.Delay(2000);
            Application.Current.Dispatcher.Invoke(() =>
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.StartQueue();
            });
        }

        private void Startgame()
        {
            int size = _server._PacketReader.ReadInt32();
            int[,] board = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    board[i, j] = _server._PacketReader.ReadInt32();
                }
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.StartGame(board, size);
            });
        }

        private void WhoStartGame()
        {
            UsernameWhoMoves = _server._PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Username == UsernameWhoMoves)
                {
                    ((App)Application.Current).CurrentGame.game.WhoMovesUpdate(true);
                    _gameTimer.StartTimer(); // Uruchamianie timera gry
                }
                else ((App)Application.Current).CurrentGame.game.WhoMovesUpdate(false);
            });
        }

        private void MakeMove()
        {
            Position tmp = new Position(_server._PacketReader.ReadInt32(), _server._PacketReader.ReadInt32());
            UsernameWhoMoves = _server._PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Username == UsernameWhoMoves)
                {
                    ((App)Application.Current).CurrentGame.game.WhoMovesUpdate(true);
                    _gameTimer.StartTimer(); // Uruchamianie timera po ruchu
                }
                else ((App)Application.Current).CurrentGame.game.WhoMovesUpdate(false);
                ((App)Application.Current).CurrentGame.HandleLeftClick(tmp);
            });
        }

        private void userDisconnect()
        {
            var uid = _server._PacketReader.ReadMessage();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        private void MessageReceived()
        {
            var message = _server._PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(message));
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                UserName = _server._PacketReader.ReadMessage(),
                UID = _server._PacketReader.ReadMessage(),
            };

            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }
    }
}