using System.Windows;
using BilliardSimulation.Data;
using BilliardSimulation.Logic;
using BilliardSimulation.ViewModel;

namespace View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create dependencies here (composition root)
            IBallRepository repository = new BallRepository();
            IBallLogic logic = new BallLogic(repository);
            var viewModel = new MainViewModel(logic);

            var mainWindow = new MainWindow();
            mainWindow.DataContext = viewModel;
            mainWindow.Show();
        }
    }
}
