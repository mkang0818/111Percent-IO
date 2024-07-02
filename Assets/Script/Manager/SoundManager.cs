using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] public AudioSource MusicAudio;

    // �̱���
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // ���� ��� �޼���
    public void SoundPlay(string SoundName, AudioClip clip)
    {
        GameObject SoundObj = new GameObject(SoundName + "Sound");
        AudioSource audiosource = SoundObj.AddComponent<AudioSource>();
        audiosource.clip = clip;
        audiosource.volume = 1;
        audiosource.Play();

        Destroy(SoundObj, clip.length);
    }
}
