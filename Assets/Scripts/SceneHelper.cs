using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneHelper : MonoBehaviour
{
    /*
    The scene-helper script assits the game-manager by providing it with the type of scene that needs to be rendered.
    */

    private GameManager GM;
    private AnimationHelper AH;
    public QnA currentSet;
    public Text text0, text1, text2, text3, question;
    private bool clickOnceFlag;
    private readonly object key = new object();
    private TimerScript timer;
    public List<Rigidbody2D> originalTextPositions;

    //define in unity!
    void Start()
    {
        AH = GameObject.Find("Animation Helper").GetComponent<AnimationHelper>();
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        timer = GameObject.Find("base").GetComponent<TimerScript>();

        originalTextPositions = new List<Rigidbody2D>();
        currentSet = GM.currentSet;
        initiateScene(currentSet.getAnswerCount());
        
        //initiate scene-intro here. If any sequence changes are desired, it's a good idea to start here.
        timer.enabled = true;
        AH.init(currentSet.getAnswerCount());
        timer.init();
        timer.startTimer();
    }


    private void initiateScene(int numOfAnswers){
        //switch TEXT string to the available answers, and add the UI's original screen pos to the list, sent to animateion-helper.
        clickOnceFlag = false;
        switch(numOfAnswers){
            case 4:
                text3.text = currentSet.answers[3];
                goto case 3;
            case 3:
                text2.text = currentSet.answers[2]; 
                break;
        }

        //always init txt0 & 1, because scene "two answers" has its own separate sceneHelper code. I.E: 3 & 4 will always need these +2 txt answers.
        text0.text = currentSet.answers[0];
        text1.text = currentSet.answers[1];

        //setting text to nothing. Animation Manager will call to a function which will fill-in the question-text.
        question.text = "";
    }



    public void checkAnswerOnClick(int num){
        lock(key){
            if(!clickOnceFlag){
                clickOnceFlag = true;
                timer.endTimer();
                Debug.Log("I was clicked! at: "+num);
                string answer;
                switch(num){
                    case 0:
                    answer = text0.text;       break;
                    case 1:
                    answer = text1.text;       break;
                    case 2:
                    answer = text2.text;       break;
                    case 3:
                    answer = text3.text;       break;
                    default: answer = null;    break;
                    }
                if (string.IsNullOrEmpty(answer)){
                    Debug.Log("ERROR: 'answer' is null! I.E: Wrong button input! (not 0~3!)");
                    return;
                    }

                //callback to GM, logic only.
                GM.reportResult(currentSet.correctAnswer.Equals(answer));
                StartCoroutine(AH.outroAnim(currentSet.answers.Count));
            }
        }
        //do not write code here//
    }

    public void nextScene(){
        GM.animateOutro(MainAnimationHelper.OutroType.ZoomIn);
    }

    //An aid-function to certain animation sequences. Used to maintain original object XY coordinanates, before shifting them out of screen in their respective methods
    //in ANIMATION HELPER.
}
