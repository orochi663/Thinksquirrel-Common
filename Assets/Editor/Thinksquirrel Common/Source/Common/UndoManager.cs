// Unity undo manager class
// UndoManager.cs
// Thinksquirrel Software Common Libraries
//  
// Authors:
//		 Daniele Giardini <http://www.holoville.com>
//       Josh Montoute <josh@thinksquirrel.com>
// 
// Original code (c) 2011 Daniele Giardini
// Available at the Unify Community Wiki: http://www.unifycommunity.com/wiki/index.php?title=EditorUndoManager
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
using UnityEditor;
using UnityEngine;

namespace ThinksquirrelSoftware.Common.Editor
{
	/// <summary>
	/// Undo manager.
	/// To use it:
	/// <list type="number">
	/// <item>
	/// <description>Store an instance in the related Editor Class (instantiate it inside the <code>OnEnable</code> method).</description>
	/// </item>
	/// <item>
	/// <description>Call <code>undoManagerInstance.CheckUndo()</code> BEFORE the first UnityGUI call in <code>OnInspectorGUI</code>.</description>
	/// </item>
	/// <item>
	/// <description>Call <code>undoManagerInstance.CheckDirty()</code> AFTER the last UnityGUI call in <code>OnInspectorGUI</code>.</description>
	/// </item>
	/// </list>
	/// </summary>
	public class UndoManager
	{
	    // VARS ///////////////////////////////////////////////////
	
	    private     Object              defTarget;
	    private     string              defName;
	    private     bool                autoSetDirty;
	    private     bool                listeningForGuiChanges;
	    
	
	    // ***********************************************************************************
	    // CONSTRUCTOR
	    // ***********************************************************************************
	
	    /// <summary>
	    /// Creates a new HOEditorUndoManager,
	    /// setting it so that the target is marked as dirty each time a new undo is stored. 
	    /// </summary>
	    /// <param name="p_target">
	    /// The default <see cref="Object"/> you want to save undo info for.
	    /// </param>
	    /// <param name="p_name">
	    /// The default name of the thing to undo (displayed as "Undo [name]" in the main menu).
	    /// </param>
	    public UndoManager( Object p_target, string p_name ) : this( p_target, p_name, true ) {}
	    /// <summary>
	    /// Creates a new HOEditorUndoManager. 
	    /// </summary>
	    /// <param name="p_target">
	    /// The default <see cref="Object"/> you want to save undo info for.
	    /// </param>
	    /// <param name="p_name">
	    /// The default name of the thing to undo (displayed as "Undo [name]" in the main menu).
	    /// </param>
	    /// <param name="p_autoSetDirty">
	    /// If TRUE, marks the target as dirty each time a new undo is stored.
	    /// </param>
	    public UndoManager( Object p_target, string p_name, bool p_autoSetDirty )
	    {
	        defTarget = p_target;
	        defName = p_name;
	        autoSetDirty = p_autoSetDirty;
	    }
	
	    // ===================================================================================
	    // METHODS ---------------------------------------------------------------------------
	    
	    /// <summary>
	    /// Call this method BEFORE any undoable UnityGUI call.
	    /// Manages undo for the default target, with the default name.
	    /// </summary>
	    public void CheckUndo() { CheckUndo( defTarget, defName ); }
	    /// <summary>
	    /// Call this method BEFORE any undoable UnityGUI call.
	    /// Manages undo for the given target, with the default name.
	    /// </summary>
	    /// <param name="p_target">
	    /// The <see cref="Object"/> you want to save undo info for.
	    /// </param>
	    public void CheckUndo( Object p_target ) { CheckUndo( p_target, defName ); }
	    /// <summary>
	    /// Call this method BEFORE any undoable UnityGUI call.
	    /// Manages undo for the given target, with the given name.
	    /// </summary>
	    /// <param name="p_target">
	    /// The <see cref="Object"/> you want to save undo info for.
	    /// </param>
	    /// <param name="p_name">
	    /// The name of the thing to undo (displayed as "Undo [name]" in the main menu).
	    /// </param>
	    public void CheckUndo( Object p_target, string p_name )
	    {
	        Event e = Event.current;
	        
	        if ( ( e.type == EventType.MouseDown && e.button == 0 ) || ( e.type == EventType.KeyUp && e.keyCode == KeyCode.Tab ) ) {
	            // When the LMB is pressed or the TAB key is released,
	            // store a snapshot, but don't register it as an undo
	            // (so that if nothing changes we avoid storing a useless undo).
	            Undo.SetSnapshotTarget( p_target, p_name );
	            Undo.CreateSnapshot();
	            Undo.ClearSnapshotTarget(); // Not sure if this is necessary.
	            listeningForGuiChanges = true;
	        }
	    }
	    
	    /// <summary>
	    /// Call this method AFTER any undoable UnityGUI call.
	    /// Manages undo for the default target, with the default name.
	    /// </summary>
	    public void CheckDirty() { CheckDirty( defTarget, defName ); }
	    /// <summary>
	    /// Call this method AFTER any undoable UnityGUI call.
	    /// Manages undo for the given target, with the default name.
	    /// </summary>
	    /// <param name="p_target">
	    /// The <see cref="Object"/> you want to save undo info for.
	    /// </param>
	    public void CheckDirty( Object p_target ) { CheckDirty( p_target, defName ); }
	    /// <summary>
	    /// Call this method AFTER any undoable UnityGUI call.
	    /// Manages undo for the given target, with the given name.
	    /// </summary>
	    /// <param name="p_target">
	    /// The <see cref="Object"/> you want to save undo info for.
	    /// </param>
	    /// <param name="p_name">
	    /// The name of the thing to undo (displayed as "Undo [name]" in the main menu).
	    /// </param>
	    public void CheckDirty( Object p_target, string p_name )
	    {
	        if ( listeningForGuiChanges && GUI.changed ) {
	            // Some GUI value changed after pressing the mouse
	            // or releasing the TAB key.
	            // Register the previous snapshot as a valid undo.
	            Undo.SetSnapshotTarget( p_target, p_name );
	            Undo.RegisterSnapshot();
	            Undo.ClearSnapshotTarget(); // Not sure if this is necessary.
	            if ( autoSetDirty )     EditorUtility.SetDirty( p_target );
	            listeningForGuiChanges = false;
	        }
	    }
	}
}