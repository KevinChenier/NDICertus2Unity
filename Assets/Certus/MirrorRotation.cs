using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MirrorRotation : MonoBehaviour
{
    public Transform Source;
    public Transform Headset;
    public Transform CertusHeadRigidBody;
    public Transform Target;
    public Vector3 Position;
    public float TranslationMultiplyer = 1f;

    public RotationConstraint rotationConstraint;

    public float RotationMultiplier_Start = 1f, RotationMultiplier_End = 1f;

    public bool CalculatePosition = false;

    void Start()
    {   
       Invoke("StartPositionTracking", .5f);
    }

    public void SetRotationMultiplier_Start(string f)
    {
        f = f.Replace(".", ",");
        try
        {
            RotationMultiplier_Start = float.Parse(f);
            rotationConstraint.weight = RotationMultiplier_Start;
        }
        catch (System.Exception)
        {
            RotationMultiplier_Start = 1f;
            rotationConstraint.weight = RotationMultiplier_Start;
        }
        

    }
    public void SetRotationMultiplier_End(string f)
    {
        f = f.Replace(".", ",");
        try
        {
            RotationMultiplier_End = float.Parse(f);
            //rotationConstraint.weight = RotationMultiplier_Start;
        }
        catch (System.Exception)
        {
            RotationMultiplier_End = 1f;
        }
        

    }
    //public void SetHorizontalRotationMultiplier(string f) { HorizontalRotationMultiplier = float.Parse(f); }

    public void SetSourceFromHeadset(bool b)
    {
        if (b)
        {
            Position = Headset.position;
            Source = Headset;

        }
        else
        {
            Position = CertusHeadRigidBody.position;
            Source = CertusHeadRigidBody;
        }
        
    }

    public void StartPositionTracking()
    {
        Position = Source.position;
        CalculatePosition = true;
    }

    void Update()
    {
        //transform.rotation = Quaternion.Inverse(Source.rotation);
        transform.localEulerAngles = new Vector3( -Source.localEulerAngles.x, -Source.localEulerAngles.y, Source.localEulerAngles.z);

        if (CalculatePosition)
        {   
            Vector3 newPosition = Source.transform.position;
            Vector3 translation = newPosition - Position;

            Vector3 _translation = new Vector3(translation.x, translation.y, -translation.z);
            transform.Translate(_translation);
            Target.position += _translation * TranslationMultiplyer;

            Position = newPosition;
        }
        
    }
}
