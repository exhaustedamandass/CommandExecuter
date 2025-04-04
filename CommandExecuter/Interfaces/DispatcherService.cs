using System;
using Avalonia.Threading;

namespace CommandExecuter.Interfaces;

public interface IDispatcherService
{
    void Post(Action action);
}

// DispatcherService using Avalonia's dispatcher, waits for UI thread
public class AvaloniaDispatcherService : IDispatcherService
{
    public void Post(Action action) => Dispatcher.UIThread.Post(action);
}
