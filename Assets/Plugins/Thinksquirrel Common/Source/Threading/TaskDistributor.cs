// Unity threading task distributor classes
// TaskDistributor.cs
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
	/// Task distributor. Dispatches multiple background tasks.
	/// </summary>
	/*! \author Original Unity Thread Helper classes (c) 2011 Marrrk (http://forum.unity3d.com/threads/90128-Unity-Threading-Helper)*/
    public class TaskDistributor : DispatcherBase
	{
        private TaskWorker[] workerThreads;

        internal WaitHandle NewDataWaitHandle { get { return dataEvent; } }

		private static TaskDistributor mainTaskDistributor;

		/// <summary>
		/// Returns the first created TaskDistributor instance. When no instance has been created an exception will be thrown.
		/// </summary>
		public static TaskDistributor Main
		{
			get
			{
				if (mainTaskDistributor == null)
					throw new InvalidOperationException("No default TaskDistributor found, please create a new TaskDistributor instance before calling this property.");

				return mainTaskDistributor;
			}
		}

		/// <summary>
		/// Creates a new instance of the TaskDistributor with ProcessorCount x3 worker threads.
		/// The task distributor will auto start his worker threads.
		/// </summary>
        public TaskDistributor()
			: this(0)
        {
        }

		/// <summary>
		/// Creates a new instance of the TaskDistributor.
		/// The task distributor will auto start his worker threads.
		/// </summary>
		/// <param name="workerThreadCount">The number of worker threads, a value below one will create ProcessorCount x3 worker threads.</param>
		public TaskDistributor(int workerThreadCount)
			: this(workerThreadCount, true)
		{
		}

		/// <summary>
		/// Creates a new instance of the TaskDistributor.
		/// </summary>
        /// <param name="workerThreadCount">The number of worker threads, a value below one will create ProcessorCount x3 worker threads.</param>
		/// <param name="autoStart">Should the instance auto start the worker threads.</param>
		public TaskDistributor(int workerThreadCount, bool autoStart)
			: base()
		{
			if (workerThreadCount <= 0)
				workerThreadCount = UnityEngine.SystemInfo.processorCount * 3;

			workerThreads = new TaskWorker[workerThreadCount];
			lock (workerThreads)
			{
				for (var i = 0; i < workerThreadCount; ++i)
					workerThreads[i] = new TaskWorker(this);
			}

			if (mainTaskDistributor == null)
				mainTaskDistributor = this;

			if (autoStart)
				Start();
		}

		/// <summary>
		/// Starts the TaskDistributor if its not currently running.
		/// </summary>
		public void Start()
		{
			lock (workerThreads)
			{
				for (var i = 0; i < workerThreads.Length; ++i)
				{
					if (!workerThreads[i].IsAlive)
					{
						workerThreads[i].Dispatcher.AddTasks(this.SplitTasks(workerThreads.Length));
						workerThreads[i].Start();
					}
				}
			}
		}

        internal void FillTasks(Dispatcher target)
        {
			target.AddTasks(this.IsolateTasks(1));
		}

		protected override void CheckAccessLimitation()
		{
			if (ThreadBase.CurrentThread != null &&
				ThreadBase.CurrentThread is TaskWorker &&
				((TaskWorker)ThreadBase.CurrentThread).TaskDistributor == this)
			{
				throw new InvalidOperationException("Access to TaskDistributor prohibited when called from inside a TaskDistributor thread. Dont dispatch new Tasks through the same TaskDistributor. If you want to distribute new tasks create a new TaskDistributor and use the new created instance. Remember to dispose the new instance to prevent thread spamming.");
			}
		}

        #region IDisposable Members

		/// <summary>
		/// Disposes all TaskDistributor, worker threads, resources and remaining tasks.
		/// </summary>
        public override void Dispose()
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

			lock (workerThreads)
			{
				for (var i = 0; i < workerThreads.Length; ++i)
					workerThreads[i].Dispose();
				workerThreads = new TaskWorker[0];
			}

			dataEvent.Close();
			dataEvent = null;

			if (mainTaskDistributor == this)
				mainTaskDistributor = null;
        }

        #endregion
    }
/*! \cond PRIVATE */
    internal class TaskWorker : ThreadBase
    {
		public Dispatcher Dispatcher;
		public TaskDistributor TaskDistributor;

		public TaskWorker(TaskDistributor taskDistributor)
            : base(false)
        {
			this.TaskDistributor = taskDistributor;
			this.Dispatcher = new Dispatcher(false);
		}
		
		private bool CheckExitEvent()
		{
			if (ThreadUtility.isWebPlayer)
				return ThreadUtility.WaitOne(exitEvent, 0);
			else
				return exitEvent.WaitOne(0, false);
		}
        
		protected override void Do()
        {
            while (!CheckExitEvent())
            {
                if (!Dispatcher.ProcessNextTask())
                {
					TaskDistributor.FillTasks(Dispatcher);
                    if (Dispatcher.TaskCount == 0)
                    {
						var result = WaitHandle.WaitAny(new WaitHandle[] { exitEvent, TaskDistributor.NewDataWaitHandle });
                        if (result == 0)
                            return;
						TaskDistributor.FillTasks(Dispatcher);
                    }
                }
            }
        }
/*! \endcond */
        public override void Dispose()
        {
            base.Dispose();
			if (Dispatcher != null)
				Dispatcher.Dispose();
			Dispatcher = null;
        }
    }
}
#endif