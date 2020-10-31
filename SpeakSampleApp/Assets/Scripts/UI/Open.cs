using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;

public class Open : MonoBehaviour
{
    // ログ表示欄
    public Text text;
    public GameObject scrollView;
    public GameObject closeButton;
    private string path;
    private string fileName = "dictionary.txt";

    void Start(){
        path = Application.dataPath + '/' + fileName;
    }

    public void OnClick()
    {
        scrollView.SetActive(true);
        closeButton.SetActive (true);
        text.text = "";
        StreamReader sr = new StreamReader(path);
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            string target = "***";
            int num = line.IndexOf(target) + 3;
            string qusetion = line.Substring(0, line.IndexOf(target));
            string answer = line.Remove(0, num);
            text.text += qusetion + "\t" + "----" + "\t" + answer + "\n";
        }
    }
}
