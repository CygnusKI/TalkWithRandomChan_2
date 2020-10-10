using UnityEngine;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TitleTweenText : MonoBehaviour
{
    void Start()
    {
        TextMeshProUGUI text = this.GetComponent<TextMeshProUGUI>();
        text.maxVisibleCharacters = 0;  
        text.DOMaxVisibleCharacters(text.text.Length, 2.0f).Play();
    }
}