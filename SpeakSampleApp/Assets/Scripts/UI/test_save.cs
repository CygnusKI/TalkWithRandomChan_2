using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class test_save : MonoBehaviour
{
    public Text text;
    public InputField inputQuestion;
    public InputField inputAnswer;
    public Button Button;

    void Start ()
    {
        inputQuestion = inputQuestion.GetComponent<InputField> ();
        inputAnswer = inputAnswer.GetComponent<InputField> ();
    }

    public void OnClick()
    {
        Debug.Log ("clicked!!!");
        StreamWriter sw = new StreamWriter("Assets/txt/dictionary.txt", true);
        sw.WriteLine(inputQuestion.text + "***" + inputAnswer.text);
        sw.Flush();
        sw.Close();
        text.text = "保存されました";
        Button.gameObject.SetActive(false);
    }
}
