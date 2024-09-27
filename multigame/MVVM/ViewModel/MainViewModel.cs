using multigame.MVVM.Core;
using multigame.MVVM.Model;
using multigame.MVVM.View;
using multigame.Net;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace multigame.MVVM.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand SendMoveToServer { get; set; }
        public RelayCommand TESTBUTTON {  get; set; }
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
            _server.connectedEvent += UserConnected;
            _server.msgReceivedEvent += MessageReceived;
            _server.userDisconnectEvent += userDisconnect;
            _server.StartGame += Startgame;
            _server.WhoStartGame+=WhoStartGame;
            _server.Makemove += MakeMove;
            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username));
            SendMessageCommand = new RelayCommand(o => _server.SendMessageToServer(Message));
            SendMoveToServer=new RelayCommand(o=>_server.SendGameStateToServer(((App)Application.Current).CurrentGame.game));
            TESTBUTTON = new RelayCommand(o => _server.TESTEVENT());
        }


        private void Startgame()
        {
            int size = _server._PacketReader.ReadInt32();
            int[,] board = new int[size, size];
            for(int i=0;i<size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    board[i, j] = _server._PacketReader.ReadInt32();
                }
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Get the current MainWindow instance
                var mainWindow = (MainWindow)Application.Current.MainWindow;

            // Ensure the ContentControl is updated on the UI thread
            
                mainWindow.StartGame(board,size);
            });
        }
        private void WhoStartGame()
        {
            UsernameWhoMoves = _server._PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Username == UsernameWhoMoves) ((App)Application.Current).CurrentGame.game.WhoMovesUpdate(true);
                else ((App)Application.Current).CurrentGame.game.WhoMovesUpdate(false);
            });
        }

        private void MakeMove()
        {
            Position tmp=new Position(_server._PacketReader.ReadInt32(), _server._PacketReader.ReadInt32());
            UsernameWhoMoves = _server._PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Username == UsernameWhoMoves) ((App)Application.Current).CurrentGame.game.WhoMovesUpdate(true);
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
            var message=_server._PacketReader.ReadMessage();
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

