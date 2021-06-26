using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour{

    public AudioClip playerDamage, playerJump, playerLost, playerWin, extraLife, goodApple, badApple;
    public bool lost = false;

    public void PlaySound(string som){
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        switch(som){
            case "playerDamage":
                audioSource.PlayOneShot(playerDamage);
                break;
            case "playerJump":
                audioSource.PlayOneShot(playerJump);
                break;
            case "playerLost":
                if(!lost){
                    audioSource.PlayOneShot(playerLost);
                    lost = true;
                }else{
                     Destroy(soundGameObject);
                }
                break;
            case "playerWin":
                audioSource.PlayOneShot(playerWin);
                break;
            case "extraLife":
                audioSource.PlayOneShot(extraLife);
                break;
            case "goodApple":
                audioSource.PlayOneShot(goodApple);
                break;
            case "badApple":
                audioSource.PlayOneShot(badApple);
                break;
        }
        Destroy(soundGameObject, 3.0f);
    }
}