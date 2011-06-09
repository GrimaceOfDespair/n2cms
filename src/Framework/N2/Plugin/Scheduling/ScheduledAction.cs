﻿using System;
using N2.Engine;
using N2.Web;

namespace N2.Plugin.Scheduling
{
    /// <summary>
    /// Base class for actions that can be scheduled to be executed by the 
    /// system at certain intervals. Inherit from this class and decorate using 
	/// the <see cref="ScheduleExecutionAttribute"/> to enable.
    /// </summary>
    public abstract class ScheduledAction
    {
		/// <summary>The method that executes the action. Implement in a subclass.</summary>
        public abstract void Execute();

        private TimeSpan interval = new TimeSpan(0, 1, 0);
        private DateTime? lastExecuted;
        private Repeat repeat = Repeat.Once;
        private bool isExecuting = false;
        private int errorCount = 0;

		/// <summary>The engine ivoking this action.</summary>
		public IEngine Engine { get; set; }

		/// <summary>The number of consecutive times this action has failed.</summary>
		public int ErrorCount
        {
            get { return errorCount; }
            set { errorCount = value; }
        }

		/// <summary>Whether the action is currently executing.</summary>
		public bool IsExecuting
        {
            get { return isExecuting; }
            set { isExecuting = value; }
        }

		/// <summary>The interval before next execution.</summary>
		public TimeSpan Interval
        {
            get { return interval; }
            set { interval = value; }
        }

		/// <summary>When the action was last executed.</summary>
		public DateTime? LastExecuted
        {
            get { return lastExecuted; }
            set { lastExecuted = value; }
        }

		/// <summary>Wheter the action should run again.</summary>
		public Repeat Repeat
        {
            get { return repeat; }
            set { repeat = value; }
        }

		/// <summary>Examines the properties to determine whether the action should run.</summary>
		public virtual bool ShouldExecute()
        {
            return !IsExecuting && (!LastExecuted.HasValue || LastExecuted.Value.Add(Interval) < Utility.CurrentTime());
        }

        /// <summary>
        /// This method will be called when error occured in the action's Execute() method. 
        /// It can be overrided to write custom error handling routine. 
        /// The default behavior is to call IErrorHandler.Notify() with the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public virtual void OnError(Exception ex)
        {
            if(Engine != null)
                Engine.Resolve<IErrorNotifier>().Notify(ex);
        }
    }
}
