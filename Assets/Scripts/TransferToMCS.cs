using MCS;
using System.Collections;
using System.Collections.Generic;

public class TransferToMCS
{
    MCSCharacterManager CharacterManager;
    static readonly char seperator = '+';

    Dictionary<string, string> blendshapesConversion = new Dictionary<string, string>()
    {
        //Brows
        { "browDownLeft", "eCTRLBrowUp_DownL_NEGATIVE_" },
        { "browDownRight", "eCTRLBrowUp_DownR_NEGATIVE_" },
        { "browInnerUp", "eCTRLBrowInnerUp_Down" },
        { "browOuterUpLeft", "eCTRLBrowOuterUp_DownL" },
        { "browOuterUpRight", "eCTRLBrowOuterUp_DownR" },

        //Cheeks
        { "cheekPuff", "eCTRLCheeksBalloonPucker" },

        //Eyes
        { "eyeBlinkLeft", "eCTRLEyesClosedL" },
        { "eyeBlinkRight", "eCTRLEyesClosedR" },
        { "eyeSquintLeft", "eCTRLEyesSquintL" },
        { "eyeWideLeft", "eCTRLEyelidsLowerUpDownL_NEGATIVE_" + seperator + "eCTRLEyelidsUpperDownUpL_NEGATIVE_" },
        { "eyeWideRight", "eCTRLEyelidsUpperDownUpL_NEGATIVE_" + seperator + "eCTRLEyelidsUpperDownUpR_NEGATIVE_" },

        //Jaw
        { "jawForward", "eCTRLJawOut_In" },

        //Mouth
        { "jawOpen", "eCTRLMouthOpen" /*+ seperator +"eCTRLMouthOpenWide"*/ },
        { "mouthClose", "eCTRLMouthOpen_NEGATIVE_" },
        { "mouthFrownLeft", "eCTRLMouthSmileSimpleL_NEGATIVE_" },
        { "mouthFrownRight", "eCTRLMouthSmileSimpleR_NEGATIVE_" },
        { "mouthFunnel", "eCTRLLipsPuckerWide" },
        { "mouthLowerDownLeft", "eCTRLLipBottomUp_DownL" },
        { "mouthLowerDownRight", "eCTRLLipBottomUp_DownR" },
        { "mouthPucker", "eCTRLLipsPucker" },
        { "mouthRollLower", "eCTRLvM" },
        //{ "mouthRollUpper", "eCTRLvM" },
        { "mouthShrugLower", "eCTRLLipBottomIn_Out" },
        { "mouthShrugUpper", "eCTRLLipTopUp_Down" },
        { "mouthSmileLeft", "eCTRLMouthSmileSimpleL" },
        { "mouthSmileRight", "eCTRLMouthSmileSimpleR" },
        { "mouthStretchLeft", "eCTRLMouthSmileSimpleL_NEGATIVE_" + seperator + "eCTRLMouthNarrowL_NEGATIVE_" },
        { "mouthStretchRight", "eCTRLMouthSmileSimpleR_NEGATIVE_" + seperator + "eCTRLMouthNarrowR_NEGATIVE_" },
        { "mouthUpperUpLeft", "eCTRLLipTopUp_DownL" },
        { "mouthUpperUpRight", "eCTRLLipTopUp_DownR" }
    };

    public TransferToMCS(MorphsManager MorphsManager)
    {
        CharacterManager = MorphsManager.CharacterManager;
    }
    
    public void SetBlendshape(string name, float weight)
    {
        if(blendshapesConversion.ContainsKey(name))
        {
            if (!blendshapesConversion[name].Contains("" + seperator))
            {
                CharacterManager.SetBlendshapeValue(blendshapesConversion[name], weight);
            }
            else
            {
                string[] blendshapes = blendshapesConversion[name].Split(seperator);

                foreach (string blendshape in blendshapes)
                {
                    CharacterManager.SetBlendshapeValue(blendshape, weight);
                }
            }
        }
    }
}
