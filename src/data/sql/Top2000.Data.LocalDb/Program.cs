var connectionString = Environment.GetEnvironmentVariable("Connectionstrings__Top2000") 
                       ?? throw new InvalidOperationException("ConnectionStrings__Top2000 is not configured in environment variables!");

EnsureDatabase.For
    .SqlDatabase(connectionString);

var upgradeEngine = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptEmbeddedInDataLibrary()
    .WithTransactionPerScript()
    .LogToConsole()
    .Build() ?? throw new InvalidOperationException($"upgradeEngine is null");

var result = upgradeEngine.PerformUpgrade();

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error.ToString());
    Console.ResetColor();
    return -1;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Success!!");
Console.ResetColor();

return 0;