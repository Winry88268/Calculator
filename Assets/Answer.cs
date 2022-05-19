using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Answer : MonoBehaviour
{
    TMP_Text m_answerText;

    void Awake()
    {
        m_answerText = this.GetComponent<TMP_Text>();

        m_answerText.SetText("");
    }

    public void clearEquation()
    {
        m_answerText.SetText("");
    }

    public void updateAnswer(string v)
    {
        m_answerText.SetText(v);
    }
}
