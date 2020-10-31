/*
 * Copyright (c) 2020, NTT DOCOMO, INC.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *  Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 *  Redistributions in binary form must reproduce the above copyright notice,
 *   this list of conditions and the following disclaimer in the documentation
 *   and/or other materials provided with the distribution.
 *  Neither the name of the NTT DOCOMO, INC. nor the names of its contributors
 *   may be used to endorse or promote products derived from this software
 *   without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL NTT DOCOMO, INC. BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using UnityEngine;
using NTTDocomo.Speak;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using System.Threading;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class TalkButton : MonoBehaviour
{
    // ログ表示欄
    private ScrollRect mScrollRect;
    // 表示するログの保持用変数
    private Text mTextLog;
    private string mLogs = "";
    private string mOldLogs = "";

    // 自動停止
    private static readonly float TIMEOUT = 10.000f;//10000/ms
    private int mDialogCounter  = 0;
    private SynchronizationContext mContext;

    //TalkAPIの設定
    string url = "https://api.a3rt.recruit-tech.co.jp/talk/v1/smalltalk";
    string apikey = "DZZzWnCYOGHjsfahjCE4YrvM0GnAUlS3";
    public string query = "";
    public string reply = "";
    public Text text;
    public InputField inputField;
    public AudioSource mmAudioSource;

    public List<string> dic = new List<string>();
    bool has_dic = false;

    //0はユーザー、1はらんだむちゃん、2はシステム
    int talker;

    public void Start()
    {
        StreamReader sr = new StreamReader("Assets/txt/dictionary.txt");
        string line;
        while ((line = sr.ReadLine()) != null) dic.Add(line);

        #if (PLATFORM_ANDROID)
        // Androidのパーミッションが有効になっていない場合、パーミッションの許可を求める
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
        #endif

        mmAudioSource = gameObject.GetComponent<AudioSource>();
        //Speak.Instance().Mute();

        //Speakの初期化
        InitializeSpeakSDK();

        //UIの取得
        mScrollRect = GameObject.Find("ScrollView").GetComponent<ScrollRect>();
        mTextLog = mScrollRect.content.GetComponentInChildren<Text>();

        //コンテキストの取得
        mContext = SynchronizationContext.Current;

        sr.Close();
    }

    public void Update()
    {
        try
        {
            Speak.Instance().Poll();
        }
        catch
        {
        }

        //  mLogsとmOldLogsが異なるときにTextを更新
        if (mScrollRect != null && mLogs != mOldLogs)
        {
            mTextLog.text = mLogs;
            // Textが追加されたとき自動でScrollViewのBottomに移動する
            mScrollRect.verticalNormalizedPosition = 0;
            mOldLogs = mLogs;
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
		    Speak.Instance().Stop(OnStop);
		    while (Speak.Instance().Poll(true)) { }
    }

    void OnApplicationQuit()
    {
        Speak.Instance().Stop(OnStop);
        while (Speak.Instance().Poll(true)) { }
    }

    // ---------------------------------------------------------------------------- //
    //  SDK初期化処理
    // ---------------------------------------------------------------------------- //
    private void InitializeSpeakSDK()
    {
        Speak.Instance().SetURL("wss://hostname.domain:443/path");
        //Speak.Instance().SetDeviceToken("1ff910ed-8a4a-4d8d-9525-2514d02cc9b5");
        Speak.Instance().SetDeviceToken("14d1d3f4-6de1-429c-92f7-b614d02503ce");

        // Callback.
        Speak.Instance().SetOnTextOut(OnTextOut);
        Speak.Instance().SetOnMetaOut(OnMetaOut);
        Speak.Instance().SetOnPlayEnd(OnPlayEnd);

        //音声入力、マイクミュート状態へ
        //Speak.Instance().SetMicMute(true);

        // AudioSource
        Speak.Instance().SetAudioSource(mmAudioSource);
    }

    // ---------------------------------------------------------------------------- //
    //
    //  ボタンイベントで使用する関数
    //
    // ---------------------------------------------------------------------------- //

    // ---------------------------------------------------------------------------- //
    //  送信ボタンを押下した時に呼び出される
    // ---------------------------------------------------------------------------- //
    public void OnClick()
    {
        // Speakの実行
        Speak.Instance().Start(OnStart, OnFailed);
        //text.text += "あなた　　　　\t" + inputField.text + "\n";
        has_dic = false;
        if(inputField.text ==  ""){

        }else{
            text.text += "あなた　　　　\t" + inputField.text + "\n";
            for (int i = 0; i < dic.Count; i++)
            {
                if (dic[i].Contains(inputField.text))
                {
                    NluMetaData data = new NluMetaData();
                    data.clientData = new ClientData();
                    data.clientData.deviceInfo = new DeviceInfo();
                    data.clientData.deviceInfo.playTTS = "on";
                    data.clientVer = "0.5.1";
                    data.language = "ja-JP";

                    has_dic = true;
                    string target = "***";
                    int num = dic[i].IndexOf(target) + 3;
                    string mes = dic[i].Remove(0, num);
                    data.voiceText = mes;

                    Debug.Log(mes);
                    text.text += "らんだむちゃん\t" + dic[i].Remove(0, num) + "\n";
                    string json = JsonUtility.ToJson(data);
                    Speak.Instance().PutMeta(json);

                    CancelInvoke("AutoStopTask");
                    Interlocked.Increment(ref mDialogCounter);

                    //text.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                    LogView(inputField.text, 0);
                    LogView(mes, 1);
                    inputField.text = "";
                    break;
                }
            }

            if (has_dic == false) StartCoroutine ("Conect");
        }
        /*for (int i = 0; i < dic.Count; i++)
        {
            if (dic[i].Contains(inputField.text))
            {
                NluMetaData data = new NluMetaData();
                data.clientData = new ClientData();
                data.clientData.deviceInfo = new DeviceInfo();
                data.clientData.deviceInfo.playTTS = "on";
                data.clientVer = "0.5.1";
                data.language = "ja-JP";

                has_dic = true;
                string target = "***";
                int num = dic[i].IndexOf(target) + 3;
                string mes = dic[i].Remove(0, num);
                data.voiceText = mes;

                Debug.Log(mes);
                text.text += "らんだむちゃん\t" + dic[i].Remove(0, num) + "\n";
                string json = JsonUtility.ToJson(data);
                Speak.Instance().PutMeta(json);

                CancelInvoke("AutoStopTask");
                Interlocked.Increment(ref mDialogCounter);

                //text.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                LogView(inputField.text, 0);
                LogView(mes, 1);
                inputField.text = "";
                break;
            }
        }

        if (has_dic == false) StartCoroutine ("Conect");*/
    }

    IEnumerator Conect()
    {
        query = inputField.text.Replace("\n", "");

        NluMetaData data = new NluMetaData();
        data.clientData = new ClientData();
        data.clientData.deviceInfo = new DeviceInfo();
        data.clientData.deviceInfo.playTTS = "on";
        data.clientVer = "0.5.1";
        data.language = "ja-JP";

        // ChatAPIに送る情報を入力
        WWWForm form = new WWWForm();

        form.AddField("apikey", apikey);
        form.AddField("query", query, Encoding.UTF8);

        // 通信
        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            //yield return request.Send();
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                try
                {
                    // 取得したものをJsonで整形
                    string itemJson = request.downloadHandler.text;
                    JsonNode jsnode = JsonNode.Parse(itemJson);
                    string reply = jsnode["results"][0]["reply"].Get<string>();
                    // Jsonから会話部分だけ抽出してTextに代入
                    if (text.text != null) {
                        //text.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                        text.text += "らんだむちゃん\t" + reply + "\n";
                    }
                    Debug.Log(reply);
                    LogView(inputField.text, 0);
                    LogView(reply, 1);
                    data.voiceText = reply;
                }
                catch (Exception e)
                {
                    // エラーが出たらこれがログに吐き出される
                    Debug.Log("JsonNode:" + e.Message);
                }
            }
        }

        string json = JsonUtility.ToJson(data);
        Speak.Instance().PutMeta(json);

        CancelInvoke("AutoStopTask");
        Interlocked.Increment(ref mDialogCounter);

        //LogView(inputField.text, 0);
        //LogView(reply, 1);
        inputField.text = "";
    }

    // ---------------------------------------------------------------------------- //
    //
    //  Speak で使用するコールバック関数
    //
    // ---------------------------------------------------------------------------- //
    public void OnStart()
    {
        Invoke("AutoStopTask", TIMEOUT);
        Debug.Log("OnStart on");
    }

    public void OnStop()
    {
        CancelInvoke("AutoStopTask");
    }

    public void OnFailed(int ecode, string failstr)
    {
        Debug.Log("OnFailed");
        Debug.Log(ecode);
        Debug.Log(failstr);

    }

    // ---------------------------------------------------------------------------- //
    //
    //  Speak に用意されているコールバック関数の受信側
    //
    // ---------------------------------------------------------------------------- //

    // ---------------------------------------------------------------------------- //
    //  メタ情報を受信した時に呼ばれるメソッド
    //
    //  登録例 : Speak.Instance().SetOnMetaOut(OnMetaOut);
    //  引数(string) : JSON形式のメタ情報
    // ---------------------------------------------------------------------------- //
    public void OnMetaOut(string metaText)
    {
        var metaData = OnMetaOutJson.CreateFromJSON(metaText);
        // 再生テキスト内容
        if (!String.IsNullOrEmpty(metaData.systemText.utterance))
        {
            // スクロールビューにテキストを表示する
            LogView(metaData.systemText.utterance, 2);
        }
        // 再生テキスト取得失敗時の表示内容
        else if (!String.IsNullOrEmpty(metaData.systemText.expression))
        {
            // スクロールビューにテキストを表示する
            LogView(metaData.systemText.expression, 2);
        }

        if (metaData.type == "speech_recognition_result")
        {
            // 対話の開始
            CancelInvoke("AutoStopTask");
            Interlocked.Increment(ref mDialogCounter);
        }
        else if (String.IsNullOrEmpty(metaData.systemText.utterance))
        {
            if (metaData.type == "nlu_result" &&
                Interlocked.Decrement(ref mDialogCounter) == 0)
            {
                // 対話の終了
                Invoke("AutoStopTask", TIMEOUT);
            }
        }
    }

    // ---------------------------------------------------------------------------- //
    //  対話テキストを受信した時に呼ばれるメソッド
    //
    //  登録例 : Speak.Instance().SetOnTextOut(OnTextOut);
    //  引数(string) : JSON形式の対話テキスト情報
    // ---------------------------------------------------------------------------- //
    public void OnTextOut(string metaText)
    {
        // スクロールビューにテキストを表示する
        // 発話内容
        var speechMetaData = OnTextOutJson.CreateFromJSON(metaText);
        string viewText = "";
        viewText = MetaFindVoiceText(speechMetaData);
        if (!String.IsNullOrEmpty(viewText))
        {
            LogView(viewText, 1);
        }
    }

    // ---------------------------------------------------------------------------- //
    //  合成音声再生終了時に呼ばれるメソッド
    //
    //  登録例 : Speak.Instance().SetOnPlayEnd(OnPlayEnd);
    //  引数(string) : 空文字
    // ---------------------------------------------------------------------------- //
    public void OnPlayEnd(string text)
    {
        if (Interlocked.Decrement(ref mDialogCounter) == 0)
        {
            Invoke("AutoStopTask", TIMEOUT);
        }
    }

    // ---------------------------------------------------------------------------- //
    //
    //  画面上に表示するための関数
    //
    // ---------------------------------------------------------------------------- //

    // ---------------------------------------------------------------------------- //
    // ログを表示するメソッド
    // ---------------------------------------------------------------------------- //
    private void LogView(string viewText, int talker)
    {
        if (!String.IsNullOrEmpty(viewText))
        {
            if(talker == 0)
            {
                mLogs += "あなた　　　　\t";
                Debug.Log("You");
            }
            else if(talker == 1)
            {
                mLogs += "らんだむちゃん\t";
                Debug.Log("らんだむちゃん");
            }
            else if(talker == 2)
            {
                mLogs += "システム\t\t";
            }
            mLogs += viewText;
            mLogs +=  "\n";
        }
    }

    // ---------------------------------------------------------------------------- //
    // JsonデータからTextを取得
    // ---------------------------------------------------------------------------- //

    private string MetaFindVoiceText(OnTextOutJson speechrec)
    {
        if(speechrec.sentences != null)
        {
            foreach (OnTextOutJson.Sentence sentence in speechrec.sentences)
            {
                if (!String.IsNullOrEmpty(sentence.converter_result))
                {
                    return sentence.converter_result;
                }
            }
        }
        return null;
    }

    // ---------------------------------------------------------------------------- //
    //
    // SDKを自動停止させるための関数
    //
    // ---------------------------------------------------------------------------- //
    private void AutoStopTask()
    {
        mContext?.Post(__ =>
        {
            Speak.Instance().Stop(OnStop);
        }, null);
    }
}