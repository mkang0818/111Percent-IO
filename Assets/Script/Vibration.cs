using UnityEngine;

public class Vibration : MonoBehaviour
{
    // Android ����Ƽ�� ���� ȣ��
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

    // �⺻ ����
    public static void Vibrate()
    {
        Handheld.Vibrate();
    }

    // ���� ���
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
