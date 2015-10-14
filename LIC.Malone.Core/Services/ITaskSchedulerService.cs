using System;
using LIC.Malone.Core.Scheduling;

namespace LIC.Malone.Core.Services
{
	public interface ITaskSchedulerService
	{
		int TaskCount();

		void Add(TimerTask task);
      TimerTask Add(DateTime runAt, Action action);
      TimerTask Add(TimeSpan runAt, Action action);
		TimerTask Add(double milliseconds, Action action);
		RepeatingTimerTask Add(DateTime runAt, Func<DateTime> action, TimeSpan errorDelay);
      RepeatingTimerTask AddRepeatingTask(Func<DateTime> action);
		RepeatingTimerTask AddRepeatingTask(Func<DateTime> action, TimeSpan initialDelay);

		void CheckTasksNow();

		bool Remove(TimerTask timerTask);

		void Start();
		void Stop();
	}
}
