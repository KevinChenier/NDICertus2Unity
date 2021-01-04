using MCS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{

    public MCSCharacterManager[] characters;
    public MorphsManager morphsManager;

    public void SetCharacter(int character)
    {
        Debug.Log(character);
        for (int i = 0; i < characters.Length; i++)
        {
            if (i == character)
            {
                characters[i].gameObject.SetActive(true);
                morphsManager.SetActiveCharacter(characters[i]);
            }
            else
            {
                characters[i].gameObject.SetActive(false);
            }
        }
    }
}
