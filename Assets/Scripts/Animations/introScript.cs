using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class introScript : MonoBehaviour
{
    public GameManager GM;
    public GameObject textParent;
    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }
    public void startGame(){
        GM.startGame();
    }
    public void turnOffText(){
        textParent.SetActive(false);
    }
}
