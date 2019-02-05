using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// TODO:
/// Broke file onto separate files for each IO type\method
/// Create Manager for choosing data type
/// Create UI for IO process operating => choose data type, choose action (save\load), check if file exists, rewrite or not?
/// If possible: create smth like FileManager (TotalCommander) for Unity. It have to: search files inside directory, get list of existent files
/// Rewrite binary Save\Load methods (for consistent file type)
/// Rewrite FileStream Operations (add using () {} )

public static class ExportMap {

    // Неожиданно, но скорее всего Unity UI Elements не умеют вызывать статические методы. Соответственно, нужен Singleton с public методами, которые вызывают статические методы.
    // Костыльный движок, нет, ну серьезно? Фичи вводим, баги не чиним? Или в чем причина такого поведения?

    public static System.IO.FileStream OpenFileStream(string folderName, string fileName, string fileExtension, bool isReadMode)
    {
        if (isReadMode)
        {
            return System.IO.File.OpenRead(GetFilePath(folderName, fileName, fileExtension));
        }
        else
        {
            return System.IO.File.OpenWrite(GetFilePath(folderName, fileName, fileExtension));
        }
    }

    public static string GetFilePath(string folderName, string fileName, string fileExtension)
    {
        string filePath = System.IO.Path.Combine(Application.dataPath, folderName);
        filePath = System.IO.Path.Combine(filePath, fileName + fileExtension);
        return filePath;
    }

    public static bool IsFileExist(string folderName, string fileName, string fileExtension)
    {
        string filePath = System.IO.Path.Combine(Application.dataPath, folderName);
        filePath = System.IO.Path.Combine(filePath, fileName + fileExtension);
        if (System.IO.File.Exists(filePath))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsDirectoryExist(string folderName)
    {
        string filePath = System.IO.Path.Combine(Application.dataPath, folderName);
        if (System.IO.Directory.Exists(filePath))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void CreateDirectory(string folderName)
    {
        string filePath = System.IO.Path.Combine(Application.dataPath, folderName);
        System.IO.Directory.CreateDirectory(filePath);
        Debug.Log("Directory was created: " + filePath);
    }

    public static void ExportGraphic(EmptyGrid map)
    {
        string folderName = "SavedMaps";

        string fileName = "check";

        string fileExtension = ".png";

        string pathToPNG = System.IO.Path.Combine(Application.dataPath, folderName);
        //Maybe it's better to use         Application.persistentDataPath

        Debug.Log("The path is: " + pathToPNG);

        if (!System.IO.Directory.Exists(pathToPNG))
        {
            System.IO.Directory.CreateDirectory(pathToPNG);
            Debug.Log("Directory was created: " + pathToPNG);
        }

        pathToPNG = System.IO.Path.Combine(pathToPNG, fileName + fileExtension);      

        Debug.Log("Current path is: " + pathToPNG);

        Texture2D mapTexture = new Texture2D(map.width, map.height, TextureFormat.RGBA32, false);

        for (int z = 0; z < map.height; z++)
        {
            for (int x = 0; x < map.width; x++)
            {
                mapTexture.SetPixel(x, z, map.values[x, z].cellMapColor);
            }
        }

        mapTexture.Apply();

        byte[] pngInBytes = ImageConversion.EncodeToPNG(mapTexture);

        if (pngInBytes == null)
        {
            Debug.LogError("Map is empty.");
            return;
        }

        // создает файл, если того не существует, иначе - открывает
        System.IO.FileStream saveFileStream = new System.IO.FileStream(pathToPNG, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
        // System.IO.File.Open(pathToPNG, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write) - Открывает FileStream в заданном пути с заданным режимом и доступом.

        System.IO.BinaryWriter binaryWriter = new System.IO.BinaryWriter(saveFileStream);

        // Same as 
        //System.IO.BinaryWriter binaryWriter = new System.IO.BinaryWriter(System.IO.File.Open(pathToPNG, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write));

        // System.IO.File.WriteAllBytes(pathToPNG, pngInBytes); - аналогично и легче

        binaryWriter.Write(pngInBytes);
        saveFileStream.Close();

#if UNITY_EDITOR
        //UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.Default);
        UnityEditor.AssetDatabase.ImportAsset(pathToPNG, UnityEditor.ImportAssetOptions.Default);
#endif
    }

    public static EmptyGrid ImportGraphic(string folderName, string fileName, string fileExtension)
    {
        folderName = "SavedMaps";

        fileName = "check";

        fileExtension = ".png";

        string pathToPNG = System.IO.Path.Combine(Application.dataPath, folderName);
        pathToPNG = System.IO.Path.Combine(pathToPNG, fileName + fileExtension);

        if (System.IO.File.Exists(pathToPNG))
        {
            // Open the file to read from.
            Cell[,] cellMap;

            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);

            //Resources.Load<Texture2D>("/Map/Save");

            byte[] pngInBytes = System.IO.File.ReadAllBytes(pathToPNG);

            // Create a texture. Texture size does not matter, since
            // LoadImage will replace with with incoming image size.
            ImageConversion.LoadImage(texture, pngInBytes);          

            cellMap = new Cell[texture.width, texture.height];

            int counter = 0;
            int counter2 = 0;

            for (int z = 0; z < texture.height; z++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    cellMap[x, z] = Cell.CreateCell(texture.GetPixel(x, z));

                    if (counter2 < 175)
                    {
                        Debug.Log(texture.GetPixel(x, z));
                        counter2++;
                    }

                    if (counter < 10 && cellMap[x, z].cellMapColor == Color.grey)
                    {
                        Debug.Log("Position: " + x + " " + z + " Current color: " + cellMap[x, z].cellMapColor);
                        counter++;
                    }

                    //Debug.Log("Position: " + x + " " + z + " Current color: " + cellMap[x, z].cellMapColor);
                }
            }

            EmptyGrid map = new EmptyGrid(cellMap, cellMap.GetLength(0), cellMap.GetLength(1));

            return map;
        }
        else
        {
            Debug.LogError("Can't find file with such name: " + pathToPNG);
            return null;
        }      
    }


    public static void ExportText(EmptyGrid map)
    {
        string folderName = "SavedMaps";

        string fileName = "textMap";

        string fileExtension = ".txt";

        string pathToTXT = System.IO.Path.Combine(Application.dataPath, folderName);

        Debug.Log("The path is: " + pathToTXT);

        if (!System.IO.Directory.Exists(pathToTXT))
        {
            System.IO.Directory.CreateDirectory(pathToTXT);
            Debug.Log("Directory was created: " + pathToTXT);
        }

        pathToTXT = System.IO.Path.Combine(pathToTXT, fileName + fileExtension);

        Debug.Log("Current path is: " + pathToTXT);

        // Этот метод эквивалентен методу FileStream(String, FileMode, FileAccess, FileShare) перегрузку конструктора с режимом файл OpenOrCreate, права доступа, заданные Write, и задайте режима общего доступа None.
        System.IO.FileStream saveFileStream = System.IO.File.OpenWrite(pathToTXT);
        System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(saveFileStream);

        for (int z = 0; z < map.height; z++)
        {
            for (int x = 0; x < map.width; x++)
            {
                streamWriter.Write(map.values[x, z].cellMapChar);
            }
            streamWriter.WriteLine();
        }

        streamWriter.Close();
        saveFileStream.Close();

#if UNITY_EDITOR
        //UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.Default);
        UnityEditor.AssetDatabase.ImportAsset(pathToTXT, UnityEditor.ImportAssetOptions.Default);
#endif
    }

    public static EmptyGrid ImportText(string folderName, string fileName, string fileExtension)
    {
        folderName = "SavedMaps";

        fileName = "textMap";

        fileExtension = ".txt";

        string pathToTXT = System.IO.Path.Combine(Application.dataPath, folderName);
        pathToTXT = System.IO.Path.Combine(pathToTXT, fileName + fileExtension);

        if (System.IO.File.Exists(pathToTXT))
        {
            // Open the file to read from.
            Cell[,] cellMap;

            System.IO.FileStream loadFileStream = System.IO.File.OpenRead(pathToTXT);
            System.IO.StreamReader streamReader = new System.IO.StreamReader(loadFileStream);

            int lineLength = 0;
            int lineCount = 0;

            List<string> list = new List<string>();

            string str = streamReader.ReadLine();

            lineLength = str.Length;
            lineCount++;
            list.Add(str);

            while (true)
            {
                str = streamReader.ReadLine();
                if (!System.String.IsNullOrEmpty(str))
                {
                    lineCount++;
                    list.Add(str);
                }
                else
                {
                    break;
                }
            }

            streamReader.Close();
            loadFileStream.Close();

            cellMap = new Cell[lineLength, lineCount];

            for (int z = 0; z < lineCount; z++)
            {
                char[] charArray = list[z].ToCharArray();
                for (int x = 0; x < lineLength; x++)
                {
                    cellMap[x, z] = Cell.CreateCell(charArray[x]);     
                }
            }

            EmptyGrid map = new EmptyGrid(cellMap, cellMap.GetLength(0), cellMap.GetLength(1));

            return map;
        }
        else
        {
            Debug.LogError("Can't find file with such name: " + pathToTXT);
            return null;
        }
    }

    public static void SaveDataBinary(EmptyGrid map)
    {
        string folderName = "Saves";
        string fileName = "data";
        string fileExtension = ".sav";

        string pathToTXT = System.IO.Path.Combine(Application.dataPath, folderName);
        Debug.Log("The path is: " + pathToTXT);

        if (!System.IO.Directory.Exists(pathToTXT))
        {
            System.IO.Directory.CreateDirectory(pathToTXT);
            Debug.Log("Directory was created: " + pathToTXT);
        }

        pathToTXT = System.IO.Path.Combine(pathToTXT, fileName + fileExtension);
        Debug.Log("Current path is: " + pathToTXT);

        using (System.IO.FileStream saveFileStream = System.IO.File.OpenWrite(pathToTXT))
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            /// then - get and prepare data method, but I skipped it
            ///
            /// [System.Serializable]
            /// public class SavedData {
            /// public int[] stats;
            /// public SavedData(PlayerData data)
            /// { stats = new int[2];
            /// stats[0] = data.health;
            /// stats[1] = data.level; } };
            /// 
            /// here
            /// SavedData data = new SavedData(GatherAllData()); // or CollectData or StoreData
            /// 
            /// binaryFormatter.Serialize(saveFileStream, data);

            binaryFormatter.Serialize(saveFileStream, map.values);

            saveFileStream.Close();
        } 

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.ImportAsset(pathToTXT, UnityEditor.ImportAssetOptions.Default);
#endif
    }

    // public static SavedData LoadDataBinary()
    public static EmptyGrid LoadDataBinary()
    {
        string folderName = "Saves";
        string fileName = "data";
        string fileExtension = ".sav";

        string pathToData = System.IO.Path.Combine(Application.dataPath, folderName);
        pathToData = System.IO.Path.Combine(pathToData, fileName + fileExtension);

        if (System.IO.File.Exists(pathToData))
        {
            System.IO.FileStream loadFileStream = System.IO.File.OpenRead(pathToData);

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            ///SavedData data = (PlayerData)binaryFormatter.Deserialize(saveFileStream);
            
            Cell[,] data = (Cell[,])binaryFormatter.Deserialize(loadFileStream);
            loadFileStream.Close();
            /// return data;
            /// 
            EmptyGrid map = new EmptyGrid(data, data.GetLength(0), data.GetLength(1));

            return map;
        }
        else
        {
            Debug.LogError("Can't find file with such name: " + pathToData);
            return null;
        }
    }

    public static void ExportJSON(EmptyGrid map)
    {
        string folderName = "SavedMaps";
        string fileName = "jsonMap";
        string fileExtension = ".json";

        string pathToJSON = System.IO.Path.Combine(Application.dataPath, folderName);
        Debug.Log("The path is: " + pathToJSON);

        if (!System.IO.Directory.Exists(pathToJSON))
        {
            System.IO.Directory.CreateDirectory(pathToJSON);
            Debug.Log("Directory was created: " + pathToJSON);
        }

        pathToJSON = System.IO.Path.Combine(pathToJSON, fileName + fileExtension);
        Debug.Log("Current path is: " + pathToJSON);

        string jsonData = JsonUtility.ToJson(new EmptyGridWrapJSON(map));

        /// Works only with primitives
        //System.Buffer.BlockCopy(map.values, 0, flatArray, 0, System.Buffer.ByteLength(map.values));

        //Cell[] flatArray = new Cell[map.width * map.height];
        //int i = 0;
        //for (int z = 0; z < map.height; z++)
        //{
        //    for (int x = 0; x < map.width; x++)
        //    {
        //        flatArray[i] = map.values[x, z];
        //        i++;
        //    }
        //}
        //string jsonData = JsonHelper.ToJson(flatArray);

        Debug.Log(jsonData);

        //using (System.IO.StreamWriter streamWriter = System.IO.File.CreateText(pathToJSON))
        //{           
        //    streamWriter.Write(jsonData);
        //    streamWriter.Close();
        //}    

        System.IO.File.WriteAllText(pathToJSON, jsonData);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.ImportAsset(pathToJSON, UnityEditor.ImportAssetOptions.Default);
#endif
    }

    public static EmptyGrid ImportJSON(string folderName, string fileName, string fileExtension)
    {
        folderName = "SavedMaps";
        fileName = "jsonMap";
        fileExtension = ".json";

        string pathToJSON = System.IO.Path.Combine(Application.dataPath, folderName);
        pathToJSON = System.IO.Path.Combine(pathToJSON, fileName + fileExtension);

        if (System.IO.File.Exists(pathToJSON))
        {
            // Open the file to read from.
            string jsonData = System.IO.File.ReadAllText(pathToJSON);

            EmptyGridWrapJSON cellMapWrapped = JsonUtility.FromJson<EmptyGridWrapJSON>(jsonData);

            EmptyGrid map = cellMapWrapped.Unwrap();

            return map;
        }
        else
        {
            Debug.LogError("Can't find file with such name: " + pathToJSON);
            return null;
        }
    }

}
