using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.UI;

public class AgentAvoidance : BaseAgent
{
    [SerializeField]
    private float speed = 10.0f;

    [SerializeField]
    private Vector3 idlePosition = Vector3.zero;

    [SerializeField]
    private Vector3 leftPosition = Vector3.zero;

    [SerializeField]
    private Vector3 rightPosition = Vector3.zero;

    [SerializeField]
    private Text rewardValue = null;

    [SerializeField]
    private Text episodesValue = null;

    [SerializeField]
    private Text stepValue = null;

    private TargetMoving targetMoving = null;

    private float overallReward = 0;
    
    private float overallSteps = 0;

    private Vector3 moveTo = Vector3.zero;

    private Vector3 prevPosition = Vector3.zero;

    private int punishCounter;

    private void Awake() {
        targetMoving = transform.parent.GetComponentInChildren<TargetMoving>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = idlePosition;
        moveTo = prevPosition = idlePosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetMoving.transform.localPosition);
        sensor.AddObservation(targetMoving.speed);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        prevPosition = moveTo;
        int direction = Mathf.FloorToInt(vectorAction[0]);
        moveTo = idlePosition;

        switch(direction)
        {
            case 0:
                moveTo = idlePosition;
                break;
            case 1:
                moveTo = leftPosition;
                break;
            case 2:
                moveTo = rightPosition;
                break;
        }

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, moveTo, Time.fixedDeltaTime * speed);

        if (prevPosition == moveTo)
        {
            punishCounter++;
        }

        if (punishCounter > 2.0f)
        {
            AddReward(-0.01f);
            punishCounter = 0;
        }
    }

    public void TakeAwayPoints()
    {
        AddReward(-0.01f);
        targetMoving.ResetTarget();

        UpadteStats();
        
        EndEpisode();
        StartCoroutine(SwapMaterial(failMaterial, 0.5f));
    }

    private void UpadteStats(){
        overallReward += this.GetCumulativeReward();
        overallSteps += this.StepCount;
        rewardValue.text = $"{this.overallReward.ToString("F2")}";
        episodesValue.text = $"{this.CompletedEpisodes}";
        stepValue.text = $"{this.overallSteps}";
    }

    public void GivePoints()
    {
        AddReward(1.0f);
        targetMoving.ResetTarget();

        UpadteStats();

        EndEpisode();
        StartCoroutine(SwapMaterial(successMaterial, 0.5f));
    }

    public override void Heuristic(float[] actionsOut)
    {
        //idle
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            actionsOut[0] = 0;
        }

        //left
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            actionsOut[0] = 1;
        }

        //right
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            actionsOut[0] = 2;
        }
    }
}
