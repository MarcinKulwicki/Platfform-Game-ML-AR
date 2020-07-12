using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class PlayerAgentCheckingDistance : BaseAgent
{
    [SerializeField]
    private float speed = 10.0f;

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private float distanceReequired = 1.5f;

    private Rigidbody playerRigidbody;  
    private Vector3 originalPosition;
    private Vector3 originalTargetPosition;
    private float maxDistance;
    private float stepDistance;

    public override void Initialize()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        originalPosition = transform.localPosition;
        originalTargetPosition = target.transform.localPosition;
    }

    public override void OnEpisodeBegin()
    {
        target.transform.localPosition = new Vector3(Random.Range(-24,-20),originalTargetPosition.y, Random.Range(-4,4));
        transform.LookAt(target.transform);
        transform.localPosition = originalPosition;
        transform.localPosition = new Vector3(Random.Range(-2,4), originalPosition.y, Random.Range(-4,4));
        maxDistance = Vector3.Distance(transform.localPosition, target.transform.localPosition);
        stepDistance = maxDistance;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //3 observation - x,y,z
        sensor.AddObservation(transform.localPosition);
        //3 observation - x,y,z
        sensor.AddObservation(target.transform.localPosition);
        //1 observation
        sensor.AddObservation(playerRigidbody.velocity.x);
        //1 observation
        sensor.AddObservation(playerRigidbody.velocity.z);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        var vectorForce = new Vector3();
        vectorForce.x = vectorAction[0];
        vectorForce.z = vectorAction[1];

        playerRigidbody.AddForce(vectorForce * speed);

        float distanceFromTheTarget = Vector3.Distance(transform.localPosition, target.transform.localPosition);

        // if(stepDistance + maxDistance * 0.02f < distanceFromTheTarget){
        //     EndEpisode();
        //     //go back and punish Agent
        //     StartCoroutine(SwapMaterial(failMaterial, 0.5f));
        // }
        // stepDistance = distanceFromTheTarget;

        if(distanceFromTheTarget <= distanceReequired)
        {
            SetReward(1.0f);
            EndEpisode();
            StartCoroutine(SwapMaterial(successMaterial, 0.5f));
        }

        if(transform.localPosition.y < 0)
        {
            if(distanceFromTheTarget < maxDistance){
                SetReward((maxDistance / distanceFromTheTarget) * 0.001f);
            }
            EndEpisode();
            //go back and punish Agent
            StartCoroutine(SwapMaterial(failMaterial, 0.5f));
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal"); // x
        actionsOut[1] = Input.GetAxis("Vertical"); // z

    }
}
