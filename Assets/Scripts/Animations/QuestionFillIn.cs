using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class QuestionFillIn : MonoBehaviour
{
    private Text question;

    public void startAnimating(string questionString){

        question = this.GetComponent<Text>();

        Debug.Log("startAnimating: "+questionString);
        question.DOText(questionString, (float)questionString.Length/20);
    }
}

