using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public NejikoController nejiko;
    public Text scoreLabel;
    public LifePanel lifePanel;

    /// <summary>
    /// 「ハイスコア更新！」　表示用テキスト
    /// </summary>
    public Text UpdateInfoLabel;

    /// <summary>
    /// 「データベースに登録しました」　表示用テキスト
    /// </summary>
    /// 
    public Text RegistInfoLabel;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //スコアラベルを更新
        int score = CalcScore();
        scoreLabel.text = "Score : " + score + "m";
        //ライフパネルを更新
        lifePanel.UpdateLife(nejiko.Life());

        //ねじ子のライフが０になったらゲームオーバー
        if(nejiko.Life() <= 0)
        {
            //これ以降のUpdateは止める
            enabled = false;


            //コルーチンにしたゲーム終了処理を開始
            StartCoroutine(ReturnToTitle(score));


            ////ハイスコア更新
            //if(PlayerPrefs.GetInt("HighScore") < score)
            //{
            //    PlayerPrefs.SetInt("HighScore", score);
            //}

            //// ２秒後にReturnToTitleを呼び出す
            //Invoke("ReturnToTitle", 2.0f);
        }
    }

    int CalcScore()
    {
        //ネジコの走行距離をスコアとする
        return (int)nejiko.transform.position.z;
    }


    /// <summary>
    /// ハイスコアの登録処理を行い、タイトルへ戻るコルーチン
    /// 
    /// </summary>
    /// <param name="score">今回の走行距離</param>
    /// <returns></returns>
    IEnumerator ReturnToTitle(int score)    
    {
       
        //ハイスコアを更新
        if (PlayerPrefs.GetInt("HighScore") < score)
        {
            PlayerPrefs.SetInt("HighScore", score);

            //「ハイスコア更新！」の文字列を表示
            UpdateInfoLabel.enabled = true;

            //走行記録をDBへ書き込み
            yield return StartCoroutine(ConnectManager.RegistMileage());

            //正常に書き込み出来ていたらゲーム画面に通知を表示
            if (!ConnectManager.IsError)
            {
                RegistInfoLabel.enabled = true;
            }
        }

        //3秒待機
        yield return new WaitForSeconds(3.0f);
        //タイトルシーンに戻る
        SceneManager.LoadScene("Title");
    }
}
