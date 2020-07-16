using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CertusAudioInstructions : MonoBehaviour
{
    public string[] AudioInstructions;
    public List<AudioClip> AudioClips;

    private AudioSource AS;

    void Start()
    {
        AS = GetComponent<AudioSource>();
        Debug.Log(Application.streamingAssetsPath);

        foreach (string instruction in AudioInstructions)
        {
            StartCoroutine(GetAudioClip(instruction));
        }
    }

    public void PlayInstruction(int i)
    {
        AS.clip = AudioClips[i];
        AS.Play();
    }

    IEnumerator GetAudioClip(string clipName)
    {
        string FullPath = "File://" + Application.streamingAssetsPath + "/Certus/Instructions/" + clipName;
        Debug.Log(FullPath);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(FullPath, AudioType.WAV))
        {
            yield return www.Send();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                AudioClips.Add(myClip);
            }
        }
    }
}
