using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomOnHover : MonoBehaviour
{
    public float sizeFactor;
    private float canvasX, canvasY, minX, minY, maxX, maxY, x, y, distanceX, distanceY;
    private float width, height;
    // Start is called before the first frame update
    void Start()
    {
        canvasX = 1600;
        canvasY = 1000;
        RectTransform rt = this.GetComponent<RectTransform>();
        //need to get accurate width/height.
        width  = (rt.sizeDelta.x * rt.localScale.x)/2;
        height = (rt.sizeDelta.y * rt.localScale.y)/2;
        //width = (width > height) ? width : height;
        //height = (width > height) ? width : height;
        var lPos = transform.localPosition;
        maxX = lPos.x + width;
        minX = lPos.x - width;
        maxY = lPos.y + height;
        minY = lPos.y - height;
        x = lPos.x;
        y = lPos.y;
        distanceX = Mathf.Abs(lPos.x - minX);
        distanceY = Mathf.Abs(lPos.y - minY);
    }

    // god help me this took too long. My math really is weak.
    void Update()
    {
        updateEverything();
        if (inSquare()){
            var ratio = getPythoScale();
            //make sure the scale is always above 1, getscale should not 0.0x to 0.5x
            float scale = 1+sizeFactor*ratio+(ratio/10);
            // (scale - transform.localScale.x > 0.1f)
              //  scale = 1 + Time.deltaTime;
            this.transform.localScale = new Vector3(scale, scale, 1);

        }

        else{
                this.transform.localScale = new Vector3(1,1,1);
        }
        
    }

    private Vector2 getMousePos(){
        var relativePos = Camera.main.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        //portView conversion = cusrair at max left = 0%x, cursair at max right = 100%x. Playing around with percentages leaves us with this equation.
        //Since all canvases are set to have XY = 0 when pos is at CENTER.
        var posCenterisZero = new Vector2((canvasX/2)*-1 + (relativePos.x * canvasX), (canvasY/2)*-1 + (relativePos.y * canvasY));
        return posCenterisZero;
    }

    private bool inSquare(){
        var mousePos = getMousePos();
        //check if within rectangle
        return ((mousePos.x <= maxX) && (mousePos.x >= minX) && (mousePos.y <= maxY) && (mousePos.y >= minY));
    }

    private float getPythoScale(){
        var mousePos = getMousePos();
        //imagine a square. Imagine the pointer quite close to the center.
        //disx/y are the "small ratios (0~1)" to the X and Y axis'. They are reveresed (1-result), because the closer the pointer is to center,
        //the bigger the image should be, not vice-versa. derp.
        float disX = getDistance(distanceX, mousePos.x, x);
        float disY = getDistance(distanceY, mousePos.y, y);
        //god help me this took me too long to comprehend, I am such a moron jesus christ.
        float pythoPointerDistance = Mathf.Sqrt(disX*disX + disY*disY);
        float pythoTotalDistance = Mathf.Sqrt(distanceX*distanceX + distanceY*distanceY);
         return (1-(pythoPointerDistance / pythoTotalDistance));
  
    }

    private float getDistance(float distance, float pos, float XY){
        //convert centerX/Y - mousePositionX/Y to distance from pointer to center
        return Mathf.Abs(XY - pos);
        //1-(result), otherwise finding "the cloeset distance to center" will yield a big number (~>1), which will
        //make the image smaller as you get to the center (the closer you are to the center, the smaller the distance, right? And so...)
        //return (1-(posDistance/distance));
    }

    private void updateEverything(){
        var lPos = transform.localPosition;
        maxX = lPos.x + width;
        minX = lPos.x - width;
        maxY = lPos.y + height;
        minY = lPos.y - height;
        x = lPos.x;
        y = lPos.y;
        distanceX = Mathf.Abs(lPos.x - minX);
        distanceY = Mathf.Abs(lPos.y - minY);
    }
}
