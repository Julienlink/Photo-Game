using UnityEngine;
using UnityEngine.AI;

public class PNJAutonomousMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    public float roamRadius = 50f; 
    public float waitTime = 2f;    

    private Vector3 startPos;
    private float timer = 0f;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        startPos = transform.position;
        SetRandomDestination();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                SetRandomDestination();
                timer = 0f;
            }
        }
    }

    void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += startPos;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}