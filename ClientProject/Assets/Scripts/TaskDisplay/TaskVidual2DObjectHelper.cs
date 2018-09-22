using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskVidual2DObjectHelper : MonoBehaviour 
{
	public void PressButton()
	{
		Debug.LogWarning("string.Empty != m_LinkURL");
		if (string.Empty != m_LinkURL)
		{
			
		}
	}

	public void UpdateLinkURL( string url )
	{
		m_LinkURL = url;
	}
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
		m_LinkButton = UnityFind.ComponentFind<Button>(this.transform, "LinkButton");
		m_LinkButton.onClick.AddListener(delegate {PressButton();} );
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

	string m_LinkURL = string.Empty ;
	Button m_LinkButton = null ;
	Text m_Title = null ;
	bool m_Initialzied = false ;
}
