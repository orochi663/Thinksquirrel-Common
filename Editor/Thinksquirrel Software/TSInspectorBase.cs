using UnityEngine;
using UnityEditor;

namespace ThinksquirrelSoftware.Common
{

	public class TSInspectorBase : Editor
	{

		// Clamp Helper Methods
		protected float ClampPositiveZero(float val)
		{
			return Mathf.Clamp(val, 0, Mathf.Infinity);
		}

		protected float ClampPositive(float val)
		{
			return Mathf.Clamp(val, Mathf.Epsilon, Mathf.Infinity);
		}

		protected int ClampPositiveInt(int val)
		{
			return Mathf.Clamp(val, 1, int.MaxValue);
		}

		protected int ClampPositiveIntZero(int val)
		{
			return Mathf.Clamp(val, 0, int.MaxValue);
		}
	}
}
