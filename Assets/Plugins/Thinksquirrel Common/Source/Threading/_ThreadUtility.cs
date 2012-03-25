// Unity threading utility component
// _ThreadUtility.cs
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
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Contains threading classes.
/// </summary>
namespace ThinksquirrelSoftware.Common.Threading
{
	/// <summary>
	/// Thread helper component. Handles dispatchers and distributors and manages threads.
	/// </summary>
	/*! \author Original Unity Thread Helper classes (c) 2011 Marrrk (http://forum.unity3d.com/members/39278-Marrrk)*/
	[AddComponentMenu("")]
	public class ThreadUtility : MonoBase
	{
	    private static ThreadUtility instance = null;
	
	    public static void EnsureHelper()
	    {
	        if (null == (object)instance)
	        {
	            instance = FindObjectOfType(typeof(ThreadUtility)) as ThreadUtility;
	            if (null == (object)instance)
	            {
	                var go = new GameObject("[ThreadUtility]");
	                go.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy | HideFlags.HideInInspector;
	                instance = go.AddComponent<ThreadUtility>();
	                instance.EnsureHelperInstance();
	            }
	        }
	    }
	
	    private static ThreadUtility Instance
	    {
	        get
	        {
	            EnsureHelper();
	            return instance;
	        }
	    }
	
	    /// <summary>
	    /// Returns the GUI/Main Dispatcher.
	    /// </summary>
	    public static Dispatcher Dispatcher
	    {
	        get
	        {
	            return Instance.CurrentDispatcher;
	        }
	    }
	
	    /// <summary>
	    /// Returns the TaskDistributor.
	    /// </summary>
	    public static TaskDistributor TaskDistributor
	    {
	        get
	        {
	            return Instance.CurrentTaskDistributor;
	        }
	    }
	
	    private Dispatcher dispatcher;
	    public Dispatcher CurrentDispatcher
	    {
	        get
	        {
	            return dispatcher;
	        }
	    }
	
	    private TaskDistributor taskDistributor;
	    public TaskDistributor CurrentTaskDistributor
	    {
	        get
	        {
	            return taskDistributor;
	        }
	    }
	
	    private void EnsureHelperInstance()
	    {
	        if (dispatcher == null)
	            dispatcher = new Dispatcher();
	
	        if (taskDistributor == null)
	            taskDistributor = new TaskDistributor();
	    }
	
	    /// <summary>
	    /// Creates new thread which runs the given action. The given action will be wrapped so that any exception will be catched and logged.
	    /// </summary>
	    /// <param name="action">The action which the new thread should run.</param>
	    /// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
	    /// <returns>The instance of the created thread class.</returns>
	    public static ActionThread CreateThread(System.Action<ActionThread> action, bool autoStartThread)
	    {
	        Instance.EnsureHelperInstance();
	
	        System.Action<ActionThread> actionWrapper = currentThread =>
	            {
	                try
	                {
	                    action(currentThread);
	                }
	                catch (System.Exception ex)
	                {
	                    UnityEngine.Debug.LogError(ex);
	                }
	            };
	        var thread = new ActionThread(actionWrapper, autoStartThread);
	        Instance.RegisterThread(thread);
	        return thread;
	    }
	
	    /// <summary>
	    /// Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so that any exception will be catched and logged.
	    /// </summary>
	    /// <param name="action">The action which the new thread should run.</param>
	    /// <returns>The instance of the created thread class.</returns>
	    public static ActionThread CreateThread(System.Action<ActionThread> action)
	    {
	        return CreateThread(action, true);
	    }
	
	    /// <summary>
	    /// Creates new thread which runs the given action. The given action will be wrapped so that any exception will be catched and logged.
	    /// </summary>
	    /// <param name="action">The action which the new thread should run.</param>
	    /// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
	    /// <returns>The instance of the created thread class.</returns>
	    public static ActionThread CreateThread(System.Action action, bool autoStartThread)
	    {
	        return CreateThread((thread) => action(), autoStartThread);
	    }
	
	    /// <summary>
	    /// Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so that any exception will be catched and logged.
	    /// </summary>
	    /// <param name="action">The action which the new thread should run.</param>
	    /// <returns>The instance of the created thread class.</returns>
	    public static ActionThread CreateThread(System.Action action)
	    {
	        return CreateThread((thread) => action(), true);
	    }
	
	    #region Enumeratable
	
	    /// <summary>
	    /// Creates new thread which runs the given action. The given action will be wrapped so that any exception will be catched and logged.
	    /// </summary>
	    /// <param name="action">The enumeratable action which the new thread should run.</param>
	    /// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
	    /// <returns>The instance of the created thread class.</returns>
	    public static ThreadBase CreateThread(System.Func<ThreadBase, IEnumerator> action, bool autoStartThread)
	    {
	        Instance.EnsureHelperInstance();
	
	        var thread = new EnumeratableActionThread(action, autoStartThread);
	        Instance.RegisterThread(thread);
	        return thread;
	    }
	
	    /// <summary>
	    /// Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so that any exception will be catched and logged.
	    /// </summary>
	    /// <param name="action">The enumeratable action which the new thread should run.</param>
	    /// <returns>The instance of the created thread class.</returns>
	    public static ThreadBase CreateThread(System.Func<ThreadBase, IEnumerator> action)
	    {
	        return CreateThread(action, true);
	    }
	
	    /// <summary>
	    /// Creates new thread which runs the given action. The given action will be wrapped so that any exception will be catched and logged.
	    /// </summary>
	    /// <param name="action">The enumeratable action which the new thread should run.</param>
	    /// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
	    /// <returns>The instance of the created thread class.</returns>
	    public static ThreadBase CreateThread(System.Func<IEnumerator> action, bool autoStartThread)
	    {
	        System.Func<ThreadBase, IEnumerator> wrappedAction = (thread) => { return action(); };
	        return CreateThread(wrappedAction, autoStartThread);
	    }
	
	    /// <summary>
	    /// Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so that any exception will be catched and logged.
	    /// </summary>
	    /// <param name="action">The action which the new thread should run.</param>
	    /// <returns>The instance of the created thread class.</returns>
	    public static ThreadBase CreateThread(System.Func<IEnumerator> action)
	    {
	        System.Func<ThreadBase, IEnumerator> wrappedAction = (thread) => { return action(); };
	        return CreateThread(wrappedAction, true);
	    }
	
	    #endregion
	
	    List<ThreadBase> registeredThreads = new List<ThreadBase>();
	    public void RegisterThread(ThreadBase thread)
	    {
	        if (registeredThreads.Contains(thread))
	        {
	            return;
	        }
	
	        registeredThreads.Add(thread);
	    }
	
	    void OnDestroy()
	    {
	        foreach (var thread in registeredThreads)
	            thread.Dispose();
	
	        if (dispatcher != null)
	            dispatcher.Dispose();
	        dispatcher = null;
	
	        if (taskDistributor != null)
	            taskDistributor.Dispose();
	        taskDistributor = null;
	    }
	
	    void Update()
	    {
	        if (dispatcher != null)
	            dispatcher.ProcessTasks();
	
	        var finishedThreads = registeredThreads.Where(thread => !thread.IsAlive).ToArray();
	        foreach (var finishedThread in finishedThreads)
	        {
	            finishedThread.Dispose();
	            registeredThreads.Remove(finishedThread);
	        }
	    }
	}
}
#endif