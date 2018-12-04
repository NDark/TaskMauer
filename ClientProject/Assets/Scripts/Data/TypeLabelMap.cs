using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskType : int 
{
	Programming = 0 ,
	Art = 1 ,
	Design = 2 ,
}

public static class TaskTypeColorMapHelper
{
	public static void TryInit()
	{
		if (s_TypeToColorMap.Count <= 0)
		{
			s_TypeToColorMap.Add((int)TaskType.Programming, Color.blue);
			s_TypeToColorMap.Add((int)TaskType.Art, Color.red);
			s_TypeToColorMap.Add((int)TaskType.Design, Color.yellow);
		}
	}


	public static Color GetColor(int type)
	{
	
		TryInit();
		if (s_TypeToColorMap.ContainsKey(type))
		{
			return s_TypeToColorMap[type];
		}
		return Color.black;
	}

	static Dictionary<int,Color> s_TypeToColorMap = new Dictionary<int, Color>();
}
