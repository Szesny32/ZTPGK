using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPause : MonoBehaviour
{
    bool animPlaying = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            animPlaying = !animPlaying;
            this.GetComponent<Animator>().speed = animPlaying ? 1f : 0f;
        }
    }
}