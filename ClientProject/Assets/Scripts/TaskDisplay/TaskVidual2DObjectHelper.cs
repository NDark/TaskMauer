using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskVidual2DObjectHelper : MonoBehaviour 
{
	public System.Action OnPressSwitchButton = new System.Action( () => {} );

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

	public void SwitchSize()
	{
		if (null != m_SwitchButton)
		{
			OnPressSwitchButton();
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


	public void UpdateColor( int type )
	{
		if (m_ColorImage)
		{
			Color color = m_ColorImage.color;
			float alpha = color.a;
			color = TaskTypeColorMapHelper.GetColor(type);
			color.a = alpha;
			m_ColorImage.color = color ;
		}
	}


	public void Setup()
	{
		m_ColorImage = this.GetComponent<Image>();
		m_Self = this.GetComponent<RectTransform>();
		m_Title = UnityFind.ComponentFind<Text>(this.transform, "Title");
		m_SwitchButton = UnityFind.ComponentFind<Button>(this.transform, "Title/SwitchButton");
		if (null != m_SwitchButton)
		{
			m_SwitchButton.onClick.AddListener(delegate {SwitchSize();} );
		}
		m_Assignee = UnityFind.ComponentFind<Text>(this.transform, "Assignee");
		m_LinkButton = UnityFind.ComponentFind<Button>(this.transform, "Assignee/LinkButton");
		if (null != m_LinkButton)
		{
			m_OpenBrowser = m_LinkButton.gameObject.AddComponent<OnClickOpenBrower>();
			m_LinkButton.onClick.AddListener(delegate {PressButton();} );
		}
		m_Initialzied = true;
	}

	public void ShowPartUI( bool visible  )
	{
		m_Assignee.gameObject.SetActive(visible);
	}

	// Use this for initialization
	void Start () 
	{
		if (false == m_Initialzied)
		{
			Setup();
		}
		
	}

	Image m_ColorImage ;
	RectTransform m_Self ;
	string m_LinkURL = string.Empty ;
	Button m_LinkButton = null ;
	Button m_SwitchButton = null ;
	OnClickOpenBrower m_OpenBrowser =null; 
	Text m_Title = null ;
	Text m_Assignee = null ;
	bool m_Initialzied = false ;
	Task2DParentRegion m_ParentPanel = null;

}
