using UnityEngine;
using System;
using System.Linq;
using Assets.LSL4Unity.Scripts.AbstractInlets;
using TMPro;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace Assets.LSL4Unity.Scripts.Examples
{
    /// <summary>
    /// NDI Certus Markers Inlet
    /// </summary>
    public class MarkersManager : AFloatInlet
    {
        public Transform[] Markers;
        public TMP_Text MouthOpening, MouthStretching, MouthProtrusion;
        public Slider OpeningSlider, StretchingSlider, ProtrusionSlider;

        public bool Compensation = false;

        public void SetCompensation(bool c)
        {
            Compensation = c;
        }

        public Transform MouthCenter,
            HeadRigidBody,
            MouthOpeningMarker1,
            MouthOpeningMarker2,
            MouthStretchingMarker1,
            MouthStretchingMarker2,
            MouthProtrusionMarker1,
            MouthProtrusionMarker2;

        public float Opening, Stretching, Protrusion, OpeningRemapped, StretchingRemapped, ProtrusionRemapped, MinOpening, MaxOpening, MinStretching, MaxStretching, MinProtrusion, MaxProtrusion;

        string lastSample = String.Empty;

        float[] Data;

        protected override void Process(float[] newSample, double timeStamp)
        {
            // just as an example, make a string out of all channel values of this sample
            lastSample = string.Join(" ", newSample.Select(c => c.ToString()).ToArray());

            Data = newSample;

            for (int i = 0; i < Markers.Length; i++)
            {
                try
                {
                    Vector3 _position = new Vector3(newSample[i * 3], newSample[i * 3 + 1], newSample[i * 3 + 2]);
                    if (_position != Vector3.zero) Markers[i].localPosition = _position;
                }
                catch (Exception)
                {
                    Debug.Log("Invalid data received!" + newSample.ToString());
                    Markers[i].localPosition = Vector3.zero;
                }
            }

            
            if (MouthCenter.position == Vector3.zero && HeadRigidBody.position != Vector3.zero)
            {
                MouthCenter.position = Vector3.Lerp(MouthStretchingMarker1.position, MouthStretchingMarker2.position, .5f);
                ParentConstraint constraint = MouthCenter.GetComponent<ParentConstraint>();
                //constraint.translationOffsets[0] = MouthCenter.position - HeadRigidBody.position;
                //Debug.LogWarning(HeadRigidBody.position);
                //Debug.LogWarning(MouthCenter.position);
                //Debug.LogWarning(MouthCenter.position - HeadRigidBody.position);
                constraint.SetTranslationOffset(0, MouthCenter.localPosition - HeadRigidBody.localPosition);
                constraint.constraintActive = true;
            }

            Opening = Vector3.Distance(MouthOpeningMarker1.position, MouthOpeningMarker2.position);
            MouthOpening.text = Opening.ToString();
            Stretching = Vector3.Distance(MouthStretchingMarker1.position, MouthStretchingMarker2.position);
            MouthStretching.text = Stretching.ToString();
            Protrusion = Vector3.Distance(MouthProtrusionMarker1.position, MouthCenter.position);
            MouthProtrusion.text = Protrusion.ToString();
            //Debug.Log(string.Format("Got {0} samples at {1}", newSample.Length, timeStamp));
            
            #region Calibration

            //Reset Calibration
            if (Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.R))
            {
                MaxOpening = 0f;
                MinOpening = 1f;
                MaxStretching = 0f;
                MinStretching = 1f;
                MaxProtrusion = 0;
                MinProtrusion = 1f;
            }

            //Calibrate Opening
            if (Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.O))
            {
                if (Opening > MaxOpening)
                {
                    MaxOpening = Opening;
                }

                if (Opening < MinOpening)
                {
                    MinOpening = Opening;
                }
            }

            if (Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.S))
            {
                if (Stretching > MaxStretching)
                {
                    MaxStretching = Stretching;
                }

                if (Stretching < MinStretching)
                {
                    MinStretching = Stretching;
                }
            }

            //Calibrate Protrusion
            if (Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.P))
            {
                if (Protrusion > MaxProtrusion)
                {
                    MaxProtrusion = Protrusion;
                }

                if (Protrusion < MinProtrusion)
                {
                    MinProtrusion = Protrusion;
                }
            }
            #endregion

            OpeningRemapped = Mathf.InverseLerp(MinOpening, MaxOpening, Opening);
            StretchingRemapped = Mathf.InverseLerp(MinStretching, MaxStretching, Stretching);
            ProtrusionRemapped = Mathf.InverseLerp(MinProtrusion, MaxProtrusion, Protrusion);
            
            if (Compensation)
            {
                float maxValue = Mathf.Max( Mathf.Max(OpeningRemapped, StretchingRemapped), ProtrusionRemapped);

                if (maxValue == ProtrusionRemapped)
                {
                    OpeningRemapped = 2 * OpeningRemapped - maxValue;
                }
                               
                //StretchingRemapped = 2 * StretchingRemapped - maxValue;
                //ProtrusionRemapped = 2 * ProtrusionRemapped - maxValue;
            }

            OpeningSlider.value = OpeningRemapped;
            StretchingSlider.value = StretchingRemapped;
            ProtrusionSlider.value = ProtrusionRemapped;
        }
    }
}