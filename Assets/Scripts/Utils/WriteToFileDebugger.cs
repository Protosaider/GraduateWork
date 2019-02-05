using UnityEngine;
using System.Collections;
using System.IO;

public class WriteToFileDebugger {

    public static string filepath;

    /// <summary>
    /// Write a string to a file
    /// </summary>
    /// <param name="fileName">File to write to.</param>
    /// <param name="content">String to write.</param>
    /// <param name="append">If set to true, append. If set to false, overwrite file.</param>
    public static void WriteStringToFile(string fileName, string content, bool append)
    {       
        StreamWriter sw = new StreamWriter(fileName, append);
        sw.WriteLine(content);
        sw.Close();
    }


    public static string GetFilePath(string folderName, string fileName, string fileExtension)
    {
        string filePath = System.IO.Path.Combine(Application.dataPath, folderName);
        filePath = System.IO.Path.Combine(filePath, fileName + fileExtension);
        return filePath;
    }
}
