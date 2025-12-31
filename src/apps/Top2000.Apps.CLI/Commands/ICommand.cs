namespace Top2000.Apps.CLI.Commands;

public interface ICommand
{
    Command Create();
}

public interface ICommand<T> : ICommand
where T : ICommand
{
}

public interface ISubCommand
{
    void CreateSubCommand();
}