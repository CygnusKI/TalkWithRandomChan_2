using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LLiipp : MonoBehaviour 
{
    //ブレンドメッシュを持つSkinnedMeshRendererを設定
    public SkinnedMeshRenderer _skinnedMeshRenderer;

    //口パクに使うブレンドメッシュのIndex
    public int _index;

    //ゲイン
    public float _gain = 4000f;

    float[] _waveData = new float[1024];

    //AudioSourceの設定
    GameObject canvas;
    AudioSource voice;


    void Start () {
        canvas = GameObject.Find("AudioCanvas");
        voice = canvas.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update () 
    {
        //AudioListenerから音声データを取得
        AudioListener.GetOutputData(_waveData,1);
        if(voice.isPlaying){
            _skinnedMeshRenderer.SetBlendShapeWeight(_index, _waveData.Average() * _gain);
        }else{
            _skinnedMeshRenderer.SetBlendShapeWeight(_index, 0);
        }
        //平均値にゲインをかけてブレンドメッシュを変化させる
    }
}