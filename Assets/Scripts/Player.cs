using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{
    private Rigidbody2D _rigidbody;
    [SerializeField]
    float moveSpeed = .0f;
    public float rateToGrow = 1.5f;
    [SerializeField]
    float jumpSpeed = .5f;
    int extraJump = 1;
    public bool canJump = false;
    GameController gameController;
    public Vector3  levelScale = new Vector3(2f,2f,0f),
                    startScale = new Vector3(2f,2f,0f),
                    targetScale = new Vector3(1f,1f,0f);

    void Start() {
        _rigidbody = GetComponent<Rigidbody2D>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        transform.localScale = levelScale;
        changeMass(transform.localScale.x);
        freeze(true);
    }
    
    void Update() {
        if(Input.GetKeyDown(KeyCode.Space) && canJump && gameController.isLevelPlaying() && (extraJump > 0)){
            extraJump--;
            _rigidbody.velocity += Vector2.up * jumpSpeed;
        }
    }

    void FixedUpdate() {
        if(gameController.isLevelPlaying()){
            _rigidbody.AddForce(new Vector2(moveSpeed, 0), ForceMode2D.Impulse);
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.transform.tag == "Arrow"){
            other.gameObject.GetComponent<Arrow>().DestroyArrow();
            gameController.PlaySound("hit");
            if(rateToGrow >= 1.0f){
                gameController.playerShrink();
                StartCoroutine(changeSize("shrink"));
                rateToGrow -= 0.5f;
            }
        }
        if(other.transform.tag == "Level"){
            extraJump=1;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "LifeTrig"){
            gameController.playerGrow();
            rateToGrow += 0.5f;
            StartCoroutine(changeSize("grow"));
            Destroy(other.transform.gameObject);
        }

        if (other.tag == "Life"){
            gameController.playerGrow();
            rateToGrow += 0.5f;
            StartCoroutine(changeSize("grow"));
            GameObject parent = other.gameObject.transform.parent.gameObject;
            Destroy(parent);
        }
        if (other.tag == "Spring"){
            activateSpring();
        }
        if (other.tag == "Exit"){
            gameController.endLevel();
        }
        if (other.tag == "Spike"){
            gameController.PlaySound("loose");

            while(rateToGrow >= 0.5f){
                gameController.playerShrink();
                StartCoroutine(changeSize("shrink"));
                rateToGrow -= 0.5f;
                if(rateToGrow == 0.0f){
                    freeze(true);
                }
            }
        }
    }

    IEnumerator changeSize(string change){
        float elapsedTime = 0;
        float timeTakes = 0.5f;

        if(change == "grow"){
            startScale += targetScale;
        }else{
            if(startScale != Vector3.zero){
                startScale -= targetScale;
            }
        }
        
        while (elapsedTime < timeTakes) {
            transform.localScale = Vector3.Lerp(transform.localScale, startScale, (elapsedTime / timeTakes));
        
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        changeMass(transform.localScale.x);
    }

    public void activateSpring(){
        gameController.PlaySound("spring");
        _rigidbody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
    }

    public void freeze(bool freeze){
        if(freeze){
           _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }else{
            _rigidbody.constraints = RigidbodyConstraints2D.None;
        }
    }

    public void changeMass(float scale){
        switch(scale){
            case 1.0f:
                _rigidbody.mass = 5.0f;
                break;
            case 2.0f:
                _rigidbody.mass = 10.0f;
                break;
            case 3.0f:
                _rigidbody.mass = 15.0f;
                break;
            case 4.0f:
                _rigidbody.mass = 20.0f;
                break;

            default:
                _rigidbody.mass = 5.0f;
                break;
        }
    }
}