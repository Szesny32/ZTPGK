using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class _GameManager : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject plane;
    private int points = 0;
    public int maxPoints = 4;
    private float startTime = 0f;
    private float finishTime = 0f;
    private bool finished = false;
    private Vector3 startPosition;
    private Vector3 startRotation;
    private float timer;
    public float resetTime = 5f;

    void Start()
    {
        startPosition = plane.transform.position;
        startRotation = plane.transform.eulerAngles;
        resetMap();
    }

    // Update is called once per frame
    void Update()
    {
        if(points == maxPoints && finished == false){
            finished = true;
            finishTime = Time.time;
            text.text = "Congrats!\n Your time: "+(finishTime - startTime)+"s\n Time to reset: "+timer+"s";
        }

        else if(finished && timer > 0f){
            timer -= Time.deltaTime;
            text.text = "Congrats!\n Your time: "+(finishTime - startTime)+"s\n Time to reset: "+timer+"s";
        } 
        
        else if(finished && timer <= 0f){
            resetMap();
        }
    }

    public void addPoint(){
        points++;
        text.text = "You have "+points+" points";
    }
    void resetMap(){
        text.text = "";
        timer = resetTime;
        points = 0;
        startTime = Time.time;
        finishTime = 0f;
        plane.transform.position = startPosition;
        Quaternion rotation = Quaternion.Euler(startRotation);
        plane.transform.rotation = rotation;

        finished = false;
        plane.GetComponent<Rigidbody>().velocity = Vector3.zero;
        plane.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
