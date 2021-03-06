// Unity threading task classes
// Task.cs
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
using System.Threading;

namespace ThinksquirrelSoftware.Common.Threading
{
	/// <summary>
	/// Bass class for a task.
	/// </summary>
	/*! \author Original Unity Thread Helper classes (c) 2011 Marrrk (http://forum.unity3d.com/threads/90128-Unity-Threading-Helper)*/
    public abstract class TaskBase
    {
        private ManualResetEvent abortEvent = new ManualResetEvent(false);
        private ManualResetEvent endedEvent = new ManualResetEvent(false);
		private bool hasStarted = false;

		/*! \cond PRIVATE */	
		protected abstract void Do();
		/*! \endcond */	
		
		/// <summary>
		/// Returns true if the task should abort. If a Task should abort and has not yet been started
		/// it will never start but indicate an end and failed state.
		/// </summary>
        public bool ShouldAbort
        {
            get
			{
				bool wait = ThreadUtility.isWebPlayer ? ThreadUtility.WaitOne(abortEvent, 0) : abortEvent.WaitOne(0, false);
				return wait;
			}
        }

		/// <summary>
		/// Returns true when processing of this task has been ended or has been skipped due early abortion.
		/// </summary>
        public bool HasEnded
        {
            get 
			{
				bool wait = ThreadUtility.isWebPlayer ? ThreadUtility.WaitOne(endedEvent, 0) : endedEvent.WaitOne(0, false);
				return wait;
			}
        }

		/// <summary>
		/// Returns true when the task has successfully been processed. Tasks which throw exceptions will
		/// not be set to a failed state, also any exceptions will not be catched, the user needs to add
		/// checks for these kind of situation.
		/// </summary>
        public bool IsSucceeded
        {
            get
            {
				return HasEnded && !ShouldAbort;
            }
        }

		/// <summary>
		/// Returns true if the task should abort and has been ended. This value will not been set to true
		/// in case of an exception while processing this task. The user needs to add checks for these kind of situation.
		/// </summary>
        public bool IsFailed
        {
            get
            {
				return HasEnded && ShouldAbort;
            }
        }

		/// <summary>
		/// Notifies the task to abort and sets the task state to failed. The task needs to check ShouldAbort if the task should  abort.
		/// </summary>
        public void Abort()
        {
			abortEvent.Set();
        }

		/// <summary>
		/// Notifies the task to abort and sets the task state to failed. The task needs to check ShouldAbort if the task should abort.
		/// This method will wait until the task has been aborted/ended.
		/// </summary>
        public void AbortWait()
		{
			Abort();
			Wait();
        }

		/// <summary>
		/// Notifies the task to abort and sets the task state to failed. The task needs to check ShouldAbort if the task should abort.
		/// This method will wait until the task has been aborted/ended or the given timeout has been reached.
		/// </summary>
		/// <param name="seconds">Time in seconds this method will max wait.</param>
        public void AbortWaitForSeconds(float seconds)
        {
			Abort();
			WaitForSeconds(seconds);
        }

		/// <summary>
		/// Blocks the calling thread until the task has been ended.
		/// </summary>
        public void Wait()
        {
			endedEvent.WaitOne();
        }

		/// <summary>
		/// Blocks the calling thread until the task has been ended or the given timeout value has been reached.
		/// </summary>
		/// <param name="seconds">Time in seconds this method will max wait.</param>
        public void WaitForSeconds(float seconds)
        {
			if (ThreadUtility.isWebPlayer)
				ThreadUtility.WaitOne(endedEvent, TimeSpan.FromSeconds(seconds));
			else
				endedEvent.WaitOne(TimeSpan.FromSeconds(seconds), false);
        }

		/// <summary>
		/// Blocks the calling thread until the task has been ended and returns the return value of the task as the given type.
		/// Use this method only for Tasks with return values (functions)!
		/// </summary>
		/// <returns>The return value of the task as the given type.</returns>
		public abstract TResult Wait<TResult>();

		/// <summary>
		/// Blocks the calling thread until the task has been ended and returns the return value of the task as the given type.
		/// Use this method only for Tasks with return values (functions)!
		/// </summary>
		/// <param name="seconds">Time in seconds this method will max wait.</param>
		/// <returns>The return value of the task as the given type.</returns>
		public abstract TResult WaitForSeconds<TResult>(float seconds);

		/// <summary>
		/// Blocks the calling thread until the task has been ended and returns the return value of the task as the given type.
		/// Use this method only for Tasks with return values (functions)!
		/// </summary>
		/// <param name="seconds">Time in seconds this method will max wait.</param>
		/// <param name="defaultReturnValue">The default return value which will be returned when the task has failed.</param>
		/// <returns>The return value of the task as the given type.</returns>
		public abstract TResult WaitForSeconds<TResult>(float seconds, TResult defaultReturnValue);

        internal void DoInternal()
        {
			hasStarted = true;
			if (!ShouldAbort)
				Do();
            endedEvent.Set();
        }

		/// <summary>
		/// Disposes this task and waits for completion if its still running.
		/// </summary>
        public void Dispose()
        {
			if (hasStarted)
				Wait();
			endedEvent.Close();
			abortEvent.Close();
        }
    }
	
	/// <summary>
	/// A task to be performed by a dispatcher.
	/// </summary>
	/*! \author Original Unity Thread Helper classes (c) 2011 Marrrk (http://forum.unity3d.com/threads/90128-Unity-Threading-Helper)*/
    public class Task : TaskBase
    {
        private Action action;
		
        public Task(Action action)
        {
            this.action = action;
        }
		
		/*! \cond PRIVATE */
		protected override void Do()
        {
            action();
        }
		/*! \endcond */	
		
		public override TResult Wait<TResult>()
		{
			throw new InvalidOperationException("This task type does not support return values.");
		}

		public override TResult WaitForSeconds<TResult>(float seconds)
		{
			throw new InvalidOperationException("This task type does not support return values.");
		}

		public override TResult WaitForSeconds<TResult>(float seconds, TResult defaultReturnValue)
		{
			throw new InvalidOperationException("This task type does not support return values.");
		}
    }
	
	/// <summary>
	/// A task to be performed by a dispatcher.
	/// </summary>
	/*! \author Original Unity Thread Helper classes (c) 2011 Marrrk (http://forum.unity3d.com/threads/90128-Unity-Threading-Helper)*/
    public class Task<T> : TaskBase
    {
        private Func<T> function;
        private T result;

        public Task(Func<T> function)
        {
            this.function = function;
        }
		
		/*! \cond PRIVATE */
		protected override void Do()
        {
            result = function();
        }
		/*! \endcond */	
		
		public override TResult Wait<TResult>()
		{
			return (TResult)(object)Result;
		}

		public override TResult WaitForSeconds<TResult>(float seconds)
		{
			return WaitForSeconds(seconds, default(TResult));
		}

		public override TResult WaitForSeconds<TResult>(float seconds, TResult defaultReturnValue)
		{
			if (!HasEnded)
				WaitForSeconds(seconds);
			if (IsSucceeded)
				return (TResult)(object)result;
			return defaultReturnValue;
		}
        
		/// <summary>
		/// Waits till completion and returns the operation result.
		/// </summary>
        public T Result
        {
            get
            {
                if (!HasEnded)
                    Wait();
                return result;
            }
        }
    }
}
#endif