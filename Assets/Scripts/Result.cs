using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Result : MonoBehaviour
{
    [Header("フェード")] public FadeImage fade;
    [Header("タイトルに戻す時に流すSE")] public AudioClip startSE;


    private bool firstPush = false;
    private bool goNextScene = false;

    //スタートボタンを押されたら呼ばれる
    public void GoTitle()
    {
        Debug.Log("Go Title!");
        if (!firstPush)
        {
            GManager.instance.PlaySE(startSE);
            Debug.Log("Go Next Scene!");
            fade.StartFadeOut();
            firstPush = true;
        }
    }

    private void Update()
    {
        if (!goNextScene && fade.IsFadeOutComplete())
        {
            SceneManager.LoadScene("Title");
            goNextScene = true;
        }
    }
}