using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedCube : MonoBehaviour
{
    //private GameObject speedCube;

    [Tooltip("加速块提供的动力大小"), SerializeField]
    private float speedUpForce = 20f;
    [Tooltip("判断m_FPP触碰加速块各个面的精度：越小越不易触发碰撞"), SerializeField]
    private float accuracy = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        //speedCube = GameObject.FindGameObjectWithTag("AccelerationBlock");
    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 加速块功能实现：
    /// 1.加速块上表面提供up方向动力
    /// 2.加速块下表面提供down方向动力
    /// 3.加速块四个侧面提供斜向上的动力
    /// </summary>
    /// <param name="other">
    /// other是m_FPP的碰撞器
    /// </param>
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("发生碰撞：-------------------------------------------");

            ContactPoint contact = other.contacts[0];                                          //碰撞点
            Rigidbody m_FPP = other.gameObject.GetComponent<Rigidbody>();                      //m_FPP刚体
            Vector3 mCenter = other.collider.bounds.center;                                    //m_FPP中心坐标          
            Vector3 mPos = contact.point;                                                      //碰撞点坐标
            float mHeight = other.collider.bounds.size.y;                                      //m_FPP高度
            float mHorizontalX = other.collider.bounds.size.x;                                 //m_FPP水平面x
            float mHorizontalZ = other.collider.bounds.size.z;                                 //m_FPP水平面z

            Debug.Log("mCenter: " + mCenter + "mHeight: " + mHeight + "mPos: " + mPos);
            Debug.Log("mHorizontalX: " + mHorizontalX + "mHorizontalZ: " + mHorizontalZ);
            //踩到加速块
            if (Mathf.Abs(mCenter.y - mHeight / 2 - mPos.y) <= accuracy)
            {
                Debug.Log("*****踩到加速块*****");
                Vector3 runDirection = transform.InverseTransformDirection(m_FPP.velocity);    //运动速度
                runDirection = Vector3.Normalize(runDirection);
                m_FPP.AddForce(runDirection + Vector3.up * speedUpForce, ForceMode.VelocityChange);
            }
            //头顶到加速块
            if (Mathf.Abs(mPos.y - mCenter.y - mHeight / 2) <= accuracy)
            {
                Debug.Log("*****头顶到加速块*****");
                m_FPP.AddForce(Vector3.down * speedUpForce, ForceMode.VelocityChange);
            }
            //侧面碰撞
            if (Mathf.Abs(Mathf.Abs(mPos.x - mCenter.x) - mHorizontalX / 2) <= accuracy ||
               Mathf.Abs(Mathf.Abs(mPos.z - mCenter.z) - mHorizontalZ / 2) <= accuracy)
            {
                Debug.Log("*****侧面碰撞*****");
                Vector3 direction = new Vector3(m_FPP.position.x - mPos.x, 0, m_FPP.position.z - mPos.z).normalized;

                m_FPP.AddForce(direction * speedUpForce * 10, ForceMode.Impulse);
                m_FPP.AddForce(Vector3.up * speedUpForce / 2, ForceMode.VelocityChange);

            }
        }
    }
}
