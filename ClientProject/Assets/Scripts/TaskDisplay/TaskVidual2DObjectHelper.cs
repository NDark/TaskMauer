using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskVidual2DObjectHelper : MonoBehaviour 
{
	public void PressButton()
	{
		if (string.Empty != m_LinkURL)
		{
			Debug.LogWarning("m_LinkURL"+m_LinkURL);
			m_OpenBrowser.m_Url = m_LinkURL;
			m_OpenBrowser.OpenBrower();
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

	public void UpdateAssignee( string content )
	{
		if (m_Assignee)
		{
			m_Assignee.text = content;
		}
	}

	public void Setup()
	{
		m_Title = UnityFind.ComponentFind<Text>(this.transform, "Title");
		m_Assignee = UnityFind.ComponentFind<Text>(this.transform, "Assignee");
		m_LinkButton = UnityFind.ComponentFind<Button>(this.transform, "LinkButton");
		m_OpenBrowser = m_LinkButton.gameObject.AddComponent<OnClickOpenBrower>();
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
	OnClickOpenBrower m_OpenBrowser =null; 
	Text m_Title = null ;
	Text m_Assignee = null ;
	bool m_Initialzied = false ;
}
