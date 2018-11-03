using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TaskMauerStaticData
{
	public static int s_UpdateSerial = 0 ;

	public static int GetUpdateSerial()
	{
		return s_UpdateSerial;
	}

	public static void SetUpdateSerial( int set )
	{
		Debug.Log("SetUpdateSerial() set=" + set);
		s_UpdateSerial = set ;
	}
}
