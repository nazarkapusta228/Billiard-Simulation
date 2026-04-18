using System.Windows;

namespace View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Minimal code-behind: UI-only wiring. ViewModel is created in App.xaml.cs (composition root)
            // Wire size changes to ViewModel if DataContext is set
            this.Loaded += (_, _) =>
            {
                if (DataContext is BilliardSimulation.ViewModel.MainViewModel vm)
                {
                    PlayFieldBorder.SizeChanged += (s, e) =>
                    {
                        vm.SetTableSize(PlayFieldBorder.ActualWidth, PlayFieldBorder.ActualHeight);
                    };

                    // Initialize sizes now
                    vm.SetTableSize(PlayFieldBorder.ActualWidth, PlayFieldBorder.ActualHeight);
                }
            };
        }
    }
}
