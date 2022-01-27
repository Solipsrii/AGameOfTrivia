using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;



public class AnimationHelper : MonoBehaviour
{
    /*
        Gets auto-enabled by SceneHelper by the end of its own Start().

        Responsible for:
        1. activating text objects' animations in a delayed sequences.
    */
    public List<Button> buttonList;
    //preferably use this one over button list, which makes actual no sense.
    public List<Transform> transformList;
    public Text question;
    public int yOffset;
    public List<Font> fontList;
    private SceneHelper SH;
    private int fontIndex;
    private List<Vector3> originalPosList;

    //run by SceneHelper
    public void init(int sceneNum){
        //initialize stuff.

        fontIndex = 0;
        //shuffle fontList randomally using Linq, like streams in java.
        fontList = fontList.OrderBy(x => Random.value).ToList<Font>();
        SH = GameObject.Find("Scene Helper").GetComponent<SceneHelper>();
        
        //initialize the rigidbody list
        originalPosList = new List<Vector3>();
        foreach(Button btn in buttonList){
            var pos = btn.GetComponent<Transform>().localPosition;
            originalPosList.Add(pos);
        }
    
        switch(sceneNum){       
            case 3:
                StartCoroutine(sceneThreePresetIntro()); break;
            case 4:
                StartCoroutine(sceneFourPresetIntro()); break;
        }

        //fill in question text
        QuestionFillIn QFI = GameObject.Find("Question").GetComponent<QuestionFillIn>();
        QFI.startAnimating(SH.currentSet.question);
    }

      private IEnumerator sceneFourPresetIntro(){
        //animate text fade-in and drop.
        var yOffsetValue = 0;
        Debug.Log("Animated!");
        foreach(Button button in buttonList){
            yield return new WaitForSeconds(0.15f);
            
            //move each button according to path
            //find txt first
            var textComponent = button.GetComponentInChildren<Text>();
            //fade in
            textComponent.DOFade(255f, 2f).SetEase(Ease.InCubic);
                //animation demo = the fade-in and pathing movement animation.
               // button.GetComponent<animationDemo>().enabled = true;
                //offsetting each text.
                //var rigidComponent = 
                var btnRigid = button.GetComponent<Rigidbody2D>();
                btnRigid.position += new Vector2(0, yOffsetValue);
                yOffsetValue += yOffset;
                button.GetComponentInChildren<Text>().font = fontList[fontIndex++];
        
            Vector2 WP0 = new Vector2(btnRigid.position.x -15f, btnRigid.position.y-20f);
            Vector2 WP1 = new Vector2(WP0.x +30f, WP0.y -15f);
            Vector2[] PathWaypoints = new[] {WP0, WP1};
        
            btnRigid.DOPath(PathWaypoints, 1.5f, PathType.CatmullRom, PathMode.Ignore).SetEase(Ease.OutCubic);

        }
    }



    private IEnumerator sceneThreePresetIntro(){
        //animate the 3 cardboards entering. Left0 bottom1 right2.
        //draw buttons out of view.
        buttonList[0].transform.localPosition = new Vector3(-1285, -6, 0);
        buttonList[1].transform.localPosition = new Vector3(-35, -850, 0);
        buttonList[2].transform.localPosition = new Vector3(1269, -12, 0);

        //tween to the original pos.
        for(int i = 0; i < 3; i++){
            Debug.Log("Attempted to animate "+i);
            var transform = buttonList[i].GetComponent<Transform>();
            transform.DOLocalMove(originalPosList[i], Random.Range(1f, 1.7f), true);
            //
        }
        yield return null;
    }

    public IEnumerator outroAnim(int sceneNum){
        yield return new WaitForSeconds(1);
        switch(sceneNum){
         case 3:
            StartCoroutine(dropTextAndCenterCorrectAnswer()); break;
         case 4:
            StartCoroutine(dropTextAndCenterCorrectAnswer()); break;
         default: Debug.Log("OutrAnim: received invalid scene count. what.");break;
         }
    }

    private IEnumerator dropTextAndCenterCorrectAnswer(){
          Button correctAnswer = null;

        //find correct answer object
        foreach(Button button in buttonList){
            if (button.GetComponentInChildren<Text>().text.Equals(SH.currentSet.correctAnswer)){
                correctAnswer = button;
                continue; //'correct answer' skips the rest of the foreach loop.
            }
            //make each object's rigidbody switch to Dynamic, which would make it adhear to gravity, and fall.
            yield return new WaitForSeconds(0.3f);
            var rigiddrop = button.GetComponent<Rigidbody2D>();
            rigiddrop.bodyType = RigidbodyType2D.Dynamic;
        }
            Sequence seq = DOTween.Sequence();
            seq.Append(
            correctAnswer.transform.DOLocalMove(new Vector3(0,0,0), 0.5f).SetEase(Ease.InCubic));
            seq.Append(correctAnswer.transform.DOScale(new Vector3(1.5f,1.5f,0), 0.3f)); //flies correct answer to center
            seq.Play();
            yield return new WaitForSeconds(1.5f);
            question.transform.DOLocalMoveY(question.transform.position.y + 400, 0.6f).SetEase(Ease.InCirc);
            correctAnswer.transform.DOMoveY(-1000, 3f).SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(1.4f);
            SH.nextScene();
    }

}
