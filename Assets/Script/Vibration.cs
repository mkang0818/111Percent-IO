using UnityEngine;

public class Vibration : MonoBehaviour
{
    // Android 네이티브 진동 호출
    public static void Vibrate(long milliseconds)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

            if (vibrator != null)
            {
                vibrator.Call("vibrate", milliseconds);
            }
        }
        else
        {
            Debug.Log("Vibration is not supported on this platform.");
        }
    }

    // 기본 진동
    public static void Vibrate()
    {
        Handheld.Vibrate();
    }

    // 진동 취소
    public static void Cancel()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

            if (vibrator != null)
            {
                vibrator.Call("cancel");
            }
        }
        else
        {
            Debug.Log("Vibration cancel is not supported on this platform.");
        }
    }
}
