using Oakton;
using Ponger;
using Wolverine;
using Wolverine.NATS;

return await Host.CreateDefaultBuilder(args)
    .UseWolverine(opts =>
    {
        opts.ApplicationAssembly = typeof(Program).Assembly;
        
        opts.ListenToNatsSubject("pings");
        
        opts.PublishMessage<PongMessage>().ToNatsSubject("pongs");

        opts.UseNats();
    })
    .RunOaktonCommands(args);