using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator openDoorAnim;
    public Animator closeDoorAnim;
    public Animator CameraMoveAnim;

    private float time;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        StartAnimationController();
    }

    // Update is called once per frame
    void Update()
    {
        if(time < 2)time += Time.deltaTime;
        else 
    }

    void StartAnimationController()
    {
        openDoorAnim.SetTrigger("doorToOpen");
        
    }
}
