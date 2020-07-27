using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Crosstales;
using RootMotion.FinalIK;
using System.Threading;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using TMPro;
using Debug = System.Diagnostics.Debug;


public class CertusManager : MonoBehaviour
{

    public string PathToPythonExe;
    public string PathToPythonScript;
    public string ServerIP = "127.0.0.1";
    public TMP_Text serverIpText, pythonExeTmpText;

    public float PerturbationOpeningMin = 1f, PerturbationOpeningMax = 1f, PerturbationOpeningDuration = 180f, PerturbationStretchingMin = 1f, PerturbationStretchingMax = 1f, PerturbationStretchingDuration = 180f, PerturbationProtrusionMin = 1f, PerturbationProtrusionMax = 1f, PerturbationProtrusionDuration = 180f;

    void Start()
    {
        PathToPythonScript = Application.streamingAssetsPath + "/Certus/Certus2Unity.py";
        PathToPythonExe = PlayerPrefs.GetString("PathToPythonExe");
        pythonExeTmpText.text = PathToPythonExe;
        ServerIP = PlayerPrefs.GetString("ServerIP");
        serverIpText.text = ServerIP;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        {
            SetPathToPythonExe();
        }

        if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        {
            StartExperiment();
        }
    }

    public void StartExperiment()
    {
        //TODO: Redo automating Python Script Start
        /*
        ExecuteCommand(ServerIP);
        Invoke("LoadMainScene", 3f);
        */

        LoadMainScene();
    }

    public void SetPathToPythonExe()
    {
        PathToPythonExe = Crosstales.FB.FileBrowser.OpenSingleFile("Python .exe file", "C://", "exe");
        PlayerPrefs.SetString("PathToPythonExe", PathToPythonExe);
        pythonExeTmpText.text = PathToPythonExe;
    }

    void LoadMainScene()
    {
        SceneManager.LoadScene("Certus2MCS", LoadSceneMode.Additive);
    }

    public void SetServerIP(string ip)
    {
        ServerIP = ip;
        PlayerPrefs.SetString("ServerIP", ip);
    }

    public void ExecuteCommand(string cmd)
    {
        var thread = new Thread(delegate () { Command(cmd); });
        thread.Start();
    }

    void Command(string input)
    {
        UnityEngine.Debug.LogWarning(string.Format("Strating command: {0} {1} {2}", PathToPythonExe, PathToPythonScript, input));
        var processInfo = new ProcessStartInfo(PathToPythonExe, PathToPythonScript + " " + input);
        processInfo.CreateNoWindow = false;
        processInfo.UseShellExecute = false;

        var process = Process.Start(processInfo);

        process.WaitForExit();
        process.Close();
    }
}
