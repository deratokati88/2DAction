using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CureItem : MonoBehaviour
{
    [Header("回復する量")] public int cureP;
    [Header("プレイヤーの判定")] public PlayerTriggerCheck playerCheck;
    [Header("アイテム取得時に鳴らすSE")] public AudioClip itemSE;

    // Update is called once per frame
    void Update()
    {
        //プレイヤーが判定内に入ったら
        if (playerCheck.isOn)
        {
            if (GManager.instance != null)
            {
                GManager.instance.PlaySE(itemSE);
                GManager.instance.hpNum += cureP;
                Destroy(this.gameObject);
            }
        }
    }
}