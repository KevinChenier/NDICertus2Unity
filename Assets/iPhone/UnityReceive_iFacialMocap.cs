using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Globalization;

public class UnityReceive_iFacialMocap : MonoBehaviour
{
	static UdpClient udp;
	Thread thread;
	SkinnedMeshRenderer meshTarget;
	List<SkinnedMeshRenderer> meshTargetList;
	string faceObjGrpName = "";
	string objectNames = "";
	List<GameObject> objectArray;
	GameObject currentCharacter;

	private string messageString = "";
	public int LOCAL_PORT = 50003;
	public bool sendDataToMCS = false;
	public List<GameObject> iFacialMocapCharacters;

	GameObject GetCharacter(string name)
    {
		foreach(GameObject character in iFacialMocapCharacters)
        {
			if (character.name.Equals(name))
            {
				Debug.Log("Character found!: " + name);
				return character;
			}
        }
		Debug.Log("No character found");
		return null;
    }

	// Start is called 
	void Start()
    {
		Initialize();
    }

    public void Initialize()
    {
        Debug.Log("Socket UDP initialized");
        udp = new UdpClient(LOCAL_PORT);
        udp.Client.ReceiveTimeout = 500;

        thread = new Thread(new ThreadStart(ThreadMethod));
        thread.Start();
    }

    // Update is called once per frame
    void Update()
	{
		try
		{
			string[] strArray1 = messageString.Split(new Char[] { '=' });

			if (strArray1.Length == 2)
			{
				//blendShapes
				foreach (string message in strArray1[0].ToString().Split(new Char[] { '|' }))
				{
					string[] strArray2 = message.Split(new Char[] { '-' });

					if (strArray2.Length == 1)
					{
						string[] strArray3 = strArray2.GetValue(0).ToString().Split(new Char[] { '!' });

						if (strArray3.GetValue(0).ToString() == "faceObjGrp")
						{
							if (faceObjGrpName != strArray3[1].ToString())
							{
								faceObjGrpName = strArray3[1].ToString();

								GameObject faceObjGrp = GetCharacter(faceObjGrpName);
								if (faceObjGrp != null)
								{
									foreach (GameObject character in iFacialMocapCharacters)
									{
										character.SetActive(false);
									}
									currentCharacter = faceObjGrp;
									currentCharacter.SetActive(true);

									List<GameObject> list = GetAllChildren.GetAll(faceObjGrp);

									meshTargetList = new List<SkinnedMeshRenderer>();
									foreach (GameObject obj in list)
									{
										meshTarget = obj.GetComponent<SkinnedMeshRenderer>();
										if (meshTarget != null)
										{
											meshTargetList.Add(meshTarget);
										}
									}
								}
							}
						}
					}
					else if (strArray2.Length == 2)
					{
						string mappedShapeName = strArray2.GetValue(0).ToString();
						float weight = float.Parse(strArray2.GetValue(1).ToString());

						for (int i = 0; i < meshTargetList.Count; i++)
						{
							int index = meshTargetList[i].sharedMesh.GetBlendShapeIndex(mappedShapeName);
							if (index > -1)
							{
								meshTargetList[i].SetBlendShapeWeight(index, weight);
							}
						}
					}
				}
				string objectNamesNow = GetCombineNames(strArray1[1].ToString());

				if (objectNamesNow != objectNames)
				{
					UpdateObjectArray(strArray1[1]);
				}

				int objectArrayIndex = 0;
				//jointNames
				foreach (string message in strArray1[1].ToString().Split(new Char[] { '|' }))
				{
					string[] strArray2 = message.Split(new Char[] { '#' });

					if (strArray2.Length == 2)
					{
						string[] commaList = strArray2[1].Split(new Char[] { ',' });
						string[] objNameList = commaList[3].Split(new Char[] { ':' });
						for (int j = 0; j < objNameList.Length; j++)
						{
							if (objectArray[objectArrayIndex] != null)
							{
								if (strArray2[0].Contains("Position"))
								{
									objectArray[objectArrayIndex].transform.localPosition = new Vector3(float.Parse(commaList[0], CultureInfo.InvariantCulture), float.Parse(commaList[1], CultureInfo.InvariantCulture), float.Parse(commaList[2], CultureInfo.InvariantCulture));
								}
								else
								{
									objectArray[objectArrayIndex].transform.localRotation = Quaternion.Euler(float.Parse(commaList[0], CultureInfo.InvariantCulture), float.Parse(commaList[1], CultureInfo.InvariantCulture), float.Parse(commaList[2], CultureInfo.InvariantCulture));
								}
							}
							objectArrayIndex++;
						}
					}
				}
			}
		}
		catch
		{ }
	}

	void ThreadMethod()
	{
		while (true)
		{
			try
			{
				IPEndPoint remoteEP = null;
				byte[] data = udp.Receive(ref remoteEP);
				messageString = Encoding.ASCII.GetString(data);
			}
			catch
			{
			}
		}
	}

	private string GetCombineNames(string strArray)
	{
		string combineStr = "";
		foreach (string message in strArray.ToString().Split(new Char[] { '|' }))
		{
			if (message != "")
			{
				string[] strArray1 = message.Split(new Char[] { '#' });
				string[] commaList = strArray1[1].Split(new Char[] { ',' });
				combineStr += commaList[3];
				combineStr += ",";
			}

		}
		return combineStr;
	}

	private void UpdateObjectArray(string strArray)
	{
		objectArray = new List<GameObject>();
		foreach (string message in strArray.ToString().Split(new Char[] { '|' }))
		{
			if (message != "")
			{
				string[] strArray1 = message.Split(new Char[] { '#' });
				string[] commaList = strArray1[1].Split(new Char[] { ',' });
				string[] objNameList = commaList[3].Split(new Char[] { ':' });
				for (int i = 0; i < objNameList.Length; i++)
				{
					GameObject bufObj = GameObject.Find(objNameList[i]);
					objectArray.Add(bufObj);
				}
			}
		}
	}

	public string GetMessageString()
	{
		return messageString;
	}

	void OnApplicationQuit()
	{
		StopUDP();
	}

    void OnDestroy()
    {
		StopUDP();
	}

    public void StopUDP()
	{
		udp.Close();
		thread.Abort();
	}

	void Stop()
	{
		try
		{
			StopUDP();
		}
		catch (IOException)
		{
		}
	}
}
public static class GetAllChildren
{
	public static List<GameObject> GetAll(this GameObject obj)
	{
		List<GameObject> allChildren = new List<GameObject>();
		allChildren.Add(obj);
		GetChildren(obj, ref allChildren);
		return allChildren;
	}

	public static void GetChildren(GameObject obj, ref List<GameObject> allChildren)
	{
		Transform children = obj.GetComponentInChildren<Transform>();
		if (children.childCount == 0)
		{
			return;
		}
		foreach (Transform ob in children)
		{
			allChildren.Add(ob.gameObject);
			GetChildren(ob.gameObject, ref allChildren);
		}
	}
}