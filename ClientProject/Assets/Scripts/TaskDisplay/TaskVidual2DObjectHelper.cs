using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskVidual2DObjectHelper : MonoBehaviour 
{
	public void UpdateTitle( string content )
	{
		if (m_Title)
		{
			m_Title.text = content;
		}
	}

	public void Setup()
	{
		m_Title = UnityFind.ComponentFind<Text>(this.transform, "Title");
			
		m_Initialzied = true;
	}

	// Use this for initialization
	void Start () 
	{
		if (false == m_Initialzied)
		{
			Setup();
		}
		
	}
	

	Text m_Title ;
	bool m_Initialzied = false ;
}
