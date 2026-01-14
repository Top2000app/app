var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder
    .AddParameter("SQLPassword", secret: true);

var sqlServer =  builder
    .AddSqlServer("SQL", sqlPassword, port:1999)
    .WithLifetime(ContainerLifetime.Persistent);

var database = sqlServer
    .AddDatabase("Top2000");

builder.AddProject<Projects.Top2000_Data_LocalDb>("DbUp")
    .WithReference(database)
    .WaitFor(database)
    .WithParentRelationship(database);
    
builder.Build().Run();