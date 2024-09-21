using multigame.MVVM.Core;
using multigame.MVVM.Model;
using multigame.MVVM.View;
using multigame.Net;
using System.Collections.ObjectModel;
using System.Windows;

namespace multigame.MVVM.ViewModel
{
    class MainViewModel
    {
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<String> Messages { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }

        private server _server;
        public MainViewModel()
        {
            _server = new server();
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<String>();  
            _server.connectedEvent += UserConnected;
            _server.msgReceivedEvent += MessageReceived;
            _server.userDisconnectEvent += userDisconnect;
            _server.StartGame += Startgame;
            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username));
            SendMessageCommand = new RelayCommand(o => _server.SendMessageToServer(Message));
        }

        private void Startgame()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Get the current MainWindow instance
                var mainWindow = (MainWindow)Application.Current.MainWindow;

            // Ensure the ContentControl is updated on the UI thread
            
                mainWindow.StartGame();
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

