using multigame.MVVM.View;
using multigame.MVVM.ViewModel;
using System.Configuration;
using System.Data;
using System.Windows;

namespace multigame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Game2 CurrentGame { get; set; }
    }

}
