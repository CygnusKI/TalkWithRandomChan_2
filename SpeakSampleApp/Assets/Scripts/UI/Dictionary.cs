using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using System.Text;

public class Dictionary : MonoBehaviour
{
    // ログ表示欄
    public Text text;
    private string path;
    private string fileName = "dictionary.txt";

    void Start()
    {
        path = Application.dataPath + '/' + fileName;
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
