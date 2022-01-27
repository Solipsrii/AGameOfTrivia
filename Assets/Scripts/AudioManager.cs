using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public AudioClip clip_clickRight,clip_clickWrong, clip_zoom, clip_endgame_Claps, clip_endgame_Confetti, clip_timer_ticktockNormal, clip_timer_LoomingBoom, bgm_music;
    public AudioSource A_SFX, A_Timer, A_BGM, A_LoomingBoom;
    public float currentLevel;
    
    void Start()
    {
        List<AudioSource> bootuplist = new List<AudioSource>(GetComponents<AudioSource>());
        A_SFX = bootuplist[0];
        A_Timer = bootuplist[1];
        A_BGM = bootuplist[2];
        currentLevel = PlayerPrefs.GetFloat("MasterVolume");

        A_BGM.Play();
    }

    public void pauseAllSFX(){
        A_SFX.Pause();
        A_Timer.Pause();
    }

    public void changeAudioLevel(float newLevel){
        currentLevel = newLevel;
        A_SFX.volume = newLevel;
        A_Timer.volume = newLevel;
        A_BGM.volume = newLevel;
        A_LoomingBoom.volume = newLevel;
    }

    public void unPauseAllSFX(){
        A_SFX.UnPause();
        A_Timer.UnPause();
    }

    //creates a linear fade out effect.
    public void fade(AudioSource audio, float newValue, float duration){
        StartCoroutine(_fade(audio, newValue, duration));
    }

    private IEnumerator _fade(AudioSource audio, float newValue, float duration){
                // d = ((a(n) - a1)/ n-1), tho the "-1" is irrelevant here:
        float changesPerFrame = ((newValue - audio.volume) / (duration));
        while((audio.volume > newValue ) || (audio.volume > 0)){
            audio.volume += (changesPerFrame * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }


    // play_...//

    public void Play(AudioSource source, AudioClip clip){
        source.volume = currentLevel;
        source.PlayOneShot(clip);
    }

/*
    public void play_CorrectClick(){
        A_SFX.clip = clickRight;
    //    A_src.Play();
    }
        public void play_IncorrectClick(){
        A_SFX.clip = clickWrong;
    //    A_src.Play();
    }

    public void play_zoomIn(){
        A_SFX.PlayOneShot(zoom);
    }

        public void play_clockSlow(){
        A_Timer.PlayOneShot(ticktockNormal);
    }

        public void play_clockFast(){
        A_Timer.Stop();
        A_Timer.PlayOneShot(ticktockNearEnd);
    }
        public void play_endgameConfetti(){
            A_SFX.PlayOneShot(endgameConfetti);
        }
        public void play_endgameClaps(){
            A_BGM.PlayOneShot(endgameClaps);
        }
*/
}
