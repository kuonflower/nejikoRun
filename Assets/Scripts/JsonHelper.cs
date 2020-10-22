using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// クラスの配列をJsonUtilityでデシリアライズするヘルパー
/// http://forum.unity3d.com/threads/how-to-load-an-array-with-jsonutility.375735/
/// </summary>
public class JsonHelper
{
/// <summary>
/// 配列になっているJSONデータをデシリアライズする
/// </summary>
/// <typeparam name="T">配列になっているデータの型</typeparam>
/// <param name="json">json文字列</param>
/// <returns>FromJsonで生成されたオブジェクトの配列</returns>
/// 

    public static List<T> GetJsonArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        Debug.Log("ジェイソンヘルパー" + newJson);
        return wrapper.array;
    }

    [Serializable]
    private class Wrapper<T>
    {
        //『#pragma~』は、プログラムの動きとは直接関係ない記述
        //(『この番号の警告を無視しろ』というコンパイラへの命令文)
#pragma warning disable 649

        public List<T> array;
    }
}
