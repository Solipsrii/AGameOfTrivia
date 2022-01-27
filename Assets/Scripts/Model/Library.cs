using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Library
{
//This class is essentially a dictionary, only I actually know how it works kthanks.

    private List<Page> pages;
    System.Random rand = new System.Random();

    public Library(){
        var length = System.Enum.GetNames(typeof(Categories)).Length;
        pages = new List<Page>(); //-1 because category ANY is not actually used for actual indexing.
    }


    public void addItem(QnA item, Categories keyPair){
        addList(keyPair);
        getPage(keyPair).addItem(item);
    }

    private void addList(Categories keyPair){
        //ensures that categories are 'one of a kind'
        //if null, keypairing does not exist, so it is allowed to create a new one.
        //v categoryIndex fetches the keyPair's (enum Categories) value. i.e: sports = 0, Chemistry = 1, etc.
        var page = getPage(keyPair);
        if (page == null)
            pages.Add(new Page(keyPair));

    }

    public QnA getItem(Categories keyPair){
        var page = getPage(keyPair);
        QnA tmpSet = null;
            if (page != null){
                tmpSet = page.getSet(); //pop 1 from stack, removes 1 item from the QnA list.
            if (page.QnAList.Count == 0)   //if the entire stack is popped, remove category (page) from the list of available categories to choose from.
                pages.Remove(getPage(keyPair));
            }

        return tmpSet;
    }

    private Page getPage(Categories keyPair){
        int index;
        //retrieves a random Page (category)
        if (keyPair == Categories.Any){
            index = rand.Next(pages.Count-1);
                return (pages[index]);
            
        }
        //get specific page
        foreach(Page p in pages)
            if (p.getCategory() == keyPair)
                return p;
    //else
    Debug.Log("Library: tried to retrieve an empty set. Either an error or set ran out its course! (at category: "+keyPair+").");
    return null;
    }

    public void randomizeSets(){
        foreach(Page p in pages)
            p.randomizeSets();
    }

    override public string ToString(){
        string str = "";
        foreach(Page p in pages)
            str = str + p.ToString()+"\n\n";
        return str;
    }

    public Categories[] getArrayOfFunctioningCategories(){
        //lambda expression, returns an array of all available categories.
    return (pages.Select(cat => cat.getCategory())).ToArray();
    }

    public void printAllCategories(){
        int i =0;
        foreach(Page p in pages)
            Debug.Log((i++)+". "+p.getCategory());
    }

    
}
