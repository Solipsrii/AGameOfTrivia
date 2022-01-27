using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class categoriesHelper_hands : MonoBehaviour
{
    private HandsAnimator HA;
    private GameManager GM;
    public Image left, right;

    public List<Sprite> categoryBank;
    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        HA = GetComponent<HandsAnimator>();

        new WaitForSeconds(0.5f);
        HA.startIntro();
        setImages();
        
    }

    // Update is called once per frame


    //create onClick event for button press on either card, and animate accordingly (send object.transform to 'HA')
    public void onClick(bool Right){
        int index = (Right) ? 1 : 0;
        GM.currentCategory = GM.choose2Categories[index];

        HA.endAnimation_jitter();
    }

    public void sceneEnded(){
        GM.NextCategoryNotChosen = false;
    }

    //helper methods//

    private void setImages(){
        left.overrideSprite = categoryBank[(int)GM.choose2Categories[0]];
        right.overrideSprite= categoryBank[(int)GM.choose2Categories[1]];
    }
}


