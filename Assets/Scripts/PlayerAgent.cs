﻿using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class PlayerAgent : Agent
{
    [SerializeField]
    private float speed = 10.0f;

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private float distanceReequired = 1.5f;

    [SerializeField]
    private MeshRenderer groundMeshRenderer;

    [SerializeField]
    private Material successMaterial;
    
    [SerializeField]
    private Material failMaterial;

    [SerializeField]
    private Material defefultMaterial;

    private Rigidbody playerRigidbody;  
    private Vector3 originalPosition;
    private Vector3 originalTargetPosition;

    public override void Initialize()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        originalPosition = transform.localPosition;
        originalTargetPosition = target.transform.localPosition;
    }

    public override void OnEpisodeBegin()
    {
        transform.LookAt(target.transform);
        target.transform.localPosition = originalTargetPosition;
        transform.localPosition = originalPosition;
        transform.localPosition = new Vector3(Random.Range(-2,4), originalPosition.y, Random.Range(-4,4));
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

        var distanceFromTheTarget = Vector3.Distance(transform.localPosition, target.transform.localPosition);

        if(distanceFromTheTarget <= distanceReequired)
        {
            SetReward(1.0f);
            EndEpisode();
            StartCoroutine(SwapMaterial(successMaterial, 0.5f));
        }

        if(transform.localPosition.y < 0)
        {
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

    private IEnumerator SwapMaterial(Material mat, float time)
    {
        groundMeshRenderer.material = mat;
        yield return new WaitForSeconds(time);
        groundMeshRenderer.material = defefultMaterial;
    }
}
