using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
TODO:Disable button after click, and only enable it after animation ends.
*/


public class SceneHelper5050 : MonoBehaviour
{
    public Button btn0_0, btn0_1, btn1_0, btn1_1;
    public Text txtFrontL, txtFrontR, txtBackL, txtBackR, questionFront, questionBack;
    public Transform cube0, cube1, cubeQ;
    private readonly Object key = new Object();
    private bool clickOnceFlag = false;
    private int correctAnswers;
    private GameManager GM;
    private QnA currentSet;
    private TimerScript timer;
    private AnimationHelper5050 AH; 
    private bool flipped;
    // Start is called before the first frame update
    void Start()
    {
        AH =    GameObject.Find("Animation Helper").GetComponent<AnimationHelper5050>();
        timer = GameObject.Find("Animation Helper").GetComponent<TimerScript>(); 
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        currentSet = GM.currentSet;
        correctAnswers = 0;

        AH.init();
        //timer requires enabling since it relies on update() to function.
        timer.enabled = true;

        flipped = false;
        //initial text init
        txtFrontL.text = currentSet.answers[0];
        txtFrontR.text = currentSet.answers[1];
        questionFront.text = currentSet.question;
        //end init

        //start anim init
    }

/*
Initializes the next scene, sets the next question and 2 answers to the flip-flopped tiles.
Gauranteed to not have an empty next-set w/"checkAnswerOnClick".
*/
    private void nextSet(){
        flipped = (flipped) ? false : true; 
        clickOnceFlag = false;

        currentSet = GM.database5050.getNextSet();
        //false = not ended
        setFlip();
    }

    private void setFlip(){
            switch(flipped){  
            //i.e: either _0 or _0 were pressed, so init _1 & _1.
            case false: 
                setText(txtFrontL, txtFrontR, questionFront);break;
            //i.e: either _1 or _1 were pressed, so init _0 & _0.
            case true:
                setText(txtBackL, txtBackR, questionBack);break;
            }
    }

    private void setText(Text A, Text B, Text Q){
            A.text = currentSet.answers[0];
            B.text = currentSet.answers[1];
            Q.text = currentSet.question;
    }

/*
    In each scene, the BUTTONS' on-click event call this function. 0~1 are possible here, where 0/1 is _0 or _1.
    In this variation, has to check if there're no more sets.
*/
    public void checkAnswerOnClick(int num){
        lock(key){
            //flag resets on each NEXTSET()
            if(!clickOnceFlag){
                clickOnceFlag = true;
                Debug.Log("I was clicked! at: "+num);
                string answer;
                switch(num){
                    case 0:
                    answer = currentSet.answers[0];break;
                    case 1:
                    answer = currentSet.answers[1];break;
                    default: 
                    answer = null;break;
                    }
                
                if (string.IsNullOrEmpty(answer)){
                    Debug.Log("ERROR: 'answer' is null! I.E: Wrong button input! (not 0~3!)");
                    return;
                }

                if (currentSet.correctAnswer.Equals(answer)){
                    correctAnswers++;
                    //animate the dots to correspond to an either correct or false answer.
                    AH.updateScoreUI(true);
                }
                else
                    AH.updateScoreUI(false);
                var clickedOn = (num == 0) ? cube0 : cube1;
                AH.rotateAnswer(clickedOn);
                AH.rotateQuestion();

                //check for end of stage.
                if (GM.database5050.ReachedEndOfList())
                    nextScene();
                else
                    nextSet();
            }
            //no code here//
        }
        //here neither//
    }

    public void nextScene(){
        StartCoroutine(nextSceneYield());
    }

private IEnumerator nextSceneYield(){
        Debug.Log("5050 ENDED! Scene wrapping up.");
         timer.endTimer();
         GM.reportResult(correctAnswers);
         //disable buttons
         //
         btn0_0.enabled = false;
         btn0_1.enabled = false;
         btn1_0.enabled = false;
         btn1_1.enabled = false;
         //
         yield return (AH.outroAnim());
         //next scene
        GM.animateOutro(MainAnimationHelper.OutroType.ImmediateBlackout);
        
}

}
