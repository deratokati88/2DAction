using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultText : MonoBehaviour
{
    private Text resultText = null;

    // Start is called before the first frame update
    void Start()
    {
        resultText = GetComponent<Text>();
        if (GManager.instance != null)
        {
            resultText.text ="Score " + GManager.instance.score
            + "\r\nContinue " + GManager.instance.retryNum;
        }
        else
        {
            Debug.Log("ゲームマネージャー置き忘れてるよ！");
            Destroy(this);
        }
    }

}