using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProgressIntType : int 
{
	UnActive = 0 ,
	Assigned = 1 ,
	Confirmed = 2 ,
	InProgress = 3 ,
	Solved = 4 ,
	Validated = 5 ,
	Close = 6 ,
	OnHold = 7 ,
}

public static class ProgressIntTypeStringMapHelper
{
	public static void TryInit()
	{
		if (s_ProgressToStringMap.Count <= 0)
		{
			s_ProgressToStringMap.Add((int)ProgressIntType.UnActive, "UnActive");
			s_ProgressToStringMap.Add((int)ProgressIntType.Assigned, "Assigned");
			s_ProgressToStringMap.Add((int)ProgressIntType.Confirmed, "Confirmed");
			s_ProgressToStringMap.Add((int)ProgressIntType.InProgress, "InProgress");
			s_ProgressToStringMap.Add((int)ProgressIntType.Solved, "Solved");
			s_ProgressToStringMap.Add((int)ProgressIntType.Validated, "Validated");
			s_ProgressToStringMap.Add((int)ProgressIntType.Close, "Close");
			s_ProgressToStringMap.Add((int)ProgressIntType.OnHold, "OnHold");

		}
	}


	public static string GetString(int type)
	{

		TryInit();
		if (s_ProgressToStringMap.ContainsKey(type))
		{
			return s_ProgressToStringMap[type];
		}
		return "";
	}

	static Dictionary<int,string> s_ProgressToStringMap = new Dictionary<int, string>();
}
