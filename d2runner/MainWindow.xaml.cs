using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;

namespace d2runner
{
    public class ReactiveMainWindow : ReactiveWindow<MainWindowViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveMainWindow
    {
        //public static readonly DependencyProperty ViewModelProperty =
        //    DependencyProperty.Register(nameof(ViewModel), typeof(MainWindowViewModel), typeof(MainWindow));

        //public MainWindowViewModel ViewModel
        //{
        //    get { return (MainWindowViewModel)GetValue(ViewModelProperty); }
        //    set { SetValue(ViewModelProperty, value); }
        //}

        //object? IViewFor.ViewModel { get => ViewModel; set => ViewModel = (MainWindowViewModel)value; }

        public MainWindow()
        {
            InitializeComponent();
            this.ViewModel = new MainWindowViewModel();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(this.ViewModel, vm => vm.CurrentRun, v => v.CurrentRun.Content).DisposeWith(disposable);
                this.OneWayBind(this.ViewModel, vm => vm.CurrentRunElapsed, v => v.CurrentRunElapsed.Text, s => s.ToString("h\\:mm\\:ss\\.f")).DisposeWith(disposable);
                this.OneWayBind(this.ViewModel, vm => vm.PlayPauseIcon, v => v.PauseResumeButton.Content).DisposeWith(disposable);
                this.BindCommand(this.ViewModel, vm => vm.StartStopCommand, v => v.PauseResumeButton).DisposeWith(disposable);
                this.BindCommand(this.ViewModel, vm => vm.AbandonRunCommand, v => v.DeleteRunButton).DisposeWith(disposable);
            });
        }
    }
}
