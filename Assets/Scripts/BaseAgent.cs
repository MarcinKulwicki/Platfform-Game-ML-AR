using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class BaseAgent : Agent
{
    [SerializeField]
    protected MeshRenderer groundMeshRenderer;

    [SerializeField]
    protected Material successMaterial;
    
    [SerializeField]
    protected Material failMaterial;

    [SerializeField]
    protected Material defefultMaterial;

    protected IEnumerator SwapMaterial(Material mat, float time)
    {
        groundMeshRenderer.material = mat;
        yield return new WaitForSeconds(time);
        groundMeshRenderer.material = defefultMaterial;
    }
}
