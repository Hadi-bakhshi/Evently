using Microsoft.Extensions.Options;
using Quartz;

namespace Evently.Modules.Users.Infrastructure.Outbox;
internal sealed class ConfigureProcessOutboxJob(IOptions<OutboxOptions> outBoxOptions)
    : IConfigureOptions<QuartzOptions>
{

    private readonly OutboxOptions _outboxOptions = outBoxOptions.Value;

    public void Configure(QuartzOptions options)
    {
        string jobName = typeof(ProcessOutboxJob).FullName!;

        options
            .AddJob<ProcessOutboxJob>(configure => configure.WithIdentity(jobName))
            .AddTrigger(configure =>
                configure
                    .ForJob(jobName)
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInSeconds(_outboxOptions.IntervalInSeconds).RepeatForever()));
    }
}
