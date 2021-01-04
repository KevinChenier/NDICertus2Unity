using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CertusFileManager
{
    public static string FilePath;

    public static void AddToFile(string s)
    {
        StreamWriter writer = new StreamWriter(FilePath, true);
        writer.WriteLine(s);
        writer.Close();
    }

    public static void AddToFile(string filePath, string s)
    {
        StreamWriter writer = new StreamWriter(filePath, true);
        writer.WriteLine(s);
        writer.Close();
    }
}
