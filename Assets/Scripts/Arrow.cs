using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour{
    public GameObject ArrowParticles;
    GameController gameController;
    void Start() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }
    
    public void DestroyArrow(){
        Instantiate(ArrowParticles, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    void OnCollisionEnter2D(Collision2D other) {
        if(other.transform.tag == "Level"){
            gameController.PlaySound("hit");
            DestroyArrow();
        }
    }
}