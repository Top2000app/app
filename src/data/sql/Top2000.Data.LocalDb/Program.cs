var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Top2000") 
                       ?? throw new InvalidOperationException("ConnectionStrings__Top2000 is not configured in environment variables!");

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