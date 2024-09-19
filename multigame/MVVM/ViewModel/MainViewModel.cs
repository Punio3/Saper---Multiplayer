using multigame.MVVM.Core;
using multigame.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace multigame.MVVM.ViewModel
{
    class MainViewModel
    {
        public RelayCommand ConnectToServerCommand { get; set; }
        private server _server;

        public MainViewModel()
        {
            _server = new server();
            // Komenda, która ustawia wiadomość serwera do właściwości ServerMessage
            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer());
        }

    }
}

