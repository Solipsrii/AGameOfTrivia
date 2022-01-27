using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GameEndedLogic : MonoBehaviour
{

    public Image bgslide, de_RedStrip;
    //0: that's the game, 1: took too long, //2: score is...
    public List<Text> textList;
    //0: *actual INT score*, 1: I wanna die :), 2+3: wanna die :) bigger
    public List<TextMeshProUGUI> textProList;
    private bool sceneNotEnded;

    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        sceneNotEnded = true;
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
        //put intro animation here
        bgslide.enabled = true;
        bgslide.transform.DOLocalMove(new Vector3(-1647, 1456, 0), 1.5f).SetEase(Ease.InCubic);
        StartCoroutine(startAnimation());

        //score text
        textProList[0].text = ""+GameObject.Find("Game Manager").GetComponent<GameManager>().correctAnswers + "/5";

    }

    // Update is called once per frame
    void Update(){
        if(sceneNotEnded)
            if (Input.GetKeyDown(KeyCode.Return) | Input.GetKeyDown(KeyCode.Escape) | Input.GetKeyDown(KeyCode.KeypadEnter)){
                sceneNotEnded = false;
                StartCoroutine(endAnimation());
        }
        
    }
    IEnumerator startAnimation(){
        //and this is why you should use the animator instead of this garbage, for large-scale animations.

        yield return new WaitForSeconds(0.6f);
        //redstrip
        de_RedStrip.transform.DOScaleY(0.6f, 1).SetEase(Ease.InCubic);
        //text in
        audioManager.Play(audioManager.A_SFX, audioManager.clip_endgame_Confetti);
        audioManager.Play(audioManager.A_BGM, audioManager.clip_endgame_Claps);
        yield return new WaitForSeconds(0.2f);
        textList[0].transform.DOLocalMove(new Vector3(-30, 272, 0), 0.7f).SetEase(Ease.OutCirc); //that's the game
        yield return new WaitForSeconds(1.2f);
        textList[1].transform.DOLocalMove(new Vector3(-75, 177, 0), 0.5f).SetEase(Ease.OutCirc); //took too long
        yield return new WaitForSeconds(1.0f);
        textProList[1].GetComponent<textproFloater>().startAnimation(0.75f, 25); //"I want to die :)" first one small
        yield return new WaitForSeconds(0.7f);
        textList[2].enabled = true;
        textList[2].DOFade(0f, 0.1f); //score is: ..
        textList[2].DOFade(255f, 0.5f);
        textList[2].transform.DOLocalMoveY(70, 0.5f).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(0.4f);
        textProList[0].GetComponent<textproFloater>().startAnimation(1f,30); //actual score
        yield return new WaitForSeconds(2.5f);
        textProList[2].GetComponent<textproFloater>().startAnimation(0.15f,50); //"I want to die :)" big
        textProList[3].GetComponent<textproFloater>().startAnimation(0.10f, 20); //"I want to die :)" big
        yield break;
    }

    IEnumerator endAnimation(){
        bgslide.enabled = true;
        bgslide.transform.DOLocalMove(new Vector3(-78, 21, 0), 1f).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(1.1f);
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
        yield  break;
    }
}
