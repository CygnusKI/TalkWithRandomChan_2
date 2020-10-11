using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class LearnButton : MonoBehaviour
{
    public Text text;
    public InputField inputQuestion;
    public InputField inputAnswer;
    Animator anim;
    //以下らんだむちゃんオブジェクトにアタッチされているanimatorを参照するための宣言
    GameObject randomchan_object;


    void Start()
    {
        inputQuestion = inputQuestion.GetComponent<InputField> ();
        inputAnswer = inputAnswer.GetComponent<InputField> ();
        randomchan_object = GameObject.Find ("Randomchan");
        anim = randomchan_object.GetComponent<Animator>();
    }

    public void OnClick()
    {
        StreamWriter sw = new StreamWriter("Assets/txt/dictionary.txt", true);
        sw.WriteLine(inputQuestion.text + "***" + inputAnswer.text);
        sw.Flush();
        sw.Close();
        anim.SetTrigger("In");
        text.text = "保存されました";
    }
}
