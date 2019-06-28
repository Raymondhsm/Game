using System.Collections;
using UnityEngine;
 
public class ShakeCamera : MonoBehaviour {
    private Transform ThisTransform = null;
    public float ShakeTime = 10000.0f;
    public float ShakeAmount = 3.0f;
    public float ShakeSpeed = 2.0f;
    // Use this for initialization
    void Start() {
        ThisTransform = GetComponent<Transform>();
        
        Debug.Log("sss");
        //StartCoroutine(Shake());
        Debug.Log("sss");
    }

    void Update()
    {
        RaycastHit rh = Capsul
    }
 
    public IEnumerator Shake() {
        Debug.Log("aa");
        Vector3 OrigPosition = ThisTransform.localPosition;
        float ElapsedTime = 0.0f;
        while (ElapsedTime < ShakeTime) {
            Vector3 RandomPoint = OrigPosition + Random.insideUnitSphere * ShakeAmount;
            ThisTransform.localPosition = Vector3.Lerp(ThisTransform.localPosition,RandomPoint,Time.deltaTime*ShakeSpeed);
            yield return null;
            ElapsedTime += Time.deltaTime;
        }
        ThisTransform.localPosition = OrigPosition;
    }
}
