var builder = DistributedApplication.CreateBuilder(args);

var cache       = // redis instance the app will use for output caching
        builder.AddRedisContainer("cache");

var pubsub      = // redis instance the app will use for simple messages
        builder.AddRedisContainer("pubsub");

var peopleTable = // azure table storage for storing people data
        builder.AddAzureStorage("azureStorage")
               .UseEmulator()
               .AddTables("person");

var apiservice  = // the back-end API the front end will call
        builder.AddProject<Projects.AspireAzdTests_ApiService>("apiservice");

_               = // the front end app
        builder.AddProject<Projects.AspireAzdTests_Web>("webfrontend")
               .WithReference(cache)
               .WithReference(pubsub)
               .WithReference(apiservice)
               .WithReference(peopleTable);

builder.Build().Run();
