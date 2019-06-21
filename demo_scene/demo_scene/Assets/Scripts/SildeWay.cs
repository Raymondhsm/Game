using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SildeWay : MonoBehaviour
{
    [Tooltip("Destination"), SerializeField]
    private GameObject nextObject;
   //[Tooltip("How long to the Destination"), SerializeField]
    private float time=20;
    private float i=0;
    private float correction;
    private Collider _collider;
    private Vector3 displacement;
    // Start is called before the first frame update
    void Start()
    {
        i = time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (i == time)
        {
            if (_collider != null)
                _collider.GetComponent<Rigidbody>().isKinematic = false;
            _collider = null;
            //Debug.Log("time" + time);
            return;
        }
        _collider.GetComponent<Rigidbody>().isKinematic = true;
        //Debug.Log("displacement.x="+displacement.x+"  time " +i);
        if(_collider !=null)
          _collider.GetComponent<Transform>().Translate(displacement,Space.World);

        i++;

    }

    void OnTriggerEnter(Collider collider)
    {
        var position = GetComponent<Transform>().position;
        var destination = nextObject.transform.position;
        displacement = destination - position;
        displacement = displacement / time;
       // displacement.y = displacement.y / time;
       // displacement.z = displacement.z / time;
        // Debug.Log(destination.x );
        // Debug.Log(destination.y );
        // Debug.Log(destination.z );
        _collider = collider;
        i = 0;
        //collider.GetComponent<Transform>().Translate(displacement);
    }
}
