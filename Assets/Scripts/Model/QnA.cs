using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//defines the model for 1 question per X answers.
public class QnA
{
    public string question;
    public List<string> answers;
    public string correctAnswer;
    public Categories category;

    public QnA(){
        answers = new List<string>();
    }

    public QnA(string Q) : this(){
    //for error testing purposes!
    this.question = Q;
    }
    public QnA(string Q, Categories category) : this(Q){
        this.category = category;
    }

    public QnA(string Q, string a1, string a2) : this(Q){
        setAnswers(new string[]{a1,a2});
    }
    public QnA(string Q, string a1, string a2, string a3) : this(Q){
        setAnswers(new string[]{a1,a2,a3});
    }
    public QnA(string Q, string a1, string a2, string a3, string a4) : this(Q){
        setAnswers(new string[]{a1,a2,a3,a4});
    }
    

    public int getAnswerCount(){
        return answers.Count;}

    public void setAnswers(string[] sArr){
        //todo: remove this after qnabank is done.
        if (sArr.Length == 0){
            Debug.Log("ERROR in QnA.cs: setAnswers() received an empty array!");
            return;
        }
        foreach(string s in sArr)
            addAnswer(s);
    }

    public void addAnswer(string s){
        if (string.IsNullOrWhiteSpace(s)){
           // Debug.Log("ERROR in QnA.cs: setAnswer() received an empty string as an answer in question '"+question+"'!.");
            return;
        }
        answers.Add(s);       
    }

    //TODO::remove this function after testing.
    public void printAllAnswers(){
        Debug.Log("Printing all answers for question: "+question);
        Debug.Log("-----");
        foreach(string answer in answers)
            Debug.Log(" "+answer);
        Debug.Log("-----");
    }

    public void init(){
        correctAnswer = answers[0];
        //The shuffle solution -- Kinda wonky stream hacky shit mate. Basically, for each item in the list, "give" a random ID. Then sort the items by these "IDs". Eh.
        answers = answers.OrderBy(x => Random.value).ToList<string>();
    }

    override public string ToString(){
        string str = "Question is: "+question+".\nAnswers are: ";
        foreach(string answer in answers)
            str = str+", "+answer;
        str += ".\n";
        return str;
    }
    
}
