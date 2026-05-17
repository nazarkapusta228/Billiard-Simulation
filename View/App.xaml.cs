using System.Windows;
using BilliardSimulation.Data;
using BilliardSimulation.Logic;
using BilliardSimulation.ViewModel;

namespace View
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Data layer
            IBallRepository repository = new BallRepository();
            IDiagnosticLogger logger = new DiagnosticLogger();

            // Logic layer
            IBallLogic logic = new BallLogic(repository, logger);

            // ViewModel
            var viewModel = new MainViewModel(logic);

            // UI
            var mainWindow = new MainWindow();
            mainWindow.DataContext = viewModel;
            mainWindow.Show();
        }
    }
}