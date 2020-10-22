using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
    public bool isDebug;//デバッグ用
    public Text userNameLabel;
    public Text highScoreLabel;
    public Text inputUserName;

    //通信状態表示用文字列
    public Text networkState;

    //ランキング情報表示用のパネル
    public GameObject rankingTablePanel;

    //1位～3位の表示欄が入った配列
    public GameObject[] rankingRows;



    // Start is called before the first frame update
    void Start()
    {
        //デバッグ用
        //インスペクタビューの「isDebug」にチェックを入れるとゲーム起動毎にデータが消える
        if (isDebug)
        {
            //isDebugにチェックが入っていたら、すべてのPlayerPrefsを削除
            PlayerPrefs.DeleteAll();
        }


        //ハイスコアを表示
        highScoreLabel.text = "High Score : " + PlayerPrefs.GetInt("HighScore").ToString() + "m";

        //登録しているユーザー名を表示
        UpdateUserName();

        //取得したConnectManagerから情報を引き出し、各種画面表示を行う
        StartCoroutine(InitWebUI());
    }

    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("NejikoRun");
    }

    //登録ボタンを押したときの処理

    public void OnSubmitButtonClicked()
    {
        //正常に入力されていたらユーザー名を更新
        if (inputUserName.text.Length < 7)
        {
            PlayerPrefs.SetString("Name", inputUserName.text);
            UpdateUserName();
        }
        else
        {
            Debug.Log("文字数:" + inputUserName.text.Length);
            userNameLabel.text = "6文字を超えています";
        }
    }

    //ユーザー名の画面表示。更新処理
    void UpdateUserName()
    {
        string name = PlayerPrefs.GetString("Name");
        Debug.Log("TitleController : 現在登録されているユーザー名/" + name);

        //初回起動時（プレイヤー名が登録されていない時）や未入力で登録時、ユーザー名を「Noname」に
        if (string.IsNullOrEmpty(name))
        {
            PlayerPrefs.SetString("name", "Noname");
            name = "Noname";
        }
        //ユーザー名入力フォームに現在のユーザー名を入れておく
        userNameLabel.text = "ユーザー名 :" + name;


    }

    //サーバへ接続し、取得した情報を元にUIを表示させるコルーチン

    //<returns></returns>
    IEnumerator InitWebUI()
    {
        //ネットワークに接続、ランキング情報を取得
        yield return StartCoroutine(ConnectManager.FindMileage());
        //情報に合わせて各種画面表示
        ShowWebUI();
    }

    /// <summary>
    /// 通信状態・ランキング情報をシーンへ表示するメソッド
    /// </summary>
    void ShowWebUI()
    {
        string baseText = "state:";

        //エラーならランキングを表示しない
        if (ConnectManager.IsError)
        {
            networkState.enabled = true;
            networkState.text = baseText + "通信エラーが発生しました";

        }
        else
        {
            networkState.enabled = true;
            networkState.text = baseText + "ランキング取得完了";
            WriteRankingTable();
        }
    }
    /// <summary>
    /// ランキング情報部分の表示処理
    /// </summary>

    void WriteRankingTable()
    {
        //各順位の情報を取得
        List<NejikoMileage> mileages = ConnectManager.Result;

        //表示用パネルをアクティブ化
        rankingTablePanel.SetActive(true);

        //各順位の情報を表示
        for (int i = 0; i < mileages.Count; i++)
        {
            ShowRankingRow(i);
            WriteRankingRow(rankingRows[i], mileages[i]);
        }
    }

    /// <summary>
    /// 指定したindexのランキング情報をアクティブ化するメソッド
    /// </summary>
    /// <param name="index"></param>
    /// 

    void ShowRankingRow(int index)
    {
        rankingRows[index].SetActive(true);
    }

    /// <summary>
    /// 指定したcolumに指定したmileageの情報を表示するメソッド
    /// </summary>
    /// <param name="row">走行記録の情報を表示する行</param>
    ///<param name="mileage">ランキングから取り出した走行記録</param>

    void WriteRankingRow(GameObject row, NejikoMileage mileage)
    {
        Text[] topics = row.GetComponentsInChildren<Text>();
        //Name.Scoreの表示内容をrecordどおりに変更
        topics[1].text = mileage.name;
        topics[2].text = mileage.score.ToString();
    }


}
