using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Equation : MonoBehaviour
{
    public TMP_Text m_equationText;
    
    public Queue<string> m_equation = new Queue<string>();

    void Awake()
    {
        m_equationText = this.GetComponent<TMP_Text>();

        m_equationText.SetText("");
    }

    void Update()
    {
        if(m_equation.Count != 0)
        {
            updateEquation();
        }
    }

    public void clearEquation()
    {
        m_equationText.SetText("");
    }

    void updateEquation()
    {
        string v = m_equationText.text + m_equation.Dequeue();
        m_equationText.SetText(v);
    }
}
