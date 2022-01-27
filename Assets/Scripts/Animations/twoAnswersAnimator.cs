using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class twoAnswersAnimator : MonoBehaviour
{
    public Animator textAnimator;
    private List<animationType> animationQueue;
    private animationType currentAnim;
    private int i = 0;


    enum animationType
    {
        Oblique,
        LR
        
    }

    // Start is called before the first frame update
    public void init()
    {
        animationType[] tmp = new[] {
            animationType.Oblique,
            animationType.LR,
            animationType.Oblique,
            animationType.Oblique,
            animationType.LR
            };

        animationQueue = new List<animationType>(tmp);
        textAnimator.Play("In_Q");
    }

    public void introAnim(){
        currentAnim = getNextAnimation();
        textAnimator.Play("In_"+currentAnim);
        Debug.Log("In_"+currentAnim);
    }

    public void outroAnim(){
        textAnimator.SetTrigger("Out_"+currentAnim);
        textAnimator.SetTrigger("Switch_Q");
    }

    public void endAnim(){
        //fly out Q
        textAnimator.SetTrigger("Out_Q");
    }

    //played if the timer runs out
    public void interruptOut(){
        textAnimator.SetTrigger("Out_"+currentAnim);
        endAnim();
    }

    private animationType getNextAnimation(){
        var anim = animationQueue[0];
        animationQueue.RemoveAt(0);
        return anim;
    }

}
