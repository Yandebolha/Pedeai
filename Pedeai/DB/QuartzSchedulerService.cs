using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace Pedeai.DB
{
    /// <summary>
    /// Gerencia o ciclo de vida do scheduler Quartz para polling de pedidos web.
    /// </summary>
    public static class QuartzSchedulerService
    {
        private static IScheduler _scheduler;

        public static async Task StartAsync()
        {
            try
            {
                _scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await _scheduler.Start();

                IJobDetail job = JobBuilder.Create<WebOrderPollingJob>()
                    .WithIdentity("webOrderPolling", "pedeai")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("webOrderTrigger", "pedeai")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(10)
                        .RepeatForever())
                    .Build();

                await _scheduler.ScheduleJob(job, trigger);

                Logger.Log("QuartzSchedulerService", "StartAsync",
                    "Scheduler iniciado — polling de pedidos web a cada 10s", null);
            }
            catch (Exception ex)
            {
                Logger.Log("QuartzSchedulerService", "StartAsync",
                    "Erro ao iniciar scheduler Quartz", ex);
            }
        }

        public static async Task StopAsync()
        {
            try
            {
                if (_scheduler != null && _scheduler.IsStarted)
                    await _scheduler.Shutdown(waitForJobsToComplete: false);
            }
            catch (Exception ex)
            {
                Logger.Log("QuartzSchedulerService", "StopAsync", "Erro ao parar scheduler", ex);
            }
        }
    }
}
