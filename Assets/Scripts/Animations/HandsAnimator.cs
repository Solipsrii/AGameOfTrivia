using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HandsAnimator : MonoBehaviour
{

    public GameObject leftParent, rightParent, rightShine, leftShine, catShine;

    public bool CategoryNotChosen, ShineL_NotTriggered, ShineR_NotTriggered, endAnimationTriggered;
    public Image leftCat, rightCat, finalCat;
    private categoriesHelper_hands CH;
    // Start is called before the first frame update
    void Start()
    {
        CH = GetComponent<categoriesHelper_hands>();

        CategoryNotChosen = true;
        ShineL_NotTriggered = true;
        ShineR_NotTriggered = true;
        endAnimationTriggered = false;
    }

    public void startIntro(){
        var delay = 1f;
        var delayRotate = delay+0.3f;
        leftParent.transform.DOLocalMove(new Vector3(-203, -120f, 0), delay).SetEase(Ease.OutCubic);
        leftParent.transform.DOLocalRotate(new Vector3(0, -1, -3), delayRotate).SetEase(Ease.OutCubic);

        rightParent.transform.DOLocalMove(new Vector3(140, -110, 0), delay).SetEase(Ease.OutCubic);
        rightParent.transform.DOLocalRotate(new Vector3(-0.16f, -1.13f, -13), delayRotate).SetEase(Ease.OutCubic);

        StartCoroutine(jitterHands(2f));
    }

    private IEnumerator jitterHands(float waitOnStart){
        System.Random randGen = new System.Random();
        yield return new WaitForSeconds(waitOnStart);
        ////
        var LeftParent_Pos = leftParent.transform.localPosition;
        var rightParent_Pos = rightParent.transform.localPosition;

        Vector3[] posBank = new Vector3[] {
            new Vector3(5, -2, 0),
            new Vector3(-5, 4, 0), 
            new Vector3(3, -7, 0),
            new Vector3(2, 2, 0)
        };

        Vector3[] rotateBank = new Vector3[] {
            new Vector3(0, 0, 4.5f),
            new Vector3(0, 0, -2.5f)
        };

        //animate
        int i = 0;
        var delayForHandsJitter = 2.5f;
        
        while(CategoryNotChosen){
            //make the hands "move a little" in random directions, with draft position and not working position.
            leftParent.transform.DOLocalMove(  LeftParent_Pos + posBank[randGen.Next(0, posBank.Length-1)], delayForHandsJitter).SetId(999);
            rightParent.transform.DOLocalMove(rightParent_Pos + posBank[randGen.Next(0, posBank.Length-1)], delayForHandsJitter).SetId(999);
            //A bit different here, rotate towards each other, and then off each other. Flip flop from 0 to 1 of RotateBank..
            leftParent.transform.DOLocalRotate(rotateBank[i%2], delayForHandsJitter+0.2f).SetId(999).SetEase(Ease.InOutCubic);
            rightParent.transform.DOLocalRotate(rotateBank[(i+1)%2], delayForHandsJitter+0.2f).SetId(999).SetEase(Ease.InOutCubic);
            i++;
            yield return new WaitForSeconds(delayForHandsJitter);
        }

    }

    public void onHoverCategory(bool Right){
        float delay = 1.1f;
        if (Right)
            StartCoroutine(_onHoverRight(delay));
        else
            StartCoroutine(_onHoverLeft(delay));
}



    private IEnumerator _onHoverRight(float delay){
        if(ShineR_NotTriggered){
            ShineR_NotTriggered = false;
        //using sequence cuz: return "shine" graphic to its origianl location as soon as the transition is over.
            Sequence seq = DOTween.Sequence();
            seq.Append(
                rightShine.transform.DOLocalMove(rightShine.transform.localPosition + new Vector3(-125, 144, 0), delay).SetEase(Ease.InOutQuad));
            seq.Append(
                rightShine.transform.DOLocalMove(rightShine.transform.localPosition, 0f)
            );
            seq.Play();
        
            yield return new WaitForSeconds(delay+0.1f);
            ShineR_NotTriggered = true;
        }
    }
    private IEnumerator _onHoverLeft(float delay){
        if(ShineL_NotTriggered){
            ShineL_NotTriggered = false;
        //using sequence cuz: return "shine" graphic to its origianl location as soon as the transition is over.
            Sequence seq = DOTween.Sequence();
            seq.Append(
                leftShine.transform.DOLocalMove(leftShine.transform.localPosition + new Vector3(137, 112, 0), delay).SetEase(Ease.InOutQuad));
            seq.Append(
                leftShine.transform.DOLocalMove(leftShine.transform.localPosition, 0f)
            );
            seq.Play();

            yield return new WaitForSeconds(delay+0.1f);
            ShineL_NotTriggered = true;
        }
    }



    public void endAnimation_jitter(){
        CategoryNotChosen = false;
        DOTween.Kill(999);

        if(!endAnimationTriggered){
            endAnimationTriggered = true;
            StartCoroutine(_endAnimation_jitter());
        }
    }

    private IEnumerator _endAnimation_jitter(){
        var randGen = new System.Random();
        int num = 15;

        leftParent.transform.DOLocalMoveY(-900, 6f);
        rightParent.transform.DOLocalMoveY(-900, 6f);
        float time = Time.time;
        while((Time.time - time) < 3f){
            leftParent.transform.localPosition += new Vector3(randGen.Next(num*-1,num), randGen.Next(num*-1,num), 0);
            rightParent.transform.localPosition += new Vector3(randGen.Next(num*-1,num), randGen.Next(num*-1,num), 0);
            yield return new WaitForEndOfFrame();
        }

        CH.sceneEnded();
    }
}
