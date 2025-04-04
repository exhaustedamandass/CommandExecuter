using System.Collections.Specialized;
using Avalonia.Controls;
using CommandExecuter.ViewModels;

namespace CommandExecuter.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        var scrollViewer = this.FindControl<ScrollViewer>("ResultsScrollViewer");

        if (DataContext is MainWindowViewModel vm)
        {
            vm.ShellCommandResults.CollectionChanged += (s, e) =>
            {
                // Only scroll to end if new items are added
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    scrollViewer?.ScrollToEnd();
                }
            };
        }
    }
}