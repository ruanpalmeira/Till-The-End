using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour{
    Transform player;
    public Vector3 offset;
    public GameController gameController;
    public float leftBarrier = 0;
    public float rightBarrier = 0;
    public float upBarrier = 0;
    public float downBarrier = 0;
    public float speed = 5f;
    public float smoothedSpeed = 0.125f;
    public Vector3 startPosition;
    public Vector3 lastPosition;
    
    void Start(){
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        player = GameObject.Find("Player").transform;
        startPosition= transform.position;
    }

    void FixedUpdate(){
        if(gameController.isCameraToggle()){
            Vector3 desiredPos = player.position + offset;
            Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothedSpeed);
            transform.position = smoothedPos; 
        }
    }

    void Update(){
        if(!gameController.isCameraToggle()){
            controlCamera();
        }
    }

    void FollowPlayer(){
        transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, offset.z);
    }

    void controlCamera(){
        var movement = Vector3.zero;
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        if(xAxis < 0){
            movement.x--;   
        }
        if(xAxis > 0){
            movement.x++;   
        }
        if(yAxis < 0){
            movement.y--;   
        }
        if(yAxis > 0){
            movement.y++;   
        }
        transform.Translate(movement * speed * Time.deltaTime, Space.Self);
        transform.position = new Vector3(
        Mathf.Clamp(transform.position.x, leftBarrier, rightBarrier),
        Mathf.Clamp(transform.position.y, downBarrier, upBarrier), -10);
    }

    public void lastPos(){
        lastPosition = transform.position;
        transform.position = lastPosition;
    }

    public void setPos(){
        if(gameController.isCameraToggle()){
            transform.position = startPosition;
        } 
    }
}