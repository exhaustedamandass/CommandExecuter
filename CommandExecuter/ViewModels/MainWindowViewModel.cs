using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using CommandExecuter.Interfaces;
using CommandExecuter.Models;
using ReactiveUI;

namespace CommandExecuter.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private const string EmptyCommandErrorMessage = "Please enter a command to execute.";
    private string? _shellCommand;
    private readonly IDispatcherService _dispatcherService;
    
    // Avalonia XAML loader requires a parameterless constructor
    public MainWindowViewModel() : this(new AvaloniaDispatcherService())
    { }

    public MainWindowViewModel(IDispatcherService? dispatcherService = null)
    {
        // Init ExecuteShellCommandAsyncCommand
        ExecuteShellCommandAsyncCommand = 
            ReactiveCommand.CreateFromTask<string?>(ExecuteShellCommandAsync);

        _dispatcherService = dispatcherService ?? new AvaloniaDispatcherService();
    }
    
    public ICommand ExecuteShellCommandAsyncCommand { get; }

    public string? ShellCommand
    {
        get => _shellCommand;
        set => this.RaiseAndSetIfChanged(ref _shellCommand, value);
    }
    
    public ObservableCollection<ShellCommandResult> ShellCommandResults { get; } = [];
    
    // should be private, but for testing purposes we need to make it public
    public async Task ExecuteShellCommandAsync(string? command)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            AddToShellCommandResults(EmptyCommandErrorMessage, OutputType.Error);
            return;
        }

        try
        {
            string shell;
            string shellArgs;
            
            // Escape any double quotes in the command
            var escapedCommand = command.Replace("\"", "\\\"");
            
            // Determine the shell to use based on the OS
            if (OperatingSystem.IsWindows())
            {
                shell = "cmd.exe";
                shellArgs = "/c \"" + escapedCommand + "\"";
            }
            else
            {
                shell = "/bin/bash";
                shellArgs = "-c \"" + escapedCommand + "\"";
            }

            var psi = new ProcessStartInfo
            {
                FileName = shell,
                Arguments = shellArgs,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process();
            process.StartInfo = psi;

            process.Start();

            // Asynchronously read the standard output and error streams.
            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            
            // Make sure we wait for the process to fully exit:
            await Task.Run(() => process.WaitForExit());
            
            var exitCode = process.ExitCode;

            if (!string.IsNullOrWhiteSpace(output))
            {
                AddToShellCommandResults(output, OutputType.Standard);
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                AddToShellCommandResults(error, OutputType.Error);
            }
            
            AddToShellCommandResults($"Exit code: {exitCode}\n", OutputType.Status);
        }
        catch (Exception ex)
        {
            AddToShellCommandResults($"Error executing command:\n{ex.Message}", OutputType.Error);
        }
    }
    
    private void AddToShellCommandResults(string result, OutputType outputType)
    {
        _dispatcherService.Post(() =>
        {
            ShellCommandResults.Add(new ShellCommandResult
            {
                Text = result,
                OutputType = outputType
            });
        });    
    }
}