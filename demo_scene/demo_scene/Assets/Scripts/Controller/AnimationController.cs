using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator openDoorAnim;
    public Animator CameraMoveAnim;
    public GameObject door;

    private float time;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(StartAnimationController());
        StartAnimationController();
    }

    // Update is called once per frame
    void Update()
    {

        GameObject cam = GameObject.Find("Main Camera");
        cam.GetComponent<Transform>().position = new Vector3(8.3f, 0,0);

    }

    void StartAnimationController()
    {
        AudioSource au = door.GetComponent<AudioSource>();

        //开门
        openDoorAnim.SetTrigger("doorToOpen");
        au.Play();
        
        //走进去
        // AnimatorStateInfo info = openDoorAnim.GetCurrentAnimatorStateInfo(0);
        // while(!info.IsName("doorOpened")){
        //     yield return 0;
        // }
        CameraMoveAnim.SetTrigger("CameraForward");
    }
}
