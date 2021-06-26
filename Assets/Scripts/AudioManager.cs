using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour{
    static AudioManager instance;

    void Awake() {
        if (instance != null){
            Destroy(gameObject);
        }else{
            instance = this;
            DontDestroyOnLoad(transform.gameObject);
        }

        PlayerPrefs.SetInt("levelReached", PlayerPrefs.GetInt("levelReached", 1));
    }
}