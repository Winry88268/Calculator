using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NumberClick : MonoBehaviour
{
    public Button m_Button;
    public TMP_Text m_ButtonText;
    public Main m_root;

	void Start () 
    {
        m_root = transform.root.GetComponent<Main>();

        m_Button = this.GetComponent<Button>();
        m_ButtonText = m_Button.GetComponentInChildren<TMP_Text>();

		m_Button.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick()
    {
		m_root.numberInput(m_ButtonText.text);
	}
}
