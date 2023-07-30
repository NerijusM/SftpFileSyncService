using Autofac;
using Autofac.Extensions.DependencyInjection;
using Quartz;
using SFTP.Wrapper;
using SFTP.Wrapper.Configs;
using SpfSyncingService.Extensions;
using SpfSyncingWorkerCore;
using SpfSyncingWorkerInfrastructure;
using SpfSyncingWorkerInfrastructure.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .UseWindowsService(options =>
    {
        options.ServiceName = "SFTP sync service";
    })
    .ConfigureServices((hostContext, services) =>
    {
        //Entity DbContext added
        //For SqLite db 
        string? connectionString = hostContext.Configuration.GetConnectionString("SqliteConnection");
        //For postgresql db 
        // string? connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException($" There not found db connectionstring {nameof(connectionString)}");

        // Extension to load DbContext
        services.AddDbContext(connectionString);

        // Configure Quarts
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.AddAllJobAndTrigger(hostContext.Configuration);
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        // Configure Logging
        services.AddLogging();

        //Configure  SftpConfig for SFTP.Wrapper;
        string? host = hostContext.Configuration.GetValue<string>("SftpConfig:host");
        string? userName = hostContext.Configuration.GetValue<string>("SftpConfig:username");
        string? password = hostContext.Configuration.GetValue<string>("SftpConfig:password");
        int? port = hostContext.Configuration.GetValue<int>("SftpConfig:port");

        if (string.IsNullOrEmpty(host) ||
            string.IsNullOrEmpty(userName) ||
            string.IsNullOrEmpty(password)  ||
            port == null)
            throw new ArgumentNullException("There are bad Sftp configuration");

        services.UseSftp(new SftpConfig
        {
            
            Host = host,
            UserName = userName,
            Password = password,
            Port = (int)port
        });
    })
    .ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        //Added autofac container from Core and Infrastructure parts of project
        containerBuilder.RegisterModule(new DefaultCoreModule());
        containerBuilder.RegisterModule(new DefaultInfrastructureModule(/*host.Environment.EnvironmentName == "Development")*/));
    })
    .Build();

await host.RunAsync();
