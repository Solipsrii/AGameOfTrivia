using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainAnimationHelper : MonoBehaviour
{
    private Transform background, img_blackoverlayT;
    private Image img_blackoverlay;
    public Image img_blurBG;
    public Animator answersAnimator;
    private AudioManager audioManager;
    private GameManager gm;
    public enum OutroType
{       
    ImmediateBlackout,
    ZoomIn,
    BlurBackground
}

    public enum IntroType
{       
    BlurBackground
}


    /*
    responsible to animate anything that's persistent on the screen throughout scenes,
    like the answers-counter.
    */

    
    // Start is called before the first frame update
    public void init(GameManager gm)
    {
        background = GameObject.Find("BG").GetComponent<Transform>();
        img_blackoverlay = GameObject.Find("BlackOverlay").GetComponent<Image>();
        img_blackoverlayT= img_blackoverlay.transform;
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
        this.gm = gm;
        
        //
        //create a sequence for the circles-score UI, to jitter up and down and change to their respective colors.
    }

    public void animateMainIntro(){
        
    }

    public IEnumerator animateOutro(OutroType type){

    switch(type){
        case OutroType.ZoomIn:
        //yield return StartCoroutine(zoomin());
        break;

        case OutroType.ImmediateBlackout:
            //TODO: Add immediate blackout animation. Will be used by 50/50 with a sound! Next thing to add! update: Surely! Totally!
            break;

        case OutroType.BlurBackground:
            blurBG_out(1f);
            yield return new WaitForSeconds(0.7f);
            break;
        }
    }

    public IEnumerator animateIntro(IntroType type){
        switch(type){
            case (IntroType.BlurBackground):
                blurBG_in(1.6f);
                yield return new WaitForSeconds(2.2f);
                break;
        }
    }

    private IEnumerator textRainbow(Text t){
        float DURR = 5f;
        Color[] c = {Color.red, Color.green, Color.blue};
        int i = 0;

        while(DURR > 0){
            t.DOBlendableColor(c[(i++)%3], 0.2f);
            yield return new WaitForSeconds(0.2f);
            DURR = DURR - Time.deltaTime*6;
        }
        yield break;
    }

    public void updateScoreUI(bool guessedCorrectly){
        //create new method to represent current score
    }

    //        ****          //
    //     animation sets   //
    //        ****          //

    private IEnumerator zoomin(){
    var backgroundScale = new Vector3(1.0f,1.0f,1.0f);
    Sequence seq = DOTween.Sequence();
    Sequence fadeSeq = DOTween.Sequence();

    seq.Append(
        background.DOScale(background.localScale - new Vector3(0.08f, 0.08f, 0), 0.7f).SetEase(Ease.InCubic));
    seq.Append(
       background.DOScale(background.localScale + new Vector3(3f,3f,0), 2.3f).SetEase(Ease.OutCubic));

    fadeSeq.Append(
        img_blackoverlayT.DOLocalRotate(new Vector3(0,0,0), 1.5f));
    fadeSeq.Append(
        img_blackoverlay.DOFade(10, 15f)).SetId(10);   

    audioManager.Play(audioManager.A_SFX, audioManager.clip_zoom);
    seq.Play();
    fadeSeq.Play();
    yield return new WaitForSeconds(2.5f);
    DOTween.Kill(10, false);
    background.DOScale(backgroundScale, 0f);
    }

    private void blurBG_in(float delay){
        img_blurBG.DOFade(1, delay).SetEase(Ease.OutCubic);
    }

    private void blurBG_out(float delay){
        img_blurBG.DOFade(0, delay).SetEase(Ease.InCubic);
    }

    public void animate_correctAnswer(){
        var rand = new System.Random();
        answersAnimator.Play("answers_correct"+rand.Next(1,2));
        
    }

    public void animate_wrongAnswer(){
        var rand = new System.Random();
        answersAnimator.Play("answers_wrong"+rand.Next(1,2));
    }
}

