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

#pragma warning disable 168
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using NTTDocomo.Speak;

[InitializeOnLoad]
public static class AutoStopSpeakSdkWhenCompilationStart
{
    static bool isSpeakSDKStarted = false;

    static AutoStopSpeakSdkWhenCompilationStart()
    {
        try
        {
            CompilationPipeline.compilationStarted += OnCompilationStarted;
            Speak.Instance().OnStart += AutoStopSpeakSdkWhenCompilationStart.OnSpeakSDKStart;
            Speak.Instance().OnStop += AutoStopSpeakSdkWhenCompilationStart.OnSpeakSDKStop;
            Speak.Instance().OnFailed += AutoStopSpeakSdkWhenCompilationStart.OnSpeakSDKFailed;
        }
        catch(System.TypeInitializationException e)
        {
            //初回load時のTypeInitializationExceptionを無視する
        }
        catch(System.Exception e)
        {
            throw e;
        }
    }

    static void OnSpeakSDKStart()
    {
        AutoStopSpeakSdkWhenCompilationStart.isSpeakSDKStarted = true;
    }

    static void OnSpeakSDKStop()
    {
        AutoStopSpeakSdkWhenCompilationStart.isSpeakSDKStarted = false;
    }

    static void OnSpeakSDKFailed(int code, string message)
    {
        AutoStopSpeakSdkWhenCompilationStart.isSpeakSDKStarted = false;
    }

    private static void OnCompilationStarted(object obj)
    {
        if (EditorApplication.isPlaying && AutoStopSpeakSdkWhenCompilationStart.isSpeakSDKStarted)
        {
            Speak.Instance().Stop(null);
            while (Speak.Instance().Poll(true)) { }
            Debug.LogError("SpeakSDK has stopped, because recompilation occurred. Please restart SpeakSDK.");
        }
    }
}
