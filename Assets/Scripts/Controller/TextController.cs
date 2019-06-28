using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 在教程关卡中名为“textPlane”的平面或游戏物体为放置脚本的物体
/// 然后将要触发的3D Text在面板中初始化即可
/// </summary>
public class TextController : MonoBehaviour
{
    public GameObject texts;//教程关卡的指导字幕
    private string content;//内容存储
    private bool isActive;
    public float liveTime = 2.0f;//字幕存在时间
    private float timer = 0.0f;//计时器

    // Start is called before the first frame update
    void Start()
    {
        content = texts.GetComponent<TextMesh>().text;
        texts.GetComponent<TextMesh>().text = "";
        isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        OnFinish();
    }

    /// <summary>
    /// 玩家通过触碰到特定地点来引发指导字幕
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionStay(Collision other)
    {
        if (!isActive) return;
        if(other.gameObject.tag=="Player")
        {
            texts.GetComponent<TextMesh>().text = content;
            isActive = false;
        }
    }

    /// <summary>
    /// 指导字幕显示一定时间后自动消失
    /// </summary>
    private void OnFinish()
    {
        if (isActive) return;
        timer += Time.deltaTime;
        if(timer>=liveTime)
        {
            texts.GetComponent<TextMesh>().text = "";
            isActive = true;
            timer = 0.0f;
        }
    }
}
