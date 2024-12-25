using UnityEngine;
using UnityEngine.AI;

public class TriggerEnemyController : MonoBehaviour
{
    public Transform ball;
    private NavMeshAgent agent;
    private bool isChasing = false;

    [Header("Chase Settings")]
    [SerializeField] private float updateRate = 0.1f;
    private float nextUpdateTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Agent is not on NavMesh!");
        }
    }

    void Update()
    {
        if (isChasing && ball != null && Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateRate;
            
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(ball.position);
            }
        }
    }

    public void StartChasing()
    {
        if (!isChasing)
        {
            isChasing = true;
            agent.isStopped = false;
            Debug.Log("Enemy started chasing!");
        }
    }
}
