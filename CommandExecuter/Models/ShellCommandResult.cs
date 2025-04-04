namespace CommandExecuter.Models;

public class ShellCommandResult
{
    public string Text { get; set; } = string.Empty;
    public OutputType OutputType { get; set; }
}