using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TimerScript : MonoBehaviour
{
    private float timeRemaining; 
    public int sceneNumber; 
    public float timerDuration;
    private Rigidbody2D rigid;
    public Image imageToFill;
    public GameObject parent;
    private Vector3 originalParnetPos, exitPosition, startPose;
    private bool clockKilled, animateClockHand, fastClockNotPlayed, CLOCK_HASNT_INIT=true;

    public Image clockHandSeconds, clockHandMiliSeconds;

    public GameType currentManager;
    private AudioManager audioManager;

    // Start is called before the first frame update
        public void init(){
        originalParnetPos = parent.transform.localPosition;
        exitPosition = GameObject.Find("exit position").GetComponent<Transform>().localPosition;
        startPose = GameObject.Find("start position").GetComponent<Transform>().localPosition;
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
        //jank unfold! I couldn't be assed to find a way to stop the timer's "tick tock" via GameManager, so here it is! I don't care anymore!
        audioManager.A_Timer.Stop();
        audioManager.A_Timer.loop = true;
        audioManager.A_Timer.volume = audioManager.currentLevel;
        //
        timeRemaining = timerDuration; 
        rigid = GetComponent<Rigidbody2D>();
        clockKilled = false;
        fastClockNotPlayed = true;

        CLOCK_HASNT_INIT = false;
}
    void LateUpdate(){
      //TIMER LOGIC 
      if(!CLOCK_HASNT_INIT) //to prevent TIMER updating before Scene Helper initializes.
       if(!clockKilled){

          timeRemaining -= Time.deltaTime;
          if(animateClockHand)
            animateHandFrantically(); //animate the bigger clock-hand going 360
          if(timeRemaining <= 3.5f && fastClockNotPlayed){
            fastClockNotPlayed = false;
            audioManager.Play(audioManager.A_LoomingBoom, audioManager.clip_timer_LoomingBoom);
            audioManager.fade(audioManager.A_Timer, 0f, 2.5f);
            audioManager.A_Timer.loop = false;
          }

      if (timeRemaining <= 0){
        clockKilled = true;
        reportToManager();
        exitClock();
        }
      }
    }

    public void startTimer(){
      Debug.Log("Starting timer on scene: "+sceneNumber);
      switch(sceneNumber){
        case 2:
          scene2(); break;
        case 3:
          scene3(); break;
        case 4: 
          scene4(); break;
        default: Debug.Log("ERROR: startTimer did not call scene 1-4, fix sceneNumber value: "+sceneNumber); break;
      }
    }

    //timer killed abruptly, user answered everything.
    public void endTimer(){
      clockKilled = true;
      DOTween.Kill(1, false); //kill image fill animation
      DOTween.Kill(2, false); //kill timer hand mili
      DOTween.KillAll();
      //kill timer audio
      audioManager.fade(audioManager.A_Timer, 0f, 0.5f);
      audioManager.A_Timer.loop = false;
      //boot timer off screen.
      exitClock();
    }

    //basically only gets called if the timer ran out.
    //calls to either scene-helper (5050 or normal) to switch out scene and a false answer.
    private void reportToManager(){
          switch(currentManager)
          {
            case (GameType.fiftyfifty):
              //5050 reports the result itself
              GameObject.Find("TextAnimator").GetComponent<SceneHelperTwoAnswers>().nextScene(1);
              break;
            
            case (GameType.normal):
              GameObject.Find("Scene Helper").GetComponent<SceneHelper>().nextScene();
              GameObject.Find("Game Manager").GetComponent<GameManager>().reportResult(false);

              break;
          }
        }

    private void fillImage(){
       imageToFill.DOFillAmount(1f, timerDuration).SetEase(Ease.Linear).SetId(1);

    }
        public void fillImageReverse(float time){
       imageToFill.DOFillAmount(0f, time).SetEase(Ease.Linear).SetId(1);
    }

    private void animateHands(){
      clockHandMiliSeconds.transform.DORotate(new Vector3(0,0,0), timerDuration).SetEase(Ease.Linear).SetId(2);
    }

    private void animateHandFrantically(){
      clockHandSeconds.transform.Rotate(new Vector3(0,0, 75f * Time.deltaTime), Space.Self);
    }
    private void animateClock(){
      //make clock bounce left and righ
      this.transform.DOLocalMove(startPose, 1.5f).SetEase(Ease.OutCubic);
      animateClockHand = true;
      fillImageReverse(timerDuration);
      animateHands();
      audioManager.A_Timer.clip = audioManager.clip_timer_ticktockNormal;
      audioManager.A_Timer.Play();

    }

    public void exitClock(){
      StartCoroutine(_exitClock());
    }
    private IEnumerator _exitClock(){
      switch(currentManager){
        case (GameType.fiftyfifty):
          parent.transform.DOLocalMove(exitPosition, 1.2f).SetEase(Ease.OutCubic);
          break;

        case (GameType.normal):
          yield return new WaitForSeconds(1.5f);
          parent.transform.DOLocalMove(exitPosition, 1.5f).SetEase(Ease.OutCubic); //move back to original position
          parent.transform.DOBlendableRotateBy(new Vector3(0,0,1550f), 2f, RotateMode.LocalAxisAdd); //rotate shit tons 
          break;
      }
    }


    private void scene2(){
      animateClock();
      parent.transform.DOScale(new Vector3(1.7f,1.7f, 1), timerDuration+1.5f);
      return;
    }
    private void scene3(){
      animateClock();
    }
    private void scene4(){
      animateClock();
    }
}
