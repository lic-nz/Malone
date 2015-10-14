using System;

namespace LIC.Malone.Core.Scheduling
{
	public class TimerTask
	{
		internal int completed;
		internal Exception exception;
		private readonly Action action;
		internal DateTime runDate;

		internal TimerTask(Action action, DateTime runDate)
		{
			this.action = action;
			this.runDate = runDate;
		}

		public bool Completed
		{
			get { return completed == 1; }
		}

		public DateTime RunDate
		{
			get { return this.runDate; }
		}

		public Exception Exception
		{
			get { return this.exception; }
		}

		internal Action Action
		{
			get { return this.action; }
		}

		public virtual void Execute()
		{
			Action();
		}

		public override string ToString()
		{
			if (action != null)
				return base.ToString() + action.Method;

			return base.ToString();
		}
	}
}
