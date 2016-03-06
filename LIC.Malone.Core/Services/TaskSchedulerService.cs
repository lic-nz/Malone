using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using LIC.Malone.Core.Scheduling;
using Action = System.Action;

namespace LIC.Malone.Core.Services
{
	public class TaskSchedulerService : ITaskSchedulerService, IDisposable
	{
		/// <summary>
		/// The maximum lag time allowed. RunAt times less then this value are not allowed.
		/// </summary>
		public readonly TimeSpan MaxPassed;
		private readonly object _sync;
		private Thread _mt;
		private readonly SortedList<DateTime, TimerTask> _taskListSorted;
		protected AutoResetEvent WaitEvent = new AutoResetEvent(true);
		private DateTime _lastRunTime;
		protected int TasksCompleted = 0;

		public TaskSchedulerService()
		{
			MaxPassed = TimeSpan.FromSeconds(-5);
			_sync = new object();
			_taskListSorted = new SortedList<DateTime, TimerTask>();
		}

		public virtual void Start()
		{
			_mt = new Thread(Scheduler)
			{
				Name = "NewTimerScheduler",
				IsBackground = true,
				Priority = ThreadPriority.Normal
			};

			// important that this stays on the normal priority
			_mt.Start();
		}

		public virtual void Stop()
		{
			_taskListSorted.Clear();

			if (_mt != null)
			{
				_mt.Abort();
				_mt.Join(1000);
			}
		}

		public int TaskCount()
		{
			return _taskListSorted.Count;
		}

		/// <summary>
		/// Schedule a Task to run at specified time.
		/// </summary>
		/// <param name="runAt"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public TimerTask Add(TimeSpan runAt, Action action)
		{
			return Add(DateTime.UtcNow.Add(runAt), action);
		}

		/// <summary>
		/// Schedule a Task to run at specified time.
		/// </summary>
		/// <param name="milliseconds"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public TimerTask Add(double milliseconds, Action action)
		{
			return Add(DateTime.UtcNow.AddMilliseconds(milliseconds), action);
		}

		public RepeatingTimerTask AddRepeatingTask(Func<DateTime> action)
		{
			return Add(DateTime.UtcNow, action, new TimeSpan(0, 0, 30));
		}

		public RepeatingTimerTask AddRepeatingTask(Func<DateTime> action, TimeSpan initialDelay)
		{
			return Add(DateTime.UtcNow.Add(initialDelay), action, new TimeSpan(0, 0, 30));
		}

		/// <summary>
		/// Schedule a Task to run at specified time.
		/// </summary>
		/// <param name="runAt"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public TimerTask Add(DateTime runAt, Action action)
		{
			if (action == null)
				throw new ArgumentNullException("action");
			if ((runAt.Subtract(DateTime.UtcNow)) < MaxPassed)
				throw new InvalidOperationException("runAT is too far in the past.");

			TimerTask to = new TimerTask(action, runAt);

			Add(to);

			return to;
		}

		public RepeatingTimerTask Add(DateTime runAt, Func<DateTime> action, TimeSpan errorDelay)
		{
			if (action == null)
				throw new ArgumentNullException("action");
			if ((runAt.Subtract(DateTime.UtcNow)) < MaxPassed)
				throw new InvalidOperationException("runAT is too far in the past.");

			var to = new RepeatingTimerTask(action, runAt, errorDelay);

			Add(to);

			return to;
		}

		public void Add(TimerTask task)
		{
			lock (_sync)
			{
				while (_taskListSorted.ContainsKey(task.runDate))
				{
					task.runDate = task.runDate.AddSeconds(1);
				}
				_taskListSorted.Add(task.runDate, task);
			}
			WaitEvent.Set();
		}

		public void CheckTasksNow()
		{
			WaitEvent.Set();
		}

		/// <summary>
		/// Remove a TimerTask.
		/// </summary>
		/// <param name="timerTask">The TimerTask that was scheduled.</param>
		/// <returns>true if TimerTask has not been scheduled yet; otherwise false.</returns>
		public bool Remove(TimerTask timerTask)
		{
			if (timerTask == null)
				throw new ArgumentNullException("timerTask");

			lock (_sync)
			{
				int index = _taskListSorted.IndexOfValue(timerTask);
				if (index > -1)
				{
					_taskListSorted.RemoveAt(index);
					Interlocked.Exchange(ref timerTask.completed, 1);
					return true;
				}
				return false;
			}
		}

		private void ReaddRepeatingTask(RepeatingTimerTask task)
		{
			// reset the task
			task.Reset();
			Add(task);
		}

		/// <summary>
		/// Main scheduler thread. One thread is used to schedule queued tasks.
		/// Tasks are run on a ThreadPool thread when there start time triggers.
		/// </summary>
		private void Scheduler()
		{
			_lastRunTime = DateTime.UtcNow;

			while (true)
			{
				try
				{
					// if there are no tasks               
					while (_taskListSorted.Count == 0)
						WaitEvent.WaitOne();

					// No task is ready to run, so wait for timespan.
					TimeSpan waitTill = RunCurrentTasks();

					if (waitTill == TimeSpan.Zero || waitTill.TotalMilliseconds <= 0)
					{			
						continue;
					}

					DetectTimeShift();

					_lastRunTime = DateTime.UtcNow;

					// wait until the next event is scheduled
					WaitEvent.WaitOne((int)waitTill.TotalMilliseconds, false);
				}
				catch (ThreadAbortException ex)
				{
					throw ex;
				}
				catch (Exception ex)
				{
				}
			}
		}

		/// <summary>
		/// Check if a time shift has occured, this will be due to the NTP server or manual intervention
		/// </summary>
		private void DetectTimeShift()
		{
			//find the difference in time since the last run
			var difference = DateTime.UtcNow - _lastRunTime;

			if (Math.Abs(difference.TotalSeconds) > 60.0)
			{
				if (difference < TimeSpan.Zero)
				{
					//we went back in time, run all tasks once

					var currentTaskList = _taskListSorted.ToList();

					// remove all tasks, will readded as part of the execution
					_taskListSorted.Clear();

					foreach (var task in currentTaskList)
						ExecuteTimerTask(task.Value);
				}
				else if (difference > TimeSpan.Zero)
				{
					//we went forward in time
				}
			}
		}



		/// <summary>
		/// Runs any current tasks returns the time to wait until the next task
		/// </summary>
		/// <returns></returns>
		protected TimeSpan RunCurrentTasks()
		{
			// check we still have tasks to run
			while (_taskListSorted.Count > 0 && _taskListSorted.Values[0].RunDate <= DateTime.UtcNow)
			{
				TimerTask firstTask = null;

				lock (_sync)
				{
					// if there are no tasks to run
					if (_taskListSorted.Count == 0)
						return TimeSpan.Zero;

					firstTask = _taskListSorted.Values[0];

					// If time for first task to run, run it on a ThreadPool thread.
					if (firstTask.RunDate <= DateTime.UtcNow)
					{
						_taskListSorted.RemoveAt(0);
					}

					// if first has been canceled, remove it without running.
					if (firstTask.Completed)
					{
						Debug.WriteLine("Cancelled task found");
						firstTask = null;
					}
				}

				if (firstTask != null)
				{
					ExecuteTimerTask(firstTask);
				}
			}

			if (_taskListSorted.Count == 0)
				return TimeSpan.Zero;

			lock (_sync)
			{
				// determine the next run date
				return _taskListSorted.Values.First().RunDate.Subtract(DateTime.UtcNow);
			}
		}

		private void ExecuteTimerTask(TimerTask task)
		{
			try
			{
				task.Execute();

				Interlocked.Exchange(ref task.completed, 1);
				Interlocked.Increment(ref TasksCompleted);
			}
			catch (Exception ex)
			{
				Interlocked.Exchange(ref task.exception, ex);
			}

			if (task is RepeatingTimerTask)
				ReaddRepeatingTask(task as RepeatingTimerTask);
		}

		public void Dispose()
		{
			Stop();
		}
	}

}
