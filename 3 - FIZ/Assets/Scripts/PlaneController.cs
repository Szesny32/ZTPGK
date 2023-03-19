using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    


    private Rigidbody rigidbody;

    [Range(0,1)]
    public float engineUsage = 0.5f;
    public float engineUsageAcceleration = 0.01f;
    
    public float engineMaxPower = 100f;

    public float climbRate = 0.0001f;

    public float rotationSpeed = 0.01f;

    public float leftRight = 0.0f;
    public float upDown = 0.0f;


    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        
  Keyboard();

        //Quaternion rotation = Quaternion.Euler(angleVelocity * Time.deltaTime);
        //rigidbody.MoveRotation(rigidbody.rotation * rotation);

        //rotation = Quaternion.Euler(angleVelocity * Time.deltaTime);
        //rigidbody.MoveRotation(rigidbody.rotation * rotation);
        Vector3 angleVelocity = transform.up * leftRight *  rotationSpeed;
        rigidbody.AddTorque(angleVelocity, ForceMode.Acceleration);

        // angleVelocity = -transform.forward * leftRight *  rotationSpeed;
        //  Quaternion rotation = Quaternion.Euler(angleVelocity);
        // rigidbody.MoveRotation(rigidbody.rotation * rotation);

        angleVelocity = transform.right * upDown* rotationSpeed;
        rigidbody.AddTorque(angleVelocity, ForceMode.Acceleration);
    


     



        Vector3 force = transform.forward * engineUsage * engineMaxPower;
        
        
        if(rigidbody.velocity.magnitude > engineUsage*engineMaxPower){



            //rigidbody.velocity = rigidbody.velocity.normalized * engineUsage* engineMaxPower;
            rigidbody.velocity = Vector3.Slerp(rigidbody.velocity, transform.forward.normalized * engineUsage* engineMaxPower, Time.deltaTime);

            force = Vector3.zero;

        }
        rigidbody.AddForce(force);
        
                
        
        rigidbody.useGravity = engineUsage < 0.1f;






    }

    void LateUpdate(){
  

    }

    void Keyboard(){
        if(Input.GetKey(KeyCode.W)){
            upDown = Mathf.Clamp(upDown-0.01f, -1f, 1f);
        } 
        if(Input.GetKey(KeyCode.A)){
            leftRight = Mathf.Clamp(leftRight-0.01f, -1f, 1f);
        }
        if(Input.GetKey(KeyCode.S)){
             upDown = Mathf.Clamp(upDown+0.01f, -1f, 1f);
        }
        if(Input.GetKey(KeyCode.D)){

            leftRight = Mathf.Clamp(leftRight+0.01f, -1f, 1f);





         }
        if(Input.GetKey(KeyCode.UpArrow)){
           engineUsage = Mathf.Clamp(engineUsage+ engineUsageAcceleration , 0f, 1f);
        }
        if(Input.GetKey(KeyCode.DownArrow)){
            engineUsage = Mathf.Clamp(engineUsage-engineUsageAcceleration, 0f, 1f);
        }

    }
}
