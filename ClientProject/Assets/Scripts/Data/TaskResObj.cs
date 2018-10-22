
[System.Serializable]
public class TaskUpdateResponse 
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
}

[System.Serializable]
public class TaskAddRequest : TaskUpdateRequestBase
{
	public TaskBundle Task = null ;
}
