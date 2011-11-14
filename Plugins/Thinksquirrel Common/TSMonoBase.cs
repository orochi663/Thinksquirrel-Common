using UnityEngine;

/// <summary>
/// Thinksquirrel Software common libraries.
/// </summary>
namespace ThinksquirrelSoftware.Common
{
	/// <summary>
	/// Monobehavior base class for all Thinksquirrel Software components. 
	/// </summary>
	/*! \author Original code for TSMonoBase.cs (c) 2008 Neil Carter (http://www.nether.org.uk/)*/
	public abstract class TSMonoBase : MonoBehaviour
	{
		/// <summary>
		/// Generic FindObjectOfType() implementation.
		/// </summary>
	    public static T FindObjectOfType<T>() where T : Object
	        {return (T)Object.FindObjectOfType(typeof(T));}
		
		/// <summary>
		/// Generic FindObjectsOfType() implementation.
		/// </summary>
	    public static T[] FindObjectsOfType<T>() where T : Object
	        {return ConvertObjectArray<T>(Object.FindObjectsOfType(typeof(T)));}
		
		/// <summary>
		/// Object array conversion. 
		/// </summary>
	    public static T[] ConvertObjectArray<T>(Object[] objects) where T : Object
	        {return System.Array.ConvertAll<Object, T>(objects, delegate(Object o) {return (T)o;});}
	
		/// <summary>
		/// Searches for the component on this GameObject and every parent until it hits the root.
		/// </summary>
	    public T GetComponentUpwards<T>() where T : Component
	        {return GetComponentUpwardsFrom<T>(gameObject);}
		
		/// <summary>
		/// Searches for the component on this GameObject and every parent until it hits the root.
		/// </summary>
	    public static T GetComponentUpwardsFrom<T>(GameObject g) where T : Component
	    {
	        T component;
	
	        Transform t = g.transform;
	        do
	        {
	            component = t.GetComponent<T>();
	
	            if(component != null)
	                break;
	
	            t = t.parent;
	        }
	        while(t != null);
	
	        return component;
	    }
	
	    /// <summary>
		/// Searches for the component on this GameObject and every parent until it hits the root.
		/// </summary>
		public static T GetComponentUpwardsFrom<T>(Component c) where T : Component
	        {return GetComponentUpwardsFrom<T>(c.gameObject);}
	
		/// <summary>
		/// Generic Instantiate() implementation.
		/// </summary>
	    public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
	    {
	        return (T)MonoBehaviour.Instantiate(original, position, rotation);
	    }
		
		/// <summary>
		/// Generic Instantiate() implementation.
		/// </summary>
	    public static T Instantiate<T>(T original) where T : Object
	    {
	        return (T)MonoBehaviour.Instantiate(original);
	    }
	}
}
