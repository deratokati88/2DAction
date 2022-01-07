using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBer : MonoBehaviour
{
    public Player player;

    public float playerRemainingHp = 0.0f;
    public int playerOldHp;
    private Image img = null;
    

    // Start is called before the first frame update
    void Start()
    {
        playerOldHp = GManager.instance.hpNum;
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        calcRemainigHp();
    }

    void calcRemainigHp()
    {
        if (playerOldHp != GManager.instance.hpNum)
        {
            playerRemainingHp = 100 * GManager.instance.hpNum / GManager.instance.maxHpNum;
            playerRemainingHp /= 100;
            img.fillAmount = playerRemainingHp;
            playerOldHp = GManager.instance.hpNum;
        }
    }
}
