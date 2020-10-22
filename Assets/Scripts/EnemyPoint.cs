using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPoint : MonoBehaviour
{
    public GameObject prefab;
    
    // Start is called before the first frame update
    void Awake()
    {
        //プレファブをどうポジションに生成
        GameObject go = (GameObject)Instantiate(
            prefab,
            Vector3.zero,
            Quaternion.identity
            );

        //一緒に削除されるように生成した敵オブジェクトを子に設定
        go.transform.SetParent(transform, false);
    }

    //ステージエディット中のためにシーンにギズモを表示
    private void OnDrawGizmos()
    {
        //ギズモの底辺が地面と同じ高さになるようにオフセットを設定
        Vector3 offset = new Vector3(0, 0.5f, 0);

        //球を表示
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(transform.position + offset, 0.5f);

        //プレファブ名のアイコンを表示
        if(prefab != null)
        {
            Gizmos.DrawIcon(transform.position + offset, prefab.name, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
