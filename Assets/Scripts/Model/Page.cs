using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Page 
{
    //"Page" is a simple object class pairing of a list and a category. It contains all questions and answers of a given Category (the "Page").
    public List<QnA> QnAList;
    Categories cPair;
    System.Random rand;

    public Page(Categories cPair){
        this.cPair = cPair;
        QnAList = new List<QnA>();
        rand = new System.Random();
    }

    //
    public void addItem(QnA item){
        QnAList.Add(item);
    }

    public QnA getSet(){
        if (QnAList.Count-1 < 0){
            Debug.Log("ERROR!!!!!!! getRandomSet tried to retrieve from an EMPTY CATEGORY: "+cPair);

        }
        var qna = QnAList[0];
        QnAList.RemoveAt(0);
        return qna;
    }

    public Categories getCategory(){
        return cPair;
    }
    //

    public void randomizeSets(){
        QnAList = QnAList.OrderBy(x => rand.Next()).ToList();
    }


    override public string ToString(){
        string str = "";
        foreach (QnA QA in QnAList){
            str = str + QA.ToString();
        }
        return str;
    }
    

}
