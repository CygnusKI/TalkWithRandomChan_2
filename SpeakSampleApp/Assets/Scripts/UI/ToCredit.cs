﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//シーンマネジメントを有効にする

public class ToCredit : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("CreditScene");
    }
}
