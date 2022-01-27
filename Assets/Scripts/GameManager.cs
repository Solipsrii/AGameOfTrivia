using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;
using System.IO;

/*
    TODO::
    1. Q Transition functions:
            (DONE) A) Templates (scenes?) for 2, 3, 4 questions. which:
                A) Alter position,
                B) Alter style (3 questions = cardboard, 4 questions = text with different fonts, 2 questions = Split screen)
        B) Animation functions. Ease-in, ease out.
        i.e: out-transition:
            A) play sound
            (v?) B) answers go out of screen (partially done)
            (v) C) zoom into black BG (done)
            (v) D) fade to black (done)
            (v) E) initiate IN-TRANSITION, basically the same thing but in reverse & without zoom. A B D. (done for the most part other than sounds)

    2. motherfucking FUNCTIONALITY
        >V A)  Clicking on an answer (button) checks if it's correct or not.
        B) Clicking on a button initiates a move-scene sequence.
          ***SWITCH SCENE  A) moves on to the next question, and according to the number of answers, loads the correct scene and DELOADS THE CURRENT ONE!
                A) list.next
                B) switch(question)
                C) load scene(2-4)
                D) deload scene    
            B) initiate TRANSITION FUNCTION OUT & IN to another scene.
        (V) C) Override text with CSV values for current question (initialized in list bank)
        D) "getNextQuestion", create a system which intercepts the number of questions on-startup, to properly count "how many rounds there are in the game". Possibly ask player for "how long do you want to play?"


    // (probably not needed w/current implementation.)
    1. create and edit method to initialize <list roundsList>. method should create either a random set of GameTypes, under a strict algorithm to not overflow beyond the total amount of QnA available (i.e: if only 5 normal questions, can't have >5 currentSet in databaseNormal.)
*/

//TODO: 4. Add timer to 5050

public class GameManager : MonoBehaviour
{
    public QnA currentSet;

    //public GameObject buttonPrefabA2, buttonPrefabA3, buttonPrefabA4, TXTPrefabA2, TXTPrefabA3, TXTPrefabA4;
    public newBank databaseNormal, database5050;
    public int correctAnswers;
    //init GameOrderAlreadySet b4 Start(), so if manipulated externally, its bool val would actually be valuable.
    private bool alreadyAnswered, gameOrderAlreadySet = false;
    private readonly object key = new object();
    private MainAnimationHelper AH;
    private AudioManager audioManager;
    private int roundsIndex;
    private List<GameType> roundsList;
    public bool NextCategoryNotChosen, CanOpenPauseMenu;
    public Categories currentCategory;
    public Categories[] choose2Categories;

    public GameObject pauseMenuParent;
    public Slider audioSlider;
    public TextAsset csv_AudioLevel;
    public Text audioLevelsInNumbers;

    // Start is called before the first frame update
    void Start()
    {
         //initialize variables
         alreadyAnswered = false;
         correctAnswers = 0;
         roundsIndex = 0;
         NextCategoryNotChosen = true;
         CanOpenPauseMenu = true;

        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
        //csv_audioLevel is only good for this 1 shot. UNITY only reads from file ONCE, so read-write operations fucks this up. Look at LateUpdate for fix (stream reader / writer).
        var volumeFromPrefs = PlayerPrefs.GetFloat("MasterVolume", 1f);
        audioSlider.value = volumeFromPrefs;
        audioManager.changeAudioLevel(volumeFromPrefs);
        alterVolumeUI();
        
        AH = this.GetComponent<MainAnimationHelper>();
        AH.init(this);
        databaseNormal = new newBank(GameType.normal);
        database5050 = new newBank(GameType.fiftyfifty);

        //default game mode goes up to 5 rounds. Try to initialize, if not already initialized by Menu.
        initRoundOrder(new List<GameType>(){
            GameType.normal, GameType.normal, GameType.normal, GameType.normal, GameType.fiftyfifty
            });
        
        //ensure that the end-game is at the last spot of the list.
        roundsList.Add(GameType.endgame);

        //
        //initialize game objects
        //
        //temp, change this as well. Only play on ACTUAL GAME INTRO. (how to tell when the game actually starts?)
        //

        //game gets started by invoking method startGame(), via the introAnimator (see canvas).
    }

    void LateUpdate(){
        //enable / disable pause menu
        if (Input.GetKeyDown(KeyCode.Escape) && (CanOpenPauseMenu) && (getRound() != GameType.endgame)){
            CanOpenPauseMenu = false;

            pauseMenuParent.SetActive(true);
            Time.timeScale = 0; //this parameter turning 0 is what causes all Unity-handled stuff stop in its tracks.
            audioManager.pauseAllSFX();
            //pause all DOTWEENS animations
            DOTween.TogglePauseAll();
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && (!CanOpenPauseMenu)){
            //write to playerPrefs;
            //
            CanOpenPauseMenu = true;
            pauseMenuParent.SetActive(false);
            Time.timeScale = 1;
            audioManager.unPauseAllSFX();
            //resume all DOTWEEN animations
            DOTween.TogglePauseAll();
        }
    }

    public void startGame(){
        StartCoroutine(loadIntroByType(getRound()));
    }

    //for 5050, add a point if 3/5 were answered correctly.
    public void reportResult(int answers){
    if (answers >= 3){
        reportResult(true);
        }
    else    
        reportResult(false);
    }

    //for 'normal' and generic.
    public void reportResult(bool flag){
        lock(key){
            if(!alreadyAnswered)
            {
            Debug.Log("GM. I was locked!");
            alreadyAnswered = true;
            //
            if (flag){
                correctAnswers++;
                AH.animate_correctAnswer();
                //audioManager.Play(audioManager.A_SFX, audioManager.clip_clickRight);
            } else{
                AH.animate_wrongAnswer();
                //audioManager.Play(audioManager.A_SFX, audioManager.clip_clickWrong);
            }
            Debug.Log("Num of correct answers: "+correctAnswers);
            ///
            }
        }
    }

    //TODO: MOVE v TO MAIN ANIMATION HELPER.
    //well that's not going to happen.


    //called on scene START, when trying to load a scene. i.e: right when "outro" ends.
    private IEnumerator animateIntro(){
        /*
        add some sort of a buffer animation here, like how they do in jackbox. Like a film drop with the new category.
        Yeah, like "stage 1!", "stage 2!"...
        */
        SceneManager.UnloadSceneAsync(getScene(), UnloadSceneOptions.None);
        
        //intro ended, can advance roundindex
        roundsIndex++;
        yield return loadIntroByType(getRound());
        yield break;
        
    }

    //loads the next animated intro, for the correct following scene, and boots up that scene.
    private IEnumerator loadIntroByType(GameType type){
        switch (type){
            case(GameType.normal):
                choose2Categories = databaseNormal.get2RandomCategories();
                StartCoroutine(AH.animateIntro(MainAnimationHelper.IntroType.BlurBackground));
                SceneManager.LoadSceneAsync("Categories_hands", LoadSceneMode.Additive);

                while(NextCategoryNotChosen)
                    yield return new WaitForSeconds(0.5f);

                StartCoroutine(AH.animateOutro(MainAnimationHelper.OutroType.BlurBackground));
                SceneManager.UnloadSceneAsync("Categories_hands", UnloadSceneOptions.None);
                break;
        }

        nextScene();
    }
    public void animateOutro(MainAnimationHelper.OutroType type){
        StartCoroutine(_animateOutro(type));
    }

    //initiates on SCENE END, which manipulates things like "Background blur" or "Zoom into background".
    private IEnumerator _animateOutro(MainAnimationHelper.OutroType type){
        yield return (AH.animateOutro(type));
        //
        //insert in-between scenes here if so desire//
        //load categories-scene, which will by the end of it trigger GM's "intro animation".
        StartCoroutine(animateIntro());
        yield break;
    }

    public void animateBGIn(MainAnimationHelper.IntroType type){
       StartCoroutine(AH.animateIntro(type));
    }


    public void nextScene(){
        //re-initialize variables for the next scene
        currentSet = getNextSet(currentCategory);
        SceneManager.LoadSceneAsync(getScene(), LoadSceneMode.Additive);
        //finally, globally increase roundsIndex by 1.
        alreadyAnswered = false;
        NextCategoryNotChosen = true;
    }

    private QnA getNextSet(Categories c){
            switch(getRound()){
            case GameType.normal:
                return databaseNormal.getNextSet(c);

            case GameType.fiftyfifty: 
                return database5050.getNextSet();
        }
        Debug.Log("Error! getNextSet() returned a null set!");
        return null;
    }

    private GameType getRound(){
        return roundsList[roundsIndex];
    }
    private string getScene(){
        Debug.Log("Tried to return scene-type: "+getRound());

        switch(getRound()){
            case (GameType.normal):
                switch(currentSet.answers.Count){
                case 3:
                    return "threeAnswers";
                case 4: 
                    return "fourAnswers";
                default: break;
            } break;

        case (GameType.fiftyfifty):
            return "twoAnswers_1";
        

        case (GameType.endgame):
            return "Endgame";
        
        default: Debug.Log("ERROR: getScene returned \"null\""); break;
        }
        
        return null;
    }


    public void initRoundOrder(List<GameType> list){
        if(gameOrderAlreadySet == false){
            gameOrderAlreadySet = true;
            //TODO: uncomment the following line, randomize the list.
           // roundsList = list.OrderBy(x => Random.value).ToList<GameType>();
           roundsList = list;
        }
    }



    //button & slider events
    
    public void QuitToMenu(){
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void audioSliderChanged(){
        //values from 0.0 ~ 1.0
        PlayerPrefs.SetFloat("MasterVolume", audioSlider.value);
        audioManager.changeAudioLevel(audioSlider.value);
    }

    public void alterVolumeUI(){
        audioLevelsInNumbers.text = ""+((int)(audioSlider.value*100))+"%";
    }


    private enum SETS
    {
        CURRENT,
        NEXT
    }
}
