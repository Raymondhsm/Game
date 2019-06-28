using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// m_trigger是Player触碰引起事件的物体
/// m_target是事件发生后消失或出现的物体
/// </summary>
public class LabController : MonoBehaviour
{
    public GameObject m_trigger;//盛放追踪器的台子或电脑
    public GameObject m_target;//追踪器或传送光圈

    // Start is called before the first frame update
    void Start()
    {
        if (m_target.tag == "transport")
            m_target.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 碰撞到Player触发事件
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag=="Player")
        {
            //盛放追踪器的台子
            if(m_trigger.tag=="Desk")
            {
                m_target.SetActive(false);
            }
            //电脑
            if(m_trigger.tag=="Computer")
            {
                m_target.SetActive(true);
            }
        }
    }
}
