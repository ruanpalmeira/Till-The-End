using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour{
    [SerializeField]
    bool levelPlaying = false;
    public bool isMenu = false;
    public bool isSelection = false;
    public bool lost = false;
    public bool rolling = false;
    public Toggle cameraToggle;
    public AudioClip grow, shrink, win, roll, hit, released, spring, loose, food;
    AudioSource jukebox;
    public Vector3[] arrowTrapsPositions, foodTrapsPositions, livesPositions;
    public Player player;

    public GameObject imageobj, circleobj, startScreenobj, selectScreenobj, endScreenobj,
                        spaceTip, extraLifeobj, spawnPoint, trapPrefab, foodPrefab;
    public GameObject[] levelArrowTraps, levelFoodTraps, levelLives;
    public Transform arrowTrapsParent, foodTrapsParent;
    public Transform extraLivesParent, soundsParent;
    public int extraLivesAmount = 0, arrowTrapsAmount = 0, foodTrapsAmount = 0;
    public CameraFollow cameraFollow;
    public Animator image, circle, transition, startScreen, selectScreen, endScreen;
    public int levelToUnlock;
    public List<GameObject> levelSounds;
    
    void Awake() {
        #if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
        #else
            Debug.unityLogger.logEnabled = false;
        #endif
    }

    void Start() {
        jukebox = GetComponent<AudioSource>();

        if(isMenu == true){
            image.SetBool("image", true);
            startScreenobj.SetActive(true);
            selectScreenobj.SetActive(false);
            endScreenobj.SetActive(false);   
            circleobj.SetActive(false);
                       
        }else{
            if(isSelection == true){
                selectScreen.SetBool("select", true);
                imageobj.SetActive(false); 
                startScreenobj.SetActive(false);
                selectScreenobj.SetActive(true);
                endScreenobj.SetActive(false);
                circleobj.SetActive(false);   
            }else{
                imageobj.SetActive(false); 
                circleobj.SetActive(true);
                circle.SetInteger("Circle", 1);
                startScreenobj.SetActive(false);
                selectScreenobj.SetActive(false);
                endScreenobj.SetActive(false);
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                loadTraps();
                
                StartCoroutine(circleWait());
            }
        }
    }

    public void startLevel(){
        levelPlaying = true;
        player.freeze(false);
        PlaySound("roll");
        rolling = true;
    }
    
    public void endLevel(){
        StopSounds();
        jukebox.clip = win;
        jukebox.Play();
        imageobj.SetActive(true); 
        image.SetBool("image", false);
        endScreenobj.SetActive(true); 
        endScreen.SetBool("end", true);
        levelPlaying = false;
        PlayerPrefs.SetInt("levelReached", levelToUnlock);
        StopSounds();
    }

    public bool isLevelPlaying(){
        return levelPlaying;
    }

    public void loadLevel(string level){
        switch(level){
            case "menu":
                SceneManager.LoadScene(0);
                break;

            case "select":
                SceneManager.LoadScene(1);
                break;
            
            case "next":
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;
            
            case "restart":
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;

            default:
                
                StartCoroutine(fadeWait(int.Parse(level) + 1));
                break;
        }
    }

    public void exitGame(){
        Application.Quit();
    }

    public bool isCameraToggle(){
        return cameraToggle.isOn;
    }

    public void playerGrow(){
        jukebox.clip = grow;
        jukebox.Play();
    }

    public void playerShrink(){
        jukebox.clip = shrink;
        jukebox.Play();
    }

    public void saveTraps(){
        rolling = false;
        StopSounds();
        player.freeze(true);
        circle.Rebind();
        circle.SetInteger("Circle", 1);

        if(levelArrowTraps != null){
            int activeScene = SceneManager.GetActiveScene().buildIndex;

            for (int i = 0; i < levelArrowTraps.Length; i++){
                var trap = levelArrowTraps[i].GetComponent<Clickable>().secondsToActivate;
                if( trap != 0.0f){
                    PlayerPrefs.SetFloat("trap" +  activeScene + i+1, trap);
                }
            }
        }

        if(levelFoodTraps != null){
            int activeScene = SceneManager.GetActiveScene().buildIndex;

            for (int i = 0; i < levelFoodTraps.Length; i++){
                var trap = levelFoodTraps[i].GetComponentInParent<Clickable>().secondsToActivate;
                if( trap != 0.0f){
                    PlayerPrefs.SetFloat("foodTrap" +  activeScene + i+1, trap);
                }
            }
        }
        cameraFollow.setPos();

        levelPlaying = false;
        loadTraps();
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        player.transform.localScale = player.levelScale;
        player.changeMass(player.transform.localScale.x);
        player.transform.position = spawnPoint.transform.position;
        player.GetComponent<Player>().rateToGrow = 1.5f;
        player.GetComponent<Player>().startScale = player.GetComponent<Player>().levelScale;
        StartCoroutine(circleWait());
        
    }

    public void loadTraps(){
        StopSounds();
        if(arrowTrapsAmount != 0){
            int j = 1;
            for (int i = 0; i < arrowTrapsAmount; i++){

                if(levelArrowTraps[i] != null){
                    Destroy(levelArrowTraps[i].gameObject);
                }

                var newTrap = Instantiate(trapPrefab, transform.position, Quaternion.identity);
                
                newTrap.transform.SetParent(arrowTrapsParent);
                newTrap.transform.position = arrowTrapsPositions[i];
                newTrap.GetComponentInChildren<Clickable>().ID = i + j;
                levelArrowTraps[i] = newTrap;
            }
        }

        if(foodTrapsAmount != 0){
            int j=4;
            for (int i = 0; i < foodTrapsAmount; i++){
                if(levelFoodTraps[i] != null){
                    Destroy(levelFoodTraps[i].gameObject);
                }

                var newTrap = Instantiate(foodPrefab, transform.position, Quaternion.identity);
                
                newTrap.transform.SetParent(foodTrapsParent);
                newTrap.transform.position = foodTrapsPositions[i];
                var newTrapScript = newTrap.GetComponentInChildren<Clickable>();
                newTrapScript.ID = i + j;
                if(newTrapScript.ID == 4){
                    newTrapScript.secondsToActivate = 0.5f;
                    newTrapScript.secondsToActivateCountdown = 0.5f;
                }
                if(newTrapScript.ID == 5){
                    newTrapScript.secondsToActivate = 1.5f;
                    newTrapScript.secondsToActivateCountdown = 0.5f;
                }
                levelFoodTraps[i] = newTrap;
            }
        }

        if(extraLivesAmount != 0){
            for (int i = 0; i < extraLivesAmount; i++){
                if(levelLives[i] != null){
                    Destroy(levelLives[i].gameObject);
                }

                var newLife = Instantiate(extraLifeobj, transform.position, Quaternion.identity);
                
                newLife.transform.SetParent(extraLivesParent);
                newLife.transform.position = livesPositions[i];
                levelLives[i] = newLife;
            }
        }

        int activeScene = SceneManager.GetActiveScene().buildIndex;
       
        float trapValue = 0.0f;

        for (int i = 0; i < arrowTrapsAmount; i++)
        {
            trapValue = PlayerPrefs.GetFloat("trap" +  activeScene + i+1, 0.0f);
            var trapScript = levelArrowTraps[i].GetComponent<Clickable>();
            trapScript.secondsToActivate = trapValue;
            trapScript.secondsToActivateCountdown = trapValue;
            trapScript.canClick = true;
            trapValue = 0.0f;
        }

        for (int i = 0; i < foodTrapsAmount; i++)
        {
            trapValue = PlayerPrefs.GetFloat("foodTrap" +  activeScene + i+1, 0.0f);
            var trapScript = levelFoodTraps[i].GetComponent<Clickable>();
            trapScript.secondsToActivate = trapValue;
            trapScript.secondsToActivateCountdown = trapValue;
            trapValue = 0.0f;
        }
        cameraToggle.isOn = false;
        
    }

    IEnumerator fadeWait(int level){
        circle.Rebind();
        circleobj.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(level);
    }

    IEnumerator circleWait(){
        yield return new WaitForSeconds(.5f);
        circle.SetInteger("Circle", 2);

        if(levelToUnlock == 4){
            yield return new WaitForSeconds(.5f);
            spaceTip.SetActive(true);
        }
    }

    public void PlaySound(string som){
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        soundGameObject.transform.SetParent(soundsParent);
        levelSounds.Add(soundGameObject);

        switch(som){
            case "roll":
                if(!rolling){
                    audioSource.loop = true;
                    audioSource.clip = roll;
                    audioSource.Play();
                }
                break;
            case "hit":
                audioSource.PlayOneShot(hit);
                break;
            case "released":
                audioSource.PlayOneShot(released);
                break;
            case "spring":
                audioSource.PlayOneShot(spring);
                break;
            case "food":
                audioSource.PlayOneShot(food);
                break;
            case "loose":
                audioSource.PlayOneShot(loose);
                break;
        }
    }  

    public void StopSounds(){
        for (int i = 0; i < levelSounds.Count; i++){
            if(levelSounds[i] != null){
                Destroy(levelSounds[i].gameObject);
            }
        }
    }
}