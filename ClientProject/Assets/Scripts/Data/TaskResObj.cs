
[System.Serializable]
public class TasksResponse 
{
	public int UpdateSerial = 0 ;
	public int RequestSerial = 0 ;
	public TaskBundle[] TaskVec = null ;
}


[System.Serializable]
public class TaskUpdateRequestBase
{
	public int UpdateSerial = 0 ;
	public int RequestSerial = 0 ;
	public string ProjectKey = "" ;
}

[System.Serializable]
public class TaskAddRequest : TaskUpdateRequestBase
{
	public TaskBundle Task = null ;
}
