using UnityEngine;

public class animatorToggle : MonoBehaviour
{
    public GameObject textAnimatorCanvas;
    public GameObject parent, timerCanvas;

    void Start(){
        //set BG to be blurry!
        GameObject.Find("Game Manager").GetComponent<GameManager>().animateBGIn(MainAnimationHelper.IntroType.BlurBackground);
    }
    public void onIntroEnd(){
        timerCanvas.SetActive(true);
        textAnimatorCanvas.SetActive(true);        
        Destroy(parent);
    }
}
