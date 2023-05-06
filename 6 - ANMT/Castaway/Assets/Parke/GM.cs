using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{
    public BowyerWatson bowyerWatson;
    public MarkerInjector markerInjector;
    public GameObject camera;
    public GameObject actor;
    bool f1Mode = true;

    public Vector3 modulate ;

    void Start()
    {
        modulate = new Vector3(-0.15f, 1.82f, -0.62f);
         camera.transform.position = f1Mode? 
                actor.transform.position + modulate :
                actor.transform.position + new Vector3(2,0,0) + modulate ;
    }

    // Update is called once per frame
    void Update()
    {       


        
        if (Input.GetKeyDown(KeyCode.P)){
            bowyerWatson.changeParkMode();
        }

        if (Input.GetKeyDown(KeyCode.K)){
            bowyerWatson.AddKeyframe();
        }


        if (Input.GetKeyDown(KeyCode.C)){
            f1Mode = !f1Mode;
           
                camera.transform.position = f1Mode? 
                actor.transform.position + modulate :
                actor.transform.position + new Vector3(2,0,0) + modulate ;

            
        }

        if (Input.GetButtonDown("Fire1")){
            if(f1Mode){
                markerInjector.CreateMarker();
            } else {
                bowyerWatson.exclude();
            }
            
        }
        
        if (Input.GetButtonDown("Fire2")){
            bowyerWatson.switchShaderMode();
        }
        
    }
}
