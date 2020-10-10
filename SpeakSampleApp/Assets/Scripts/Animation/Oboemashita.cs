using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oboemashita : MonoBehaviour
{
    private AudioSource sound01;

    void Start()
    {
        sound01 = GetComponent<AudioSource>();
    }

    public void OnClick()
    {
        sound01.PlayOneShot(sound01.clip);
    }
}
