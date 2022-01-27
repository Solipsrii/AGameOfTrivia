using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneHelperTwoAnswers : MonoBehaviour
{
    private GameManager GM;
    private QnA currentSet;
    private newBank database;
    private twoAnswersAnimator AM;
    public Text answer1, answer2, question;
    private TimerScript timerComponent;
    private readonly Object key = new Object();
    private readonly Object keyUpdate = new Object();
    private int numOfCorrectAnswers, numOfClicks;
    public bool canTriggerIntro;


    // Start is called before the first frame update
    void Start()
    {
        numOfClicks = 0;
        canTriggerIntro = true;
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        timerComponent = GameObject.Find("base").GetComponent<TimerScript>();

        database = GM.database5050;
        currentSet = GM.database5050.getNextSet();
        numOfCorrectAnswers = 0;

        setText();
        AM = this.GetComponent<twoAnswersAnimator>();
        AM.init();
        AM.introAnim();
        timerComponent.init();
        timerComponent.startTimer();

    }

    public void onClick(bool right){
        //1: rightmost answer (answer2), 0: leftmost answer. (answer1)
        //checks which question was pressed, advances the question-counter, and also paints the txt and plays a sound.
        //basically, all gameplay logic happens here.

        lock(key){
            numOfClicks++;
            bool correct = false;

            if (right){
                if(string.Equals(answer2.text, currentSet.correctAnswer))
                    correct = true;
            }
            //TODO: for some reason, this comparison causes a crash. 
            if(!right){ //clicked on answer 1
                if(string.Equals(answer1.text, currentSet.correctAnswer))
                    correct = true;
            }
            if (correct)
                numOfCorrectAnswers++;

            //paint answer here
            paintText(right, correct);
            //fly out the 2 answers
            AM.outroAnim();

            if (numOfClicks == 5){
                AM.endAnim();
                canTriggerIntro = false;
            }

        }
        //end of lock
    }

    //call from last frame of each *_Out animation's keyframe (as an event)
    public void playNextSet(){
        if (canTriggerIntro){
            getNextSet();
            setText();
            AM.introAnim();
            //restart colors
            answer1.color = Color.white;
            answer2.color = Color.white;
        }
    }

    public void nextScene(int timerInterrupt){
        StartCoroutine(_nextScene(timerInterrupt));
    }

    private IEnumerator _nextScene(int timerInterrupt){
        canTriggerIntro = false;
        if (timerInterrupt == 1){
            AM.interruptOut();
            yield return new WaitForSeconds(1.5f);
        }
        //reportResults() is triggered via the Q_Out animation.
        GM.animateOutro(MainAnimationHelper.OutroType.BlurBackground);
    }

    public void reportResults(){
        //this will auto-trigger the right / wrong animation
        GM.reportResult(numOfCorrectAnswers);
    }

    //helper funcs//

    
    //activated on each button click

    public void event_exitTimer(){
        timerComponent.endTimer();
    }
    private void getNextSet(){
        currentSet = database.getNextSet();
    }

    private void setText(){
        answer1.text = currentSet.answers[0];
        answer2.text = currentSet.answers[1];
        question.text = currentSet.question;
    }

    private void paintText(bool right, bool correct){
        if (right){
            if(correct)
                answer2.color = Color.green;
            else
                answer2.color = Color.red;
        } 

        else { //leftmost answer
            if(correct)
                answer1.color = Color.green;   
            else
                answer1.color = Color.red;
        }
    } 


}
