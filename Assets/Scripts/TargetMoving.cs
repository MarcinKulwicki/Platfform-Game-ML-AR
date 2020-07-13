using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMoving : MonoBehaviour
{
    [SerializeField]
    private AgentAvoidance agent = null;

    [SerializeField]
    [Range(0.1f, 5.0f)]
    private float minSpeed = 5.0f;
    
    [SerializeField]
    [Range(5.0f, 100.0f)]
    private float maxSpeed = 50.0f;
    
    public float speed = 0;

    [SerializeField]
    private float maxDistance = 10.0f;

    private Vector3 originalPosition;

    private void Awake() {
        originalPosition = transform.localPosition;
        speed = Random.Range(minSpeed, maxSpeed);
    }

    private void Update() {
        //if we are beyond the max distance restart the position
        if(transform.localPosition.x >= originalPosition.x + maxDistance)
        {
            transform.localPosition = originalPosition;
        }
        else
        {
            //move towards max distance
            transform.localPosition = new Vector3(
                transform.localPosition.x + (Time.deltaTime * speed), 
                transform.localPosition.y, 
                transform.localPosition.z
            );
        }
    }

    public void ResetTarget()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        transform.localPosition = originalPosition;
        transform.localRotation = Quaternion.identity;
    }

    private void OnCollisionEnter(Collision collision) 
    {
        if (collision.transform.tag.ToLower() == "player")
        {
            Debug.Log("Point taken away");
            agent.TakeAwayPoints();
        }
        else if (collision.transform.tag.ToLower() == "wall")
        {
            Debug.Log("Points gained");
            agent.GivePoints();
        }
    }
}
