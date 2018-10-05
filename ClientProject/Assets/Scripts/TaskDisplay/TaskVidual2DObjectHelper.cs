﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskVidual2DObjectHelper : MonoBehaviour 
{
	
	public RectTransform SelfRect 
	{ 
		get {
			if (null == m_Self)
			{
				m_Self = this.GetComponent<RectTransform>();
			}
			return m_Self; 
		} 
	} 

	public void SetupParentPanel(TaskDisplayManager manager , int parentID , List<int> childrenIDList )
	{
		GameObject parentPanelObj = UnityFind.GameObjectFind(this.gameObject, "ParentPanel");
		if (null != parentPanelObj)
		{
			Task2DParentRegion parentPanel = parentPanelObj.GetComponent<Task2DParentRegion>();
			if (null == parentPanel)
			{
				parentPanel = parentPanelObj.AddComponent<Task2DParentRegion>();
			}

			m_ParentPanel = parentPanel;
			m_ParentPanel.SetupParentPanel(manager, parentID, childrenIDList);
		}
	}

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
		m_Self = this.GetComponent<RectTransform>();
		m_Title = UnityFind.ComponentFind<Text>(this.transform, "Title");
		m_Assignee = UnityFind.ComponentFind<Text>(this.transform, "Assignee");
		m_LinkButton = UnityFind.ComponentFind<Button>(this.transform, "LinkButton");
		if (null != m_LinkButton)
		{
			m_OpenBrowser = m_LinkButton.gameObject.AddComponent<OnClickOpenBrower>();
			m_LinkButton.onClick.AddListener(delegate {PressButton();} );
		}
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

	RectTransform m_Self ;
	string m_LinkURL = string.Empty ;
	Button m_LinkButton = null ;
	OnClickOpenBrower m_OpenBrowser =null; 
	Text m_Title = null ;
	Text m_Assignee = null ;
	bool m_Initialzied = false ;
	Task2DParentRegion m_ParentPanel = null;

}
