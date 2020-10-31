using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToTitleFromCaution : MonoBehaviour
{

    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0)/*右クリックしたら*/){
            //Fadeoutしてシーン遷移
            FadeManager.FadeOut("Title");
        }
    }
}
