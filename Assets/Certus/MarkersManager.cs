using UnityEngine;
using System;
using System.Linq;
using Assets.LSL4Unity.Scripts.AbstractInlets;
using TMPro;
using UnityEngine.Animations;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Assets.LSL4Unity.Scripts.Examples
{
    /// <summary>
    /// NDI Certus Markers Inlet
    /// </summary>
    public class MarkersManager : AFloatInlet
    {
        public Transform[] Markers;
        public TMP_Text MouthOpening, MouthStretching, MouthProtrusion, LeftEyebrowRaise, RightEyebrowRaise;
        public Slider OpeningSlider, StretchingSlider, ProtrusionSlider, LeftEyebrowRaiseSlider, RightEyebrowRaiseSlider;

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
            MouthProtrusionMarker2,
            NoseMarker,
            LeftEyebrowMarker,
            RightEyebrowMarker;

        public float Opening, Stretching, Protrusion, LeftEyebrow, RightEyebrow, 
            OpeningRemapped, StretchingRemapped, ProtrusionRemapped, LeftEyebrowRemapped, RightEyebrowRemapped, 
            MinOpening, MaxOpening, MinStretching, MaxStretching, MinProtrusion, MaxProtrusion, MinLeftEyebrow, MaxLeftEyebrow, MinRightEyebrow, MaxRightEyebrow;

        string lastSample = String.Empty;

        float[] Data;

        [Range(0, 5)]
        public float NoiseReduction;
        List<Vector3> LastPositions = new List<Vector3>();

        protected override void Process(float[] newSample, double timeStamp)
        {
            // just as an example, make a string out of all channel values of this sample
            lastSample = string.Join(" ", newSample.Select(c => c.ToString()).ToArray());

            Data = newSample;

            // Initialize LastPositions
            if (LastPositions.Count == 0)
                for (int i = 0; i < Markers.Length; i++)
                    LastPositions.Add(default);

            for (int i = 0; i < Markers.Length; i++)
            {
                try
                {
                    Vector3 _position = new Vector3(newSample[i * 3], newSample[i * 3 + 1], newSample[i * 3 + 2]);

                    if (_position != Vector3.zero && Vector3.Distance(LastPositions[i], _position) > NoiseReduction)
                    {
                        Markers[i].localPosition = _position;
                        LastPositions[i] = _position;
                    }
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
            LeftEyebrow = Vector3.Distance(LeftEyebrowMarker.position, NoseMarker.position);
            LeftEyebrowRaise.text = LeftEyebrow.ToString();
            RightEyebrow = Vector3.Distance(RightEyebrowMarker.position, NoseMarker.position);
            RightEyebrowRaise.text = RightEyebrow.ToString();
            //Debug.Log(string.Format("Got {0} samples at {1}", newSample.Length, timeStamp));

            #region Calibration

            //Reset Calibration
            if (Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.Space))
            {
                MaxOpening = 0f;
                MinOpening = 1f;
                MaxStretching = 0f;
                MinStretching = 1f;
                MaxProtrusion = 0;
                MinProtrusion = 1f;
                MaxLeftEyebrow = 1f;
                MinLeftEyebrow = 0f;
                MaxRightEyebrow = 1f;
                MinRightEyebrow = 0f;
            }

            //Calibrate Opening
            if (Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.O))
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    MaxOpening = Opening;
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    MinOpening = Opening;
                }
            }

            if (Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.S))
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    MaxStretching = Stretching;
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    MinStretching = Stretching;
                }
            }

            //Calibrate Protrusion
            if (Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.P))
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    MaxProtrusion = Protrusion;
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    MinProtrusion = Protrusion;
                }
            }

            //Calibrate Eyebrows
            if (Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.E))
            {
                if (Input.GetKey(KeyCode.L)) 
                {
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        MaxLeftEyebrow = LeftEyebrow;
                    }

                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        MinLeftEyebrow = LeftEyebrow;
                    }
                }

                if (Input.GetKey(KeyCode.R))
                {
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        MaxRightEyebrow = RightEyebrow;
                    }

                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        MinRightEyebrow = RightEyebrow;
                    }
                }
            }
            #endregion

            OpeningRemapped = Mathf.InverseLerp(MinOpening, MaxOpening, Opening);
            StretchingRemapped = Mathf.InverseLerp(MinStretching, MaxStretching, Stretching);
            ProtrusionRemapped = Mathf.InverseLerp(MinProtrusion, MaxProtrusion, Protrusion);
            LeftEyebrowRemapped = Mathf.InverseLerp(MinLeftEyebrow, MaxLeftEyebrow, LeftEyebrow);
            RightEyebrowRemapped = Mathf.InverseLerp(MinRightEyebrow, MaxRightEyebrow, RightEyebrow);

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
            LeftEyebrowRaiseSlider.value = LeftEyebrowRemapped;
            RightEyebrowRaiseSlider.value = RightEyebrowRemapped;
        }
    }
}