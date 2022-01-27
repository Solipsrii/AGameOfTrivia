using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
public Text audioText, audioSIKES;
public AudioSource sfx_GameStart;
public Image img_GameStartOverlay; 

public Slider audioSlider;
public RectTransform audioParentTransform;
private bool optionsToggle;
public Animator menuAnimator;


/*
TODO:: add animations to GAME START. Fade out to black, primarily.
*/

     void Start(){
       if(!(PlayerPrefs.HasKey("MasterVolume")))
          PlayerPrefs.SetFloat("MasterVolume", 1f);
        audioSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        audioText.text = ""+((int)(audioSlider.value*100))+"%";
       optionsToggle = true;
     }

    //buttons//
    
    public void quitGame(){
      Application.Quit();
    }

    public void clickedOnStart(){
      StartCoroutine(_clickedOnStart());
    }

    private IEnumerator _clickedOnStart(){
      if(!optionsToggle){
        onOptionsClick();
        yield return new WaitForSeconds(1f);
      }
      
      menuAnimator.SetTrigger("menu_out");
    }

    public void startGame(){
      StartCoroutine(_startGame());
    }
    private IEnumerator _startGame(){
      sfx_GameStart.Play();
      img_GameStartOverlay.enabled = true;

      yield return new WaitForSeconds(0.7f);
      SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    }

    public void onAudioChange(){
        PlayerPrefs.SetFloat("MasterVolume", audioSlider.value);
        audioText.text = ""+((int)(audioSlider.value*100))+"%";
    }
 
    public void onOptionsClick(){
      audioSIKES.DOFade(0, 4f).SetEase(Ease.InCubic);
      if(optionsToggle)
        audioParentTransform.DOLocalMoveX(0, 1.3f).SetEase(Ease.OutCubic);
      else
        audioParentTransform.DOLocalMoveX(-500, 1f).SetEase(Ease.InCubic);

      optionsToggle = (optionsToggle) ? false : true;
    }

}
