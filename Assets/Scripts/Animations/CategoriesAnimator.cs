using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CategoriesAnimator : MonoBehaviour
{

    public Camera camera;
    public Light light;
    private GameManager gm;
    private categoriesHelper CH;
    private AudioManager audioManager;
    
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
        CH = this.GetComponent<categoriesHelper>();

        animateCamera();
        animateLight();
    }

    private void animateCamera(){
        camera.transform.DOMoveZ(-377, 1f).SetEase(Ease.InOutCubic);
        camera.transform.DOLocalRotate(new Vector3(90,0,0), 1.5f).SetEase(Ease.InOutCubic);
    }

    private void animateLight(){
        Sequence MoveSequence = DOTween.Sequence();
        Sequence brightnessSequence = DOTween.Sequence();
        MoveSequence.Append(
            light.transform.DOLocalMoveX(200, 5f));
        MoveSequence.Append(
            light.transform.DOLocalMoveX(-200, 5f));

        brightnessSequence.Append(
            light.DOIntensity(9, 3f).SetEase(Ease.InOutCubic));
        brightnessSequence.Append(
            light.DOIntensity(11, 3f).SetEase(Ease.InOutCubic));


        brightnessSequence.SetLoops(-1);
        MoveSequence.SetLoops(-1);
        MoveSequence.Play();
        brightnessSequence.Play();
    }

    public void animateCardsToCenterAndOut(Transform chosenCard, Transform discardedCard){
        StartCoroutine(animateCardsToCenterAndOut_(chosenCard, discardedCard));
    }
    private IEnumerator animateCardsToCenterAndOut_(Transform chosenCard, Transform discardedCard){
        chosenCard.DOLocalMoveX(9, 0.5f).SetEase(Ease.OutCubic);
        chosenCard.DOLocalMoveY(-10, 0.7f).SetEase(Ease.InOutCubic);
        TweenCallback c = new TweenCallback(animateLight);
        //if the card which wasn't chosen was either on the RIGHT (+), or on the left.
        if(discardedCard.position.x > 0)
            discardedCard.DOMoveX(100, 1f).SetEase(Ease.InCubic);
        else
            discardedCard.DOMoveX(-90, 1f).SetEase(Ease.InCubic);
    
        //wait for animations to stop
        yield return new WaitForSeconds(1f);
        //animate outro
        chosenCard.DOLocalMoveZ(-300f, 0.8f).SetEase(Ease.InCubic);
        //wait and end scene.
        yield return new WaitForSeconds(0.8f);
        CH.endScene();
    }
    
}
