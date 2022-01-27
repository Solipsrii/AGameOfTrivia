using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//TODO: intercept number of questions on-startup, to properly count "how many rounds there are in the game". Possibly ask player for "how long do you want to play?"

public class newBank
{
    private List<List<QnA>> l_Any;
    private QnA currentSet;
    private int count;
    private System.Random r;
    public Library library;
    private int pushbackIndex;
    private string[] listOfCategories;
    
    private TextAsset csv;
        public newBank(){
            count = -1;
            library = new Library();
            r = new System.Random();

            //for efficiency sake, all future null categories will be pushed to the back, and pushbackIndex will NOT point to them.
            listOfCategories = (System.Enum.GetNames(typeof(Categories)));
            pushbackIndex = listOfCategories.Length-1; 
        }

        public newBank(GameType GT) : this(){
            if(GameType.fiftyfifty == GT){
                csv = Resources.Load("data/50_50") as TextAsset;
                init();
            }
            else if(GameType.normal == GT){
                csv = Resources.Load("data/normal") as TextAsset;
                init();
            }
        }
    
        private void init(){
            //line 0 = explanations for the columns, i.e: uselses data for the computer, line 1 = actual data read.
            //take total string and convert it to array of lines.
        string[] lineArr = csv.text.Split('\n');
        for (int i=1; i < lineArr.Length; i++){
            //create an array out of the words of this particular line.
            string[] setArr = lineArr[i].Split('@');
            //create a new QNA object, initialized with the line's question. (cell 0)
            Categories cat = stringToEnum(setArr[1]);
            QnA tmpQNA = new QnA(setArr[0], cat);

            for(int j=2; j < setArr.Length; j++){
                //skip 'question' & Category, which is in cell 0 & 1.
                tmpQNA.addAnswer(setArr[j]);
            }
            //check if line is valid, needs to have over 1 answers (2~4 answers are acceptable)
            if (tmpQNA.getAnswerCount() > 1){
                tmpQNA.init(); //shuffle the order of the answers, so correct-answer won't be the first on the list.
                library.addItem(tmpQNA, cat);
            }
        }
        //done adding, can finalize library
        library.randomizeSets();
        //Debug.Log(library.ToString());
    }

    private Categories stringToEnum(string s){
    //remove white spaces, i.e: in Computer Science.
    if(System.Enum.TryParse((s.Replace(" ", "")), out Categories c)){
        if (c == Categories.Any)
        Debug.Log("ERROR!: '"+s+"' was treated as Categories.ANY!!");
        return c;
    }
    //else
    Debug.Log("ERROR: Could not convert string to Categories, with: "+s);
    return Categories.Any;
    }


    public QnA getCurrentSet(){
        return currentSet;
    }

    public QnA getNextSet(){
        //no specified category, so, retrieves a set from a random category. Used by 5050.
        count++;
        return library.getItem(Categories.Any); 
    }

    public QnA getNextSet(Categories c){
        //get a random set (QnA), from a specified category.
        count++;
        return library.getItem(c);
    }

    public bool ReachedEndOfList(){
        //hard-coding all game modes to end at max 5 answers each.
        return (count == 4);
    }

    /*
    Retrieves an of array Categories[0,1] which ensures that the two chosen categories will not be the same.
    At the start of each operation, there's a function call to check if there are any new emptied out categories, and "pushes them out of range".
    */

    public Categories[] get2RandomCategories(){
    Categories cat1, cat2;
    Categories[] cats = library.getArrayOfFunctioningCategories();
    
    cat1 = cats[r.Next(cats.Length-1)];
    do { 
        cat2 = cats[r.Next(cats.Length-1)]; }
        while(cat2 == cat1);

    return new Categories[]{cat1, cat2};
    }
}

 
