using UnityEngine;
using System.IO;
//参考元：https://marumaro7.hatenablog.com/entry/litjsonjagged
using LitJson;
using System;

public class LocalData
{
    /// <summary>
    /// ローカルファイルにデータを保存する
    /// </summary>
    /// <typeparam name="T"> データの型 </typeparam>
    /// <param name="file"> 保存先のファイル名 </param>
    /// <param name="data"> 現在のデータ </param>
    public static void Save<T>(string file, T data)
    {
        Debug.Log("Save");

        StreamWriter writer;
        var json = JsonUtility.ToJson(data);
        using (writer = new StreamWriter(Application.dataPath + "/" + file, false))
        {
            Debug.Log(Application.dataPath + "/" + file);
            writer.Write(json);
            writer.Flush();
            writer.Close();
        }
    }

    /// <summary>
    /// 多次元配列を含むデータをローカルファイルに保存する
    /// </summary>
    /// <typeparam name="T"> データの型 </typeparam>
    /// <param name="file"> 保存先のファイル名 </param>
    /// <param name="data"> 現在のデータ </param>
    public static void SaveMultidimArray<T>(string file, T data)
    {
        Debug.Log("Save");

        StreamWriter writer;
        var json = JsonMapper.ToJson(data);

        using (writer = new StreamWriter(Application.dataPath + "/" + file, false))
        {
            Debug.Log(Application.dataPath + "/" + file);
            writer.Write(json);
            writer.Flush();
            writer.Close();
        }
    }

    public static T Load<T>(string file)
    {
        string datastr;
        StreamReader reader;
        try
        {
            using (reader = new StreamReader(Application.dataPath + "/" + file))
            {
                datastr = reader.ReadToEnd();
                reader.Close();
            }

            var gameData = JsonUtility.FromJson<T>(datastr); // ロードしたデータで上書き

            if (gameData != null)
            {
                Debug.Log(gameData + "のデータをロードしました");

                return gameData;
            }
            else
            {
                return default;
            }
        }
        catch
        {
            Debug.Log("データを取得できませんでした。フォルダを作成します");
            string folderPath = Path.Combine(Application.dataPath, @"SaveData");
            Debug.Log(folderPath);
            Directory.CreateDirectory(folderPath);
            return default;
        }
    }

    /// <summary>
    /// 多次元配列を含むデータをローカルファイルから取得する
    /// </summary>
    /// <param name="filePath"> 保存先のファイル名 </param>

    public static T LoadMultidimArray<T>(string filePath)
    {
        string datastr;
        StreamReader reader;
        try
        {
            using (reader = new StreamReader(filePath))
            {
                datastr = reader.ReadToEnd();
                reader.Close();
            }

            Debug.Log(datastr);
            var data = JsonMapper.ToObject<T>(datastr);

            if (data != null)
            {
                Debug.Log(data + "のデータをロードしました");

                return data;
            }
            else
            {
                return default;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"データを取得できませんでした。ファイルパス：{filePath} {e}");
            return default;
        }
    }
}

