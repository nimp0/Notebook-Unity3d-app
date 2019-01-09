using UnityEngine;
using m = UnityEngine.Mathf;

class Sizes : MonoBehaviour
{
	public static class Graph
	{
		public static float nodeDistanceMultiplier = 4;

		public static float DistanceFunctionTotal(int x, int maxX)
		{
			float totalD = 0;

			for(int i = 0; i < x; i++)
			{
				totalD += DistanceFunction(i + 1, maxX);
			}

			return totalD;
		}

		public static float DistanceFunction(int x, int maxX)
		{
			return m.Log(maxX + 3 - x);
		} 
	}

	public static class Map
	{
		public static float minimumDistance = Graph.nodeDistanceMultiplier;
	}

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

	public class Ideas
	{
		public static float sizeMultiplier = 2;
		public static Vector2 sizeNameGUI = new Vector2(1, .5f);
		//public static float HitForceMultiplier = 222f;
		public static float movingHeightOffset = 2;
	}

	public class BuildingParameters
	{
		public static float blockWidth = .1f;
		public static float heightMiliDelta = 0.001f;
	}

	public class Corridors
	{
		////public static int orderPointsAmount = 4;
		//public static int order = 4;
		//public static int pointsAmount = 100;
		////public static float maxRadius = 30;
		////public static Vector3 randomizeVector = new Vector3(1,0.2f,1);
		//public static Vector2 size = new Vector3(Doors.w, Doors.h);
		//public static float windowHeight = Doors.h / 3;
		//public static float windowBaseHeight = Doors.h / 3;
		//public static float trackHeight = 1;
		//public static float lineWidth
		//{
		//	get
		//	{
		//		return Doors.h;
		//	}
		//}
		//public static float lineEndWidthMultiplier = 1f;
		public static int elementsAmount = 22;
	}

	public class Rooms
	{
		public static float w = 35;
		public static float h = 21;
		public static float l = 35;
		public static float x = 20;

		public static Vector3 Size
		{
			get
			{
				return new Vector3(w, h, BuildingParameters.blockWidth);
			}
		}
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

	//public class Sides
	//{
	//	public class Sections
	//	{
	//		public static float wallMinWidth = Doors.w * 1.5f;
	//		public static float deltaDeepness
	//		{
	//			get
	//			{
	//				return wallMinWidth/ 2;
	//			}
	//		}
	//	}
	//}	

	//public class Doors
	//{
	//	public static float w = 2;
	//	public static float h = 4;

	//	public static float DoorWallWidth
	//	{
	//		get
	//		{
	//			return w * 29.7f / 21;
	//		}
	//	}

	//	public static Vector2 Size
	//	{
	//		get
	//		{
	//			return new Vector3(w, h);
	//		}
	//	}

	//	public static float doorOutlineThicness
	//	{
	//		get
	//		{
	//			return h / 3;
	//		}
	//	}
	//}
}