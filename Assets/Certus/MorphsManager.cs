using System.Collections;
using System.Collections.Generic;
using Assets.LSL4Unity.Scripts.Examples;
using UnityEngine;
using MCS;
using TMPro;
using System;
//using Pixelplacement;
//using Pixelplacement.TweenSystem;

public class MorphsManager : MonoBehaviour
{
    [Range(0,1)]
    public float StretchingVsSmile;

    public MCSCharacterManager CharacterManager;
    public MirrorRotation HeadRotation;

    public Light Main_Light;
    public string SpeechState = "Silence";
    //public int CurrentCharacter = 0;

    public MarkersManager markersManager;
    //public CertusManager certusManager;

    public float PerturbationOpening = 1f, PerturbationStretching = 1f, PerturbationProtrusion = 1f;
    public float PerturbationOpeningMin = 1f, PerturbationOpeningMax = 1f, PerturbationStretchingMin = 1f, PerturbationStretchingMax = 1f, PerturbationProtrusionMin = 1f, PerturbationProtrusionMax = 1f;


    public float SilenceDuration = 2f, TalkDuration = 4f;
    public int NbRepetitions = 30;
    public int CurrentRepetition = 0;
    public float PerturbationInterpolation = 1f;
    public float PerturbationHeadRotation = 1f;

    //public TweenBase TweenOpening, TweenStretching, TweenProtrusion;

    public TextMeshProUGUI P_opening, P_stretching, P_protrusion, P_head;
    public TextMeshProUGUI FilePathUI;

    public bool ApplyVisemes = false;

    public bool RecordData;

    public string FileName = "Data";
    public string FilePath;

    public Transform HeadRigidBody, HeadEffector;

    void Start()
    {
        //certusManager = FindObjectOfType<CertusManager>();

        markersManager = GetComponent<MarkersManager>();
        CharacterManager.SetBlendshapeValue("PHMMouthWidth_NEGATIVE_", 100f );

        //TweenOpening    = Tween.Value(PerturbationOpeningMin,    PerturbationOpeningMax,    SetPerturbationOpening,    PerturbationOpeningDuration,    0f, Tween.EaseLinear, Tween.LoopType.None);
        //TweenStretching = Tween.Value(PerturbationStretchingMin, PerturbationStretchingMax, SetPerturbationStretching, PerturbationStretchingDuration, 0f, Tween.EaseLinear, Tween.LoopType.None);
        //TweenProtrusion = Tween.Value(PerturbationProtrusionMin, PerturbationProtrusionMax, SetPerturbationProtrusion, PerturbationProtrusionDuration, 0f, Tween.EaseLinear, Tween.LoopType.None);

        //TweenOpening.Stop();
        //TweenStretching.Stop();
        //TweenProtrusion.Stop();

        FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + FileName + ".csv";
        CertusFileManager.FilePath = FilePath;
        FilePathUI.text = FilePath;

        HeadRigidBody = GameObject.Find("HeadRigidBody").GetComponent<Transform>();
        HeadEffector = GameObject.Find("HeadEffector").GetComponent<Transform>();
    }

    public void UpdateDataPath(string fileName)
    {
        FileName = fileName;
        FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + FileName + ".csv";
        CertusFileManager.FilePath = FilePath;
        FilePathUI.text = FilePath;
    }

    public void SetPerturbationOpeningMin(string f)
    {
        f = f.Replace(".", ",");
        try
        {
            PerturbationOpeningMin = float.Parse(f);
        }
        catch (System.Exception)
        {

            PerturbationOpeningMin = 1f;
        }
        
    }

    public void SetPerturbationOpeningMax(string f)
    {
        f = f.Replace(".", ",");
        try
        {
            PerturbationOpeningMax = float.Parse(f);
        }
        catch (System.Exception)
        {
            PerturbationOpeningMax = 1f;
        }
    }

    public void SetPerturbationStretchingMin(string f)
    {
        f = f.Replace(".", ",");
        try
        {
            PerturbationStretchingMin = float.Parse(f);
        }
        catch (System.Exception)
        {
            PerturbationStretchingMin = 1f;
        }
    }
    public void SetPerturbationStretchingMax(string f)
    {
        f = f.Replace(".", ",");
        try
        {
            PerturbationStretchingMax = float.Parse(f);
        }
        catch (System.Exception)
        {
            PerturbationStretchingMax = 1f;
        }
    }

    public void SetPerturbationProtrusionMin(string f)
    {
        f = f.Replace(".", ",");
        try
        {
            PerturbationProtrusionMin = float.Parse(f);
        }
        catch (System.Exception)
        {
            PerturbationProtrusionMin = 1f;
        }
    }

    public void SetPerturbationProtrusionMax(string f)
    {
        f = f.Replace(".", ",");
        try
        {
            PerturbationProtrusionMax = float.Parse(f);
        }
        catch (System.Exception)
        {
            PerturbationProtrusionMax = 1f;
        }
    }

    public void SetSilenceDuration(string f)
    {
        f = f.Replace(".", ",");
        try
        {
            SilenceDuration = float.Parse(f);
        }
        catch (System.Exception)
        {
            SilenceDuration = 2f;
        }
    }

    public void SetTalkDuration(string f)
    {
        f = f.Replace(".", ",");
        try
        {
            TalkDuration = float.Parse(f);
        }
        catch (System.Exception)
        {
            TalkDuration = 4f;
        }
    }

    public void SetNbRepetitions(string f)
    {
        try
        {
            NbRepetitions = int.Parse(f);
        }
        catch (System.Exception)
        {
            NbRepetitions = 10;
        }
    }

    public void SetActiveCharacter(MCSCharacterManager c)
    {
        CharacterManager = c;
        CharacterManager.SetBlendshapeValue("PHMMouthWidth_NEGATIVE_", 100f);
    }

    public void applyVisemes(bool a)
    {
        ApplyVisemes = a;
    }

    public void PerturbationAnimationStart()
    {
        /*
        TweenOpening.Start();
        TweenStretching.Start();
        TweenProtrusion.Start();
        */
        CurrentRepetition = 0;
        for (int i = 0; i < NbRepetitions; i++)
        {
            Invoke("ApplyPerturbation", (SilenceDuration + TalkDuration) * i);
        }

        StartCoroutine(StartDataRecording((SilenceDuration + TalkDuration) * NbRepetitions));
    }

    IEnumerator StartDataRecording(float duration)
    {
        CertusFileManager.AddToFile("Time;Time (Ticks);"
                                    + "Opening;"
                                    + "Perturbated Opening;"
                                    + "Stretching;"
                                    + "Perturbated Stretching;"
                                    + "Protrusion;"
                                    + "Perturbated Protrusion;"
                                    + "Head Perturbation;"
                                    + "User Head X; User Head Y; User Head Z; Character Head X; Character Head Y; Character Head Z;"
                                    + "Repetition; State");
        RecordData = true;
        yield return new WaitForSeconds(duration);
        RecordData = false;
    }

    public void ApplyPerturbation()
    {
        CurrentRepetition += 1;
        PerturbationInterpolation = Mathf.InverseLerp(1, NbRepetitions, CurrentRepetition);
        PerturbationOpening = Mathf.Lerp(PerturbationOpeningMin, PerturbationOpeningMax, PerturbationInterpolation);
        PerturbationStretching = Mathf.Lerp(PerturbationStretchingMin, PerturbationStretchingMax, PerturbationInterpolation);
        PerturbationProtrusion = Mathf.Lerp(PerturbationProtrusionMin, PerturbationProtrusionMax, PerturbationInterpolation);
        PerturbationHeadRotation = Mathf.Lerp(HeadRotation.RotationMultiplier_Start, HeadRotation.RotationMultiplier_End, PerturbationInterpolation);

        HeadRotation.rotationConstraint.weight = PerturbationHeadRotation;

        P_opening.text = PerturbationOpening.ToString();
        P_stretching.text = PerturbationStretching.ToString();
        P_protrusion.text = PerturbationProtrusion.ToString();
        P_head.text = PerturbationHeadRotation.ToString();

        Main_Light.intensity = .25f;
        SpeechState = "Silence";
        Invoke("LightOn", SilenceDuration);
    }

    public void LightOn()
    {
        Main_Light.intensity = .75f;
        SpeechState = "Speech";
    }
    

    public void PerturbationAnimationStop()
    {
        /*
        TweenOpening.Stop();
        TweenStretching.Stop();
        TweenProtrusion.Stop();
        */
    }

    public void PerturbationAnimationRewind()
    {
        /*
        TweenOpening.Rewind();
        TweenStretching.Rewind();
        TweenProtrusion.Rewind();
        */
    }

    public void PerturbationAnimationUpdate()
    {
        /*
        TweenOpening = Tween.Value(PerturbationOpeningMin, PerturbationOpeningMax, SetPerturbationOpening, PerturbationOpeningDuration, 0f, Tween.EaseLinear, Tween.LoopType.None);
        TweenStretching = Tween.Value(PerturbationStretchingMin, PerturbationStretchingMax, SetPerturbationStretching, PerturbationStretchingDuration, 0f, Tween.EaseLinear, Tween.LoopType.None);
        TweenProtrusion = Tween.Value(PerturbationProtrusionMin, PerturbationProtrusionMax, SetPerturbationProtrusion, PerturbationProtrusionDuration, 0f, Tween.EaseLinear, Tween.LoopType.None);
        */

        PerturbationAnimationRewind();
    }

    void Update()
    {
        if (ApplyVisemes)
        {
            CharacterManager.SetBlendshapeValue("eCTRLMouthOpenWide", markersManager.OpeningRemapped    * PerturbationOpening    * 100f );
            CharacterManager.SetBlendshapeValue("PHMMouthWidth",      markersManager.StretchingRemapped * PerturbationStretching * (1f - StretchingVsSmile) *100f );
            CharacterManager.SetBlendshapeValue("eCTRLMouthSmile",      markersManager.StretchingRemapped * PerturbationStretching * StretchingVsSmile * 100f );
            CharacterManager.SetBlendshapeValue("PHMMouthWidth_NEGATIVE_",      100f - markersManager.StretchingRemapped * PerturbationStretching * 100f );
            CharacterManager.SetBlendshapeValue("eCTRLvW",            markersManager.ProtrusionRemapped * PerturbationProtrusion * 100f );
        }
        else
        {
            CharacterManager.SetBlendshapeValue("eCTRLMouthOpenWide", 0f);
            CharacterManager.SetBlendshapeValue("PHMMouthWidth"     , 0f);
            CharacterManager.SetBlendshapeValue("eCTRLMouthSmile", 0f);
            CharacterManager.SetBlendshapeValue("PHMMouthWidth_NEGATIVE_", 100f);
            CharacterManager.SetBlendshapeValue("eCTRLvW"           , 0f);
        }

        if (RecordData)
        {
            CertusFileManager.AddToFile(System.DateTime.Now + ";" + System.DateTime.UtcNow.Ticks + ";"
                                                     + markersManager.OpeningRemapped + ";"
                                                     + markersManager.OpeningRemapped * PerturbationOpening + ";"
                                                     + markersManager.StretchingRemapped + ";"
                                                     + markersManager.StretchingRemapped * PerturbationStretching + ";"
                                                     + markersManager.ProtrusionRemapped + ";"
                                                     + markersManager.ProtrusionRemapped * PerturbationProtrusion + ";"
                                                     + PerturbationHeadRotation + ";"
                                                     + HeadRigidBody.localEulerAngles.x + ";"
                                                     + HeadRigidBody.localEulerAngles.y + ";"
                                                     + HeadRigidBody.localEulerAngles.z + ";"
                                                     + HeadEffector.localEulerAngles.x + ";"
                                                     + HeadEffector.localEulerAngles.y + ";"
                                                     + HeadEffector.localEulerAngles.z + ";"
                                                     + CurrentRepetition.ToString() + ";"
                                                     + SpeechState);
        }
    }

    public void SetPerturbationOpening(float v)
    {
        PerturbationOpening = v;
    }
    public void SetPerturbationStretching(float v)
    {
        PerturbationStretching = v;
    }
    public void SetPerturbationProtrusion(float v)
    {
        PerturbationProtrusion = v;
    }
}
