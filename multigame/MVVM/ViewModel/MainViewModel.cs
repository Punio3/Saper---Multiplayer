using multigame.MVVM.Core;
using multigame.MVVM.Model;
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
            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username));
            SendMessageCommand = new RelayCommand(o => _server.SendMessageToServer(Message));
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

