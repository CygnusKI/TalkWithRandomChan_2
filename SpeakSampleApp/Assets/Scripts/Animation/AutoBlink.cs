using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class AutoBlink : MonoBehaviour
{
    //とじ目のBlendShapesが登録されているSkinnedMeshRendererを取得
    //まほば君の場合は顔のオブジェクトが持っている
    public SkinnedMeshRenderer face_SkinnedMeshRenderer;
 
    //とじ目の要素数
    public int eyeClose_KeyNomber;
 
    //SetBlendShapeWeightに使用する数値
    //0でまぶたが完全に開く、100でまぶたが完全に閉じる
    private float weight = 0.0f;
     
    //時間を計測するための引数
    public float countTime = 0.0f;
    //まばたきの発動タイミングの引数
    public float blinkTriggerTime = 5.0f;
 
    void FixedUpdate()
    {
        //FixedUpdateは初期設定のままなら
        //0.02秒間隔でくり返し呼ばれる
        CheckCountTime();
    }
 
    void CheckCountTime()
    {
        //時間を計測
        countTime += Time.deltaTime;
        //時間がまばたきの発動タイミングに達したら
        if (countTime > blinkTriggerTime)
        {
            //計測時間をリセット
            countTime = 0.0f;
            //まばたきの発動タイミングの引数に
            //5.0fから10.5fの間でランダムな数値を取得
            blinkTriggerTime = Random.Range(4.0f, 9.5f);
 
            //目を閉じる処理開始
            StartCoroutine("CloseEye");
        }
    }
 
    IEnumerator CloseEye()
    {
        //閉じる際の絶妙な中目加減
        weight = 80.0f;
        //中目にしてちょっとだけ次の処理を待つ
        face_SkinnedMeshRenderer.SetBlendShapeWeight(eyeClose_KeyNomber, weight);
        yield return new WaitForSeconds(0.008f);
        //まぶたを完全に閉じてちょっとだけ次の処理を待つ
        face_SkinnedMeshRenderer.SetBlendShapeWeight(eyeClose_KeyNomber, 100.0f);
        yield return new WaitForSeconds(0.040f);
 
        //目を開く処理開始
        StartCoroutine("OpenEye");
    }
 
    IEnumerator OpenEye()
    {
        //開く際の絶妙な中目加減
        weight = 60.0f;
        //中目にしてちょっとだけ次の処理を待つ
        face_SkinnedMeshRenderer.SetBlendShapeWeight(eyeClose_KeyNomber, weight);
        yield return new WaitForSeconds(0.015f);
        //まぶたを完全に開く
        face_SkinnedMeshRenderer.SetBlendShapeWeight(eyeClose_KeyNomber, 0.0f);
    }
     
}