using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasHelper : MonoBehaviour
{
    public Transform firstPosObject;
    public Transform secondPosObject;
    


    void Update()
    {
        if (firstPosObject != null && secondPosObject != null)
        {
            float distanceFromTheTarget = Vector3.Distance(firstPosObject.localPosition, secondPosObject.localPosition);
            Debug.Log(distanceFromTheTarget);
        }
    }
}
