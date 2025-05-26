using Oakton;
using Pinger;
using Wolverine;
using Wolverine.NATS;

#region sample_bootstrapping_rabbitmq

return await Host.CreateDefaultBuilder(args)
    .UseWolverine(opts =>
    {
        opts.ApplicationAssembly = typeof(Program).Assembly;

        // Listen for messages coming into the pongs subject
        opts.ListenToNatsSubject("pongs");

        // Publish messages to the pings subject
        opts.PublishMessage<PingMessage>().ToNatsSubject("pings");

        opts.UseNats();

        opts.Services.AddHostedService<PingerService>();
    }).RunOaktonCommands(args);

#endregion