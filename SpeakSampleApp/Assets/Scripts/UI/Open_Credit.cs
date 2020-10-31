using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class Open_Credit : MonoBehaviour
{
    // ログ表示欄
    public Text text;
    public GameObject scrollView;
    public GameObject closeButton;

    public void OnClick()
    {
        scrollView.SetActive(true);
        closeButton.SetActive (true);
    }
}
