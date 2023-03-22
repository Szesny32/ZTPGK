using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    // Start is called before the first frame update
    public _GameManager gameManager;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     void OnTriggerEnter(Collider other){

        if (other.gameObject.name == "Plane"){

            Vector3 force = other.gameObject.transform.forward * 1000f;
            other.gameObject.GetComponent<Rigidbody>().AddForce(force);
            gameManager.addPoint();
        }
     }
}
