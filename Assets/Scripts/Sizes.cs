using UnityEngine;
using m = UnityEngine.Mathf;

class Sizes
{
	public class Physics
	{
		public static float raycastDistance = 10000;
	}

	public class UI
	{
		public static float pixelsPerUnit = 100;
		public static Vector3 localScale = new Vector3(0.5f, 0.5f, 1);
	}

	public static class InpuwWindowData
	{
		public static float nameFotnSize = 17;
		public static float contextFontSize = 13;
	}

	public static class Notebook
	{
		public static float minPointLineThreshold = .005f;
		public static float maxPointLineTreshold = .05f;
		public static float offsetFromWall = .001f;
		public static float textPanelColliderDepth = 100f;
	}

	public class BuildingParameters
	{
		public static float blockWidth = .1f;
		public static float heightMiliDelta = 0.001f;
	}

	public class Walls
	{
		public static Vector3 Size
		{
			get
			{
				return new Vector3(1, 1 / Mathf.Sqrt(2), BuildingParameters.blockWidth);
			}
		}
	}
}