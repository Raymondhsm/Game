using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SildeWayGo : MonoBehaviour
{
    //[Tooltip("Destination"), SerializeField]
    private GameObject m_nextObject;
    [Tooltip("over distance"), SerializeField]
    private float overDistance = 5;
    [Tooltip("How long to the Destination"), SerializeField]
    private float m_Time = 20;
    private float m_i = 0;
    private float m_Correction;
    private Collider m_Collider;
    private Vector3 m_Displacement;
    private GameObject m_Parent;
    public Transform[] SildeWayDirection;
    private Vector3 m_Speed;
    private Vector3 m_NormalVector;
    private float m_j = 0;
    private Vector3 m_SpeedZero;
    // Start is called before the first frame update
    void Start()
    {
        m_i = m_Time+overDistance;
        m_Speed = new Vector3(0,0,0);
        m_SpeedZero=new Vector3(0,0,0);
        m_Parent = this.gameObject.transform.parent.gameObject;
        SildeWayDirection = m_Parent.GetComponentsInChildren<Transform>();
        m_NormalVector = GetComponent<Transform>().right;
        foreach (Transform child in SildeWayDirection)
        {
            if (child == GetComponent<Transform>())
                break;
            else
                m_j++;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (m_i == m_Time+overDistance)
        {
            if (m_Collider != null)
                m_Collider.GetComponent<Rigidbody>().isKinematic = false;
            m_Collider = null;
            return;
        }
       
        if(m_Collider != null)
            m_Collider.GetComponent<Rigidbody>().isKinematic = true;
        if (m_Collider != null)
            m_Collider.GetComponent<Transform>().Translate(m_Displacement, Space.World);

        m_i++;
    }

    void OnTriggerEnter(Collider collider)
    {
        m_Speed = collider.GetComponent<Rigidbody>().velocity;
        if (m_j == SildeWayDirection.Length-1 && m_Speed != m_SpeedZero)
        {
            m_Parent.GetComponent<SildeWay>().enabled = false;
        }
        if (m_j == 1 && m_Speed != m_SpeedZero)
        {
            m_Parent.GetComponent<SildeWay>().enabled = true;
        }
        if (m_Parent.GetComponent<SildeWay>().enabled)
        {
            if (m_j >= SildeWayDirection.Length - 1)
            {
                m_i = m_Time;
                return;
            }
            else
                m_nextObject = SildeWayDirection[(int)m_j + 1].gameObject;
        }
        else
        {
            if (m_j <= 1)
            {
                m_i = m_Time;
                return;
            }
            else
                m_nextObject = SildeWayDirection[(int)m_j - 1].gameObject;
        }
        var position = GetComponent<Transform>().position;
        var destination = m_nextObject.transform.position;
        m_Displacement = destination - position;
        m_Displacement = m_Displacement.normalized * 0.9f + m_Displacement;
        m_Displacement.x = m_Displacement.x / m_Time;
        m_Displacement.y = m_Displacement.y / m_Time;
        m_Displacement.z = m_Displacement.z / m_Time;
        m_Collider = collider;
        m_i = 0;
        //collider.GetComponent<Transform>().Translate(displacement);
    }
}
