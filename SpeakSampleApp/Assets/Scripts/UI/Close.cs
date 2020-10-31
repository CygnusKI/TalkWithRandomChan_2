using UnityEngine;
using System;
using UnityEngine.UI;

public class Close : MonoBehaviour
{
    public GameObject scrollView;
    public GameObject closeButton;

    public void OnClick()
    {
        scrollView.SetActive (false);
        closeButton.SetActive (false);
    }
}
