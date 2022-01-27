using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//TODO:: Add "wrong" / "correct" indicators to flip. Like a red / green glow or a sound. Also add camera shake already.

public class AnimationHelper5050 : MonoBehaviour
{
    int answersIndex;
    float answersCounterOriginalPOS;
    //

    private Vector3 vector180;
    //
    public List<Transform> transformList;
    public List<SpriteRenderer> answersCounterList;
    private TimerScript timer;
    //callback to scenehelper so this class can report back once animation ends.


    public void init(){
        //init stuff here, not yet sure what really.
        timer = GetComponent<TimerScript>();
        vector180 = new Vector3(0, 180, 0);

        answersIndex = 0;
        answersCounterOriginalPOS = answersCounterList[0].transform.localPosition.y;
        //
        //set circles out of screen, bottom.
        for(int i = 0; i < answersCounterList.Count; i++){
            answersCounterList[i].transform.localPosition = 
            new Vector3(
                    answersCounterList[i].transform.localPosition.x, 
                    answersCounterList[i].transform.localPosition.y - 50, 
                    0);     
        }

        animateMainIntro();
    }
    //helper methods here//
    public void rotateAnswer(Transform trans){
        //I want:
        //A. It first glows either red or green, or whatever, and
        //B. It first rotates and then the other piece rotates.
        Transform clickedOnTransform, otherTransform;
        clickedOnTransform = (trans == transformList[0]) ? transformList[0] : transformList[1];
        otherTransform =     (trans != transformList[0]) ? transformList[0] : transformList[1];
        StartCoroutine(rotateAnswer(clickedOnTransform, otherTransform));
    }

    private IEnumerator rotateAnswer(Transform clickedOnTransform, Transform otherTransform){
        var delayRotation = 0.2f;
        clickedOnTransform.DOLocalRotate(vector180, delayRotation).SetRelative();
        yield return new WaitForSeconds(0.15f);
        otherTransform.DOLocalRotate(vector180, delayRotation).SetRelative();
    }

    public void rotateQuestion(){
        //[2] = question block to rotate.
        transformList[2].DOLocalRotate(new Vector3(180, 0, 0) ,0.2f).SetRelative();
    }

    public int outroAnim(){
        //TODO: create actual outro
        //outro timer by backing out the fill
        timer.fillImageReverse(0.3f);
        return 2;

    }
    private IEnumerator outroAnimNumerator(){
        yield return null;
    }

        //put any sequences here for scene intro
        public void animateMainIntro(){
        animateIntroAnswersCounter();
    }

    public void animateIntroAnswersCounter(){
        StartCoroutine(animateIntroAnswersCounterx());
    }
    private IEnumerator animateIntroAnswersCounterx(){
        foreach(SpriteRenderer SR in answersCounterList){
            var trans = SR.transform;
            trans.DOLocalMoveY(answersCounterOriginalPOS, 0.6f).SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(0.2f);
        }
    }

        public void updateScoreUI(bool guessedCorrectly){
        //sequence has to be declared locally. Why? I DON'T FUCKING KNOW.
        Sequence seq_ScoreUIJitter = DOTween.Sequence();
        var answerIndicator = answersCounterList[answersIndex];
        //
        seq_ScoreUIJitter
        .Append(answerIndicator.transform.DOLocalMoveY(answerIndicator.transform.localPosition.y + 15, 0.15f).SetEase(Ease.InCubic));
        seq_ScoreUIJitter
        .Append(answerIndicator.transform.DOLocalMoveY(answerIndicator.transform.localPosition.y, 0.45f).SetEase(Ease.OutCubic));
        //

     answerIndicator.color = (guessedCorrectly) ? Color.green : Color.red;
     seq_ScoreUIJitter.Play();
     answersIndex++;
     //

    }
}   
