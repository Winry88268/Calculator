using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    string m_equationNumbers;

    List<float> m_numbers = new List<float>();
    List<string> m_functions = new List<string>();

    List<Tuple<int, string>> m_parenthesesIndex = new List<Tuple<int, string>>();
    List<Tuple<int, float>> m_powersIndex = new List<Tuple<int, float>>();
    List<Tuple<int, string>> m_multdivIndex = new List<Tuple<int, string>>();

    float? m_answer;

    public Equation m_equationDisplay;
    public Answer m_answerDisplay;

    bool m_error;
    
    void Start()
    {
        m_equationDisplay = this.GetComponentInChildren<Equation>();
        m_answerDisplay = this.GetComponentInChildren<Answer>();

        clear(); 
    }

    public void clear()
    {
        BroadcastMessage("clearEquation");
    }

    void clearEquation()
    {
        m_equationNumbers = null;
        m_numbers.Clear();
        m_functions.Clear();
        m_parenthesesIndex.Clear();
        m_powersIndex.Clear();
        m_multdivIndex.Clear();
        m_answer = null;
        m_error = false;
    }

    //New number input process
    public void numberInput(string n)
    {
        //Impossible input - do nothing until cleared
        if(m_error) return;

        //If null = new number string
        if(m_equationNumbers == null)
        {
            m_equationNumbers = n;    
        }
        else
        {
            m_equationNumbers += n;
        }

        if(m_functions.Count >0 && m_functions.Last() == "pow")
        {
            //Equation Display to update
            m_equationDisplay.m_equation.Enqueue("<sup>" + n + "</sup>");
        }
        else
        {
            //Equation Display to update
            m_equationDisplay.m_equation.Enqueue(n);
        } 
    }

    //New function input process
    public void functionInput(string f)
    {
        queueNumbers();

        //Impossible input - do nothing until cleared
        if(m_error) return;

        //Add to Functions list
        m_functions.Add(f);
            
        if(f == "pow")
        {
            //Equation Display to update
            m_equationDisplay.m_equation.Enqueue("<sup>^</sup>");
        }
        else
        {
            //Equation Display to update
            m_equationDisplay.m_equation.Enqueue(f);
        }
    }

    void queueNumbers()
    {  
         //Equation = null, default first number to 0
        if(m_equationNumbers == null)
        {
            m_numbers.Add(0f);
        }
        //Otherwise parse and add to m_numbers
        else
        {   
            try
            {
                m_numbers.Add(float.Parse(m_equationNumbers));
            }
            catch (FormatException)
            {
                m_answerDisplay.updateAnswer("Error: NaN");
                m_error = true;
            }          
        }

        //Purge equation
        m_equationNumbers = null;
    }

    //= function has been called
    public void calculateAnswer()
    {
        //Impossible input - do nothing until cleared
        if(m_error) return;

        //!null = previous answer still displayed - do nothing until cleared
        if(m_answer != null) return;

        try
        {
            queueNumbers();
            parentheses();
            powers();
            divideAndMultiply();        
            addAndSubtract();

            m_answer = m_numbers[0];
            //Purge m_numbers
            m_numbers.Clear();
            //Display answer
            m_answerDisplay.updateAnswer(m_answer.ToString());
        }
        catch (DivideByZeroException)
        {
            m_answerDisplay.updateAnswer("Error: Div/0");
            m_error = true;
        }
    }

    void parentheses()
    {

    }

    void powers()
    {
        //Iterate functions - tuple<index,number> for powers
        foreach(var v in m_functions)
        {
            //Index in m_functions & matching index in m_numbers
            if(v == "pow")
            {
                int i = m_functions.IndexOf(v);
                m_powersIndex.Add(new Tuple<int, float>(i, m_numbers[i + 1]));        
            }
        }

        //If any powers
        if(m_powersIndex != null)
        {
            //Reverse to avoid index cascade error
            m_powersIndex.Reverse();

            //Remove all pow from functions list
            foreach(Tuple<int, float> tuple in m_powersIndex)
            {
                m_functions.RemoveAt(tuple.Item1);
            }

            //Reverse again to avoid BODMAS error
            m_powersIndex.Reverse();

            //Complete pow functions on numbers list
            foreach(Tuple<int, float> tuple in m_powersIndex)
            {   
                //x = number at same index
                double x = m_numbers[tuple.Item1];
                double y = tuple.Item2;

                m_answer = (float) Math.Pow(x, y);

                //Remove power number
                m_numbers.RemoveAt(tuple.Item1 + 1);
                //Replace number
                m_numbers[tuple.Item1] = (float) m_answer;
            }
        }
    }

    void divideAndMultiply()
    {
        //Iterate functions - tuple<index,function> for * & /
        foreach(var v in m_functions)
        {
            if(v == "*" | v == "/")
            {
                m_multdivIndex.Add(new Tuple<int, string>(m_functions.IndexOf(v), v));
            }
        }

        //If any * or /
        if(m_multdivIndex != null)
        {
            //Reverse to avoid index cascade error
            m_multdivIndex.Reverse();

            //Remove all * & / from functions list
            foreach(Tuple<int, string> tuple in m_multdivIndex)
            {
                m_functions.RemoveAt(tuple.Item1);
            }

            //Reverse again to avoid BODMAS error
            m_multdivIndex.Reverse();

            //Complete * & / functions on numbers list
            foreach(Tuple<int, string> tuple in m_multdivIndex)
            {   
                //x = number at same index
                float x = m_numbers[tuple.Item1];
                //y = number at index + 1
                float y = m_numbers[(tuple.Item1 + 1)];
                string z = tuple.Item2;
                
                if(y == 0 && z == "/")
                {
                    throw new DivideByZeroException();
                }

                if(z == "*")
                {
                    m_answer = x*y;
                }
                else
                {
                    m_answer = x/y;
                }

                //Replace higher index of 2 calculated numbers
                m_numbers[(tuple.Item1 + 1)] = (float) m_answer;
            }

            //Reverse to avoid index cascade error
            m_multdivIndex.Reverse();

            //Remove all processed numbers from numbers list
            foreach(Tuple<int, string> tuple in m_multdivIndex)
            {
                m_numbers.RemoveAt(tuple.Item1);
            }
        }
    }

    void addAndSubtract()
    {
        //At least 2 separate numbers
        while(m_numbers.Count > 1)
        {
            float x = m_numbers[0];
            float y = m_numbers[1];
            string z = m_functions[0];

            if(z == "+")
            {
                m_answer = x+y;
            }
            else
            {
                m_answer = x-y;
            }

            //Each iteration reduces both numbers & functions list length by 1
            m_numbers.RemoveRange(0, 2);
            m_numbers.Insert(0, (float) m_answer);
            m_functions.RemoveAt(0);
        }
    }
}