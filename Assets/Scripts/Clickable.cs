using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clickable : MonoBehaviour{
    public int trapWasClicked = 0;
    public int ID = 0;
    public AudioSource clickSound;
    public float secondsToActivate = 0.0f;
    public float secondsToActivateCountdown = 0.0f;
    float valuetoset = 0.05f;
    GameController gameController;
    public Text traptext;
    [SerializeField]
    bool isSetting = false;
    bool stopSetting = false;
    bool working = false;
    bool add = true;
    public bool canClick = true;
    public Rigidbody2D arrow, food;
    public SpriteRenderer myRenderer;

    void Awake() {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        myRenderer = this.GetComponent<SpriteRenderer>();
        clickSound = GameObject.Find("Traps").GetComponent<AudioSource>();        
    }

    void Start() {
        if(ID == 1 || ID == 2 || ID == 3){
            arrow = GetComponentInChildren<Rigidbody2D>();
        }else{
            food = GetComponentInChildren<Rigidbody2D>();
            if(ID == 4){
                secondsToActivate = 0.5f;
                secondsToActivateCountdown = 0.5f;
            }
            if(ID == 5){
                secondsToActivate = 1.5f;
                secondsToActivateCountdown = 1.5f;
            }
        }
    }

    void Update(){
        if(gameController.isLevelPlaying()){
            if(!working){
                StartCoroutine(countdown());
            
                if(ID == 1 || ID == 2 || ID == 3){
                    StartCoroutine(activateTrap());
                }else{
                    StartCoroutine(activateFood());
                }
                working = true;
            }
        }else{
            if (Input.GetMouseButtonDown(0)){
                add = true;
                stopSetting = false;
                makeTrue();
            }
            if (Input.GetMouseButtonUp(0)){
                add = true;
                isSetting = false;
                stopSetting = true;
            }

            if (Input.GetMouseButtonDown(1)){
                add = false;
                stopSetting = false;
                makeTrue();
            }
            if (Input.GetMouseButtonUp(1)){
                add = false;
                isSetting = false;
                stopSetting = true;
            }

            if(isSetting){
                makeFalse();
                if(canClick){
                    trapClicked(add);
                }
                
            }
        }
        updateText();
    }

    public void makeTrue(){
        isSetting = true;
    }

    public void makeFalse(){
        isSetting = false;
        if(stopSetting == false){
            Invoke("makeTrue", .2f);
        }
    }

    private void trapClicked(bool operation){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    
        RaycastHit2D hits2D = Physics2D.GetRayIntersection(ray);
        if(hits2D.collider != null){
            if(hits2D.transform.tag == "Trap"){
                trapWasClicked = hits2D.transform.GetComponent<Clickable>().ID;
                if(ID == 1 || ID == 2 || ID == 3){
                    playClickSound();
                }
                if(operation){
                    setTrapMili(valuetoset);
                }else{
                    setTrapMili(-valuetoset);
                }
            }
        }
    }

    public void playClickSound(){
        if(!clickSound.isPlaying){
            clickSound.Play();
        }
    }

    public void setTrapMili(float value){
        if(trapWasClicked == this.ID){
            this.secondsToActivate += value;
            if(secondsToActivate < 0.0f){
                secondsToActivate = 0.0f;
            }
            this.secondsToActivateCountdown = this.secondsToActivate;
        }
    }

    public void updateText(){
        traptext.text = secondsToActivateCountdown.ToString("#0.0");
    }

    IEnumerator activateTrap(){
        myRenderer.color = MyColors.yellow;
        yield return new WaitForSeconds(this.secondsToActivate);
        myRenderer.color = MyColors.green;

        if(this.arrow != null){
            gameController.PlaySound("released");
            this.arrow.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
            arrow.velocity += Vector2.down;
        }
    }

    IEnumerator activateFood(){
        myRenderer.color = MyColors.yellow;
        yield return new WaitForSeconds(this.secondsToActivate);
        myRenderer.color = MyColors.green;

        if(this.food != null){
            gameController.PlaySound("released");
            this.food.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
            food.velocity += Vector2.down;
        }
    }

    IEnumerator countdown(){
        while(this.secondsToActivateCountdown >= 0.1f){
            yield return new WaitForSeconds(.1f);
            this.secondsToActivateCountdown -= .1f;
        }
        this.secondsToActivateCountdown = .0f;
    }

    void OnMouseEnter(){
        if(ID == 1 || ID == 2 || ID == 3){
            myRenderer.color = MyColors.yellow;
        }
    }

    void OnMouseExit() {
        myRenderer.color = MyColors.black;
    }
}