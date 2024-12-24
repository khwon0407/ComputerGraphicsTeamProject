using UnityEngine;
using UnityEngine.AI;

public class FollowBall : MonoBehaviour
{
    public Transform ball; // 공의 Transform

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (ball != null)
        {
            // 공의 위치를 NavMesh Agent의 목표로 설정
            agent.SetDestination(ball.position);
        }
    }
}

