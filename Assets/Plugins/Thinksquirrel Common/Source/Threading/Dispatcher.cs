// Unity threading dispatcher
// Dispatcher.cs
// Thinksquirrel Software Common Libraries
//  
// Author:
//       Josh Montoute <josh@thinksquirrel.com>
// 
// Copyright (c) 2011-2012, Thinksquirrel Software, LLC
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
// Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT 
// SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT 
// OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// This file is available at https://github.com/Thinksquirrel-Software/Thinksquirrel-Common
//
#if !COMPACT
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;

namespace ThinksquirrelSoftware.Common.Threading
{
	/// <summary>
	/// Base class for a dispatcher.
	/// </summary>
	/*! \author Original Unity Thread Helper classes (c) 2011 Marrrk (http://forum.unity3d.com/threads/90128-Unity-Threading-Helper)*/
    public abstract class DispatcherBase : IDisposable
    {
		protected Queue<TaskBase> taskQueue = new Queue<TaskBase>();
        protected ManualResetEvent dataEvent = new ManualResetEvent(false);

		public DispatcherBase()
		{
		}

		/// <summary>
		/// Returns the currently existing task count. Early aborted tasks will count too.
		/// </summary>
        public int TaskCount
        {
            get
            {
                lock (taskQueue)
                    return taskQueue.Count;
            }
        }

		/// <summary>
		/// Creates a new Task based upon the given action.
		/// </summary>
		/// <typeparam name="T">The return value of the task.</typeparam>
		/// <param name="function">The function to process at the dispatchers thread.</param>
		/// <returns>The new task.</returns>
		public Task<T> Dispatch<T>(Func<T> function)
        {
			CheckAccessLimitation();

            var task = new Task<T>(function);
            AddTask(task);
            return task;
        }

		/// <summary>
		/// Creates a new Task based upon the given action.
		/// </summary>
		/// <param name="action">The action to process at the dispatchers thread.</param>
		/// <returns>The new task.</returns>
        public Task Dispatch(Action action)
        {
			CheckAccessLimitation();

            var task = new Task(action);
            AddTask(task);
            return task;
        }

		internal void AddTask(TaskBase task)
        {
            lock (taskQueue)
                taskQueue.Enqueue(task);
			dataEvent.Set();
        }

        internal void AddTasks(IEnumerable<TaskBase> tasks)
        {
            lock (taskQueue)
				foreach (var task in tasks)
					taskQueue.Enqueue(task);
			dataEvent.Set();
        }

		internal IEnumerable<TaskBase> SplitTasks(int divisor)
        {
			if (divisor == 0)
				divisor = 2;
			var count = TaskCount / divisor;
            return IsolateTasks(count);
        }

        internal IEnumerable<TaskBase> IsolateTasks(int count)
        {
			List<TaskBase> newTasks = new List<TaskBase>();

			if (count == 0)
				count = taskQueue.Count;

            lock (taskQueue)
            {
				for (int i = 0; i < count && taskQueue.Count != 0; ++i)
					newTasks.Add(taskQueue.Dequeue());
            }

			if (TaskCount == 0)
				dataEvent.Reset();

			return newTasks;
        }

		protected abstract void CheckAccessLimitation();

        #region IDisposable Members

        public virtual void Dispose()
        {
			while (true)
			{
				TaskBase currentTask;
				lock (taskQueue)
				{
					if (taskQueue.Count != 0)
						currentTask = taskQueue.Dequeue();
					else
						break;
				}
				currentTask.Dispose();
			}

			dataEvent.Close();
			dataEvent = null;
        }

        #endregion
    }
	
	/// <summary>
	/// Performs tasks on threads.
	/// </summary>
	/*! \author Original Unity Thread Helper classes (c) 2011 Marrrk (http://forum.unity3d.com/threads/90128-Unity-Threading-Helper)*/
    public class Dispatcher : DispatcherBase
    {
        [ThreadStatic]
        private static TaskBase currentTask;

        [ThreadStatic]
        internal static Dispatcher currentDispatcher;
        
        protected static Dispatcher mainDispatcher;

		/// <summary>
		/// Returns the task which is currently being processed. Use this only inside a task operation.
		/// </summary>
        public static TaskBase CurrentTask
        {
            get
            {
                if (currentTask == null)
                    throw new InvalidOperationException("No task is currently running.");

                return currentTask;
            }
        }

		/// <summary>
		/// Returns the Dispatcher instance of the current thread. When no instance has been created an exception will be thrown.
		/// </summary>
        public static Dispatcher Current
        {
            get
			{
				if (currentDispatcher == null)
					throw new InvalidOperationException("No Dispatcher found for the current thread, please create a new Dispatcher instance before calling this property.");
				return currentDispatcher; 
			}
            set
            {
                if (currentDispatcher != null)
                    currentDispatcher.Dispose();
                currentDispatcher = value;
            }
        }

		/// <summary>
		/// Returns the first created Dispatcher instance, in most cases this will be the Dispatcher for the main thread. When no instance has been created an exception will be thrown.
		/// </summary>
        public static Dispatcher Main
        {
            get
            {
				if (mainDispatcher == null)
					throw new InvalidOperationException("No Dispatcher found for the main thread, please create a new Dispatcher instance before calling this property.");

                return mainDispatcher;
            }
        }

		/// <summary>
		/// Creates a new function based upon an other function which will handle exceptions. Use this to wrap safe functions for tasks.
		/// </summary>
		/// <typeparam name="T">The return type of the function.</typeparam>
		/// <param name="function">The orignal function.</param>
		/// <returns>The safe function.</returns>
		public static Func<T> CreateSafeFunction<T>(Func<T> function)
		{
			return () =>
				{
					try
					{
						return function();
					}
					catch
					{
						CurrentTask.Abort();
						return default(T);
					}
				};
		}

		/// <summary>
        /// Creates a new action based upon an other action which will handle exceptions. Use this to wrap safe action for tasks.
		/// </summary>
		/// <param name="function">The orignal action.</param>
		/// <returns>The safe action.</returns>
		public static Action CreateSafeAction<T>(Action action)
		{
			return () =>
			{
				try
				{
					action();
				}
				catch
				{
					CurrentTask.Abort();
				}
			};
		}

		/// <summary>
		/// Creates a Dispatcher, if a Dispatcher has been created in the current thread an exception will be thrown.
		/// </summary>
		public Dispatcher()
			: this(true)
		{
		}

		internal Dispatcher(bool setThreadDefaults)
        {
			if (!setThreadDefaults)
				return;

            if (currentDispatcher != null)
				throw new InvalidOperationException("Only one Dispatcher instance allowed per thread.");

			currentDispatcher = this;

            if (mainDispatcher == null)
                mainDispatcher = this;
        }

		/// <summary>
		/// Processes all remaining tasks. Call this periodically to allow the Dispatcher to handle dispatched tasks.
        /// Only call this inside the thread you want the tasks to process to be processed.
		/// </summary>
        public void ProcessTasks()
        {
			bool wait = ThreadUtility.isWebPlayer ? ThreadUtility.WaitOne(dataEvent, 0) : dataEvent.WaitOne(0, false);
			if (wait)
				ProcessTasksInternal();
        }

		/// <summary>
		/// Processes all remaining tasks and returns true when something has been processed and false otherwise.
		/// This method will block until th exitHandle has been set or tasks should be processed.
        /// Only call this inside the thread you want the tasks to process to be processed.
		/// </summary>
		/// <param name="exitHandle">The handle to indicate an early abort of the wait process.</param>
		/// <returns>False when the exitHandle has been set, true otherwise.</returns>
        public bool ProcessTasks(WaitHandle exitHandle)
        {
            var result = WaitHandle.WaitAny(new WaitHandle[] { exitHandle, dataEvent });
            if (result == 0)
                return false;
            ProcessTasksInternal();
            return true;
        }

		/// <summary>
		/// Processed the next available task.
        /// Only call this inside the thread you want the tasks to process to be processed.
		/// </summary>
		/// <returns>True when a task to process has been processed, false otherwise.</returns>
        public bool ProcessNextTask()
        {
            lock (taskQueue)
            {
                if (taskQueue.Count == 0)
                    return false;
                else
                    ProcessTask();
            }

			if (TaskCount == 0)
				dataEvent.Reset();

            return true;
        }

		/// <summary>
		/// Processes the next available tasks and returns true when it has been processed and false otherwise.
		/// This method will block until th exitHandle has been set or a task should be processed.
        /// Only call this inside the thread you want the tasks to process to be processed.
		/// </summary>
		/// <param name="exitHandle">The handle to indicate an early abort of the wait process.</param>
		/// <returns>False when the exitHandle has been set, true otherwise.</returns>
        public bool ProcessNextTask(WaitHandle exitHandle)
        {
            var result = WaitHandle.WaitAny(new WaitHandle[] { exitHandle, dataEvent });
            if (result == 0)
                return false;

            lock (taskQueue)
                ProcessTask();
			if (TaskCount == 0)
				dataEvent.Reset();
            return true;
        }

        private void ProcessTasksInternal()
        {
            lock (taskQueue)
            {
                while (taskQueue.Count != 0)
                    ProcessTask();
			}

			if (TaskCount == 0)
				dataEvent.Reset();
		}

        private void ProcessTask()
        {
            if (taskQueue.Count == 0)
                return;
			RunTask(taskQueue.Dequeue());
        }

		internal void RunTask(TaskBase task)
		{
			var oldTask = currentTask;
			currentTask = task;
			currentTask.DoInternal();
			currentTask = oldTask;
		}

		protected override void CheckAccessLimitation()
		{
			if (currentDispatcher == this)
				throw new InvalidOperationException("Dispatching a Task with the Dispatcher associated to the current thread is prohibited. You can run these Tasks without the need of a Dispatcher.");
		}

        #region IDisposable Members

		/// <summary>
		/// Disposes all dispatcher resources and remaining tasks.
		/// </summary>
        public override void Dispose()
        {
			while (true)
			{
				lock (taskQueue)
				{
					if (taskQueue.Count != 0)
						currentTask = taskQueue.Dequeue();
					else
						break;
				}
				currentTask.Dispose();
			}

			dataEvent.Close();
			dataEvent = null;

			if (currentDispatcher == this)
				currentDispatcher = null;
			if (mainDispatcher == this)
				mainDispatcher = null;
        }

        #endregion
    }
}
#endif