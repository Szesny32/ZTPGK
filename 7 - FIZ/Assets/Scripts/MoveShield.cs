using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveShield : MonoBehaviour
{
    private Rigidbody rigidbody;
    
    public Vector3 originPos;
    public float amplitude = 10.0f;
    public float frequency = 10.0f;
    public float positionRange = 2.0f;

    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        originPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 position = originPos;
        position.x += positionRange*Mathf.Sin(Time.time);
        transform.position =  position; 


        Vector3 angles = new Vector3(0f,0f,Mathf.Sin(Time.time*frequency)*amplitude);
        Quaternion rotation = Quaternion.Euler(angles);
        transform.rotation = rotation;

    
    }
}
