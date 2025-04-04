using CommandExecuter.Interfaces;
using CommandExecuter.Models;
using CommandExecuter.ViewModels;

namespace CommandExecuter.Test.ViewModels;

[TestFixture]
public class MainWindowViewModelTests
{
    private MainWindowViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _viewModel = new MainWindowViewModel(new TestDispatcherService());
        }

        [Test]
        public async Task ExecuteShellCommandAsync_WithEmptyCommand_AddsErrorToResults()
        {
            var initialCount = _viewModel.ShellCommandResults.Count;
            await _viewModel.ExecuteShellCommandAsync(null);

            Assert.That(_viewModel.ShellCommandResults.Count, Is.EqualTo(initialCount + 1));
            var lastResult = _viewModel.ShellCommandResults.Last();
        Assert.Multiple(() =>
        {
            Assert.That(lastResult.Text, Is.EqualTo("Please enter a command to execute."));
            Assert.That(lastResult.OutputType, Is.EqualTo(OutputType.Error));
        });
        }

        [Test]
        public async Task ExecuteShellCommandAsync_WithValidCommand_CapturesOutputAndExitCode()
        {
            var initialCount = _viewModel.ShellCommandResults.Count;
            const string testCommand = "echo hello";
            await _viewModel.ExecuteShellCommandAsync(testCommand);

            Assert.That(_viewModel.ShellCommandResults.Count, Is.GreaterThanOrEqualTo(initialCount + 2));
            var outputResult = _viewModel.ShellCommandResults[^2];
        Assert.Multiple(() =>
        {
            Assert.That(outputResult.Text, Does.Contain("hello"));
            Assert.That(outputResult.OutputType, Is.EqualTo(OutputType.Standard));
        });

        var exitResult = _viewModel.ShellCommandResults.Last();
        Assert.Multiple(() =>
        {
            Assert.That(exitResult.Text, Does.Contain("Exit code: 0"));
            Assert.That(exitResult.OutputType, Is.EqualTo(OutputType.Status));
        });
    }

        [Test]
        public async Task ExecuteShellCommandAsync_WithInvalidCommand_CapturesErrorAndExitCode()
        {
            var initialCount = _viewModel.ShellCommandResults.Count;
            const string invalidCommand = "thisShouldFailForSure12345";
            await _viewModel.ExecuteShellCommandAsync(invalidCommand);

            Assert.That(_viewModel.ShellCommandResults.Count, Is.GreaterThanOrEqualTo(initialCount + 2));
            var errorOutput = _viewModel.ShellCommandResults[_viewModel.ShellCommandResults.Count - 2];
            Assert.That(errorOutput.OutputType, Is.EqualTo(OutputType.Error));

            var exitResult = _viewModel.ShellCommandResults.Last();
        Assert.Multiple(() =>
        {
            Assert.That(exitResult.Text, Does.Contain("Exit code:"));
            Assert.That(exitResult.OutputType, Is.EqualTo(OutputType.Status));
        });
    }
}

// For testing purposes DispatcherService that executes immediately
public class TestDispatcherService : IDispatcherService
{
    public void Post(Action action) => action();
}

