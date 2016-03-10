using System;

namespace LIC.Malone.Core.Scheduling
{
	public class RepeatingTimerTask : TimerTask
	{
		public RepeatingTimerTask(Func<DateTime> func, DateTime firstRun, TimeSpan errorDelay) : base(null, firstRun)
		{
			Func = func;
			ErrorDelay = errorDelay;
		}

		public Func<DateTime> Func { get; set; }

		public TimeSpan ErrorDelay { get; set; }

		public override void Execute()
		{
			runDate = Func();
		}

		internal void Reset()
		{
			if (Exception != null)
				runDate = DateTime.UtcNow.Add(ErrorDelay);

			completed = 0;
			exception = null;
		}

		public override string ToString()
		{
			return base.ToString() + Func.Method;
		}
	}
}
