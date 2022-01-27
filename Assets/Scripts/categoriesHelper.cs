using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class categoriesHelper : MonoBehaviour
{
    GameManager gm;
    public Image left, right;
    public List<Sprite> categoryImageBank;
    CategoriesAnimator ca;
    
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        ca = this.GetComponent<CategoriesAnimator>();
        setImages();
    }


    private void setImages(){
        left.overrideSprite = categoryImageBank[(int)gm.choose2Categories[1]];
        right.overrideSprite= categoryImageBank[(int)gm.choose2Categories[0]];
    }
    
    public void onClick(bool index){
        //left category = true
        //right category  = false
        
        var intIndex = (index) ? 1 : 0;
        gm.currentCategory = gm.choose2Categories[intIndex];
        
        var categoryClickedOn = (index) ? left : right;
        var categoryDiscarded = (index) ? right: left;
        ca.animateCardsToCenterAndOut(categoryClickedOn.transform, categoryDiscarded.transform);
        
    }

    public void endScene(){
        gm.NextCategoryNotChosen = false;
        
    }
}
