using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ConnectManager
{

    /// <summary>
    /// 接続するサーブレットへのルートパス
    /// </summary>
    const string ROOTPATH = "http://localhost:8080/nejikoRunOnlineServer/NejikoMileageServlet";
    /// <summary>
    /// 

    /// サーブレットから返ってきた文字列
    /// </summary>
    static string responseText = "";
    /// <summary>
    /// 接続中にエラーが起こったかどうかのフラグ
    /// </summary>
    ///
    
    static bool isError = true;


    /// <summary>
    /// データベースから取り出した上位3位の走行記録
    /// </summary>
    /// 

    static List<NejikoMileage> result = new List<NejikoMileage>();

    //プロパティ
    public static bool IsError
    {
        get { return isError; }
    }
    public static List<NejikoMileage> Result
    {
        get { return result; }
    }
    /// <summary>
    /// ランキング情報を取得する際に呼び出すコルーチン
    /// </summary>
    /// <returns></returns>
    public static IEnumerator FindMileage()
    {
        //Webに接続してデータを取得
        yield return GetRequest();
    
    if (isError)
        {
            Debug.Log("GetRanking:通信エラー");
        }
        else
        {
            result = JsonHelper.GetJsonArray<NejikoMileage>(responseText);
            foreach (NejikoMileage nm in result)
            {
                Debug.Log(nm.name + "/" + nm.score);
            }
            Debug.Log("正常終了 取得した配列:" + result + "/配列の長さ:" + result.Count);
        }
    }
    /// <summary>
    /// 走行記録の書き込みを行う際に呼び出すコルーチン
    /// </summary>
    /// <returns></returns>
    public static IEnumerator RegistMileage()
    {


        //ゲーム中のセーブデータをもとに送信用ファイルを生成
        NejikoMileage sendRecord = MakeNejikoMileage();
        //サーブレットへPostリクエストを行い、送信用ファイルの内容を送信
        yield return PostRequest(sendRecord);
        //実行結果に応じてログを表示
        if (isError)
        {

            Debug.Log("SetRecord:通信エラー");
        }
        else
        {
            Debug.Log("SetRecord:正常終了");
        }
    }
    /// <summary>
    /// 実際にサーブレットへGETリクエストを行うコルーチン
    /// </summary>
    /// <returns></returns>
    static IEnumerator GetRequest()
    {
        //変数の初期化
        InitVar();
        UnityWebRequest request = new UnityWebRequest(ROOTPATH);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        //レスポンスが返ってくるまで実行を待つ
        yield return request.SendWebRequest();
        //接続の途中でエラーが起きた場合はログを返す
        if (request.isNetworkError || request.isHttpError)
        {
            isError = true;
            Debug.Log("WebConnectionGet:通信エラー / " + request.error);

            Debug.Log("WebConnectionGet:エラーコード / " + request.responseCode);
        }
        //正常に接続できた場合
        else
        {
            responseText = request.downloadHandler.text;
            isError = false;
            //デバッグ用表示
            Debug.Log("WebConnectGet.サーブレットから受け取ったJSON:" + responseText);

            result = JsonHelper.GetJsonArray<NejikoMileage>(responseText);
            foreach (NejikoMileage nm in result)
            {
                Debug.Log(nm.name + "/" + nm.score);
            }
            Debug.Log("正常終了 取得した配列:" + result + "/配列の長さ:" + result.Count);
        }
    }

    /// <summary>
    /// 実際にサーブレットへPOSTリクエストを行うコルーチン
    /// </summary>
    /// <param name="mileage"></param>
    /// <returns></returns>
    static IEnumerator PostRequest(NejikoMileage mileage)
    {
        //変数を初期化

        InitVar();

        //引数で渡されたクラスの情報を引き出し、送りたいPOSTリクエストのリクエストパラメータに入れる
        WWWForm form = new WWWForm();
        form.AddField("name", mileage.name);
        form.AddField("score", mileage.score);
        UnityWebRequest request = UnityWebRequest.Post(ROOTPATH, form);
        yield return request.SendWebRequest();

        //接続の途中でエラーが起きた場合はログを返す
        if (request.isNetworkError || request.isHttpError)
        {
            isError = true;
            Debug.Log("WebConnectionPost:通信エラー / " + request.error);
            Debug.Log("WebConnectionPost:エラーコード / " + request.responseCode);
        }
        //正常に接続できた場合
        else
        {
            //成功/失敗の文字列が返ってくるので受け取る
            responseText = request.downloadHandler.text;
            //書き込み成功なら
            if (responseText == "SUCCESS")
            {
                Debug.Log("WebConnectionPost:レコード書き込み完了");
                isError = false;

            }

            //失敗なら
            else
            {
                Debug.Log("WebConnectionPost:レコード書き込みエラー");
                isError = true;
            }
        }
    }

    /// <summary>
    /// 取得したJSON文字列、エラー発生フラグの初期化用メソッド
    /// </summary>
    public static void InitVar()
    {
        responseText = "";
        isError = true;
    }

    /// <summary>
    /// ゲーム中のセーブデータをもとに送信用ファイルを生成するメソッド
    /// </summary>
    static NejikoMileage MakeNejikoMileage()
    {

        Debug.Log("name:" + PlayerPrefs.GetString("Name") + "/score:" + PlayerPrefs.GetInt("HighScore"));
        string name = PlayerPrefs.GetString("Name");
        int score = PlayerPrefs.GetInt("HighScore");

        return new NejikoMileage(name, score);
    }
}


