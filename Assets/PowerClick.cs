using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerClick : MonoBehaviour
{
    public Button m_Button;
    public Main m_root;

	void Start () 
    {
        m_root = transform.root.GetComponent<Main>();

        m_Button = this.GetComponent<Button>();

		m_Button.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick()
    {
		m_root.functionInput("pow");
	}
}
