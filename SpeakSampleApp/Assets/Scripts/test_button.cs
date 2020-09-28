using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class test_button : MonoBehaviour
{
    public Text text;
    public InputField inputQuestion;
    public InputField inputAnswer;
    public Button Button;

    void Start()
    {
        inputQuestion = inputQuestion.GetComponent<InputField> ();
        inputAnswer = inputAnswer.GetComponent<InputField> ();
    }

    public void Save()
    {
        Debug.Log ("Start");
        text.text = inputQuestion.text + "\n" + inputAnswer.text;
    }

    public void OnClick()
    {
        Debug.Log ("clicked");
        StartCoroutine ("Save");
        Button.gameObject.SetActive(true);
    }
}
