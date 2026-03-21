using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public class SearchState : StateMachineBehaviour
{
    private NavMeshAgent agent;
    private float wanderRadius = 10f;
    private float wanderTimer;
    private float searchInterval = 3f; // Интервал поиска предметов

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = false;
            agent.speed = 2f; // Скорость поиска
        }
        wanderTimer = 0f;
        Debug.Log("Entered Search State");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent == null) return;

        // 1. Логика случайного блуждания
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= searchInterval)
        {
            SetRandomDestination();
            wanderTimer = 0f;
        }

        // Находим все объекты с тегом "Collectable" в радиусе 5
        Collider[] hitColliders = Physics.OverlapSphere(animator.transform.position, 5f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Collectable"))
            {
                CharacterAI ai = animator.GetComponent<CharacterAI>();
                if (ai != null)
                {
                    ai.targetItem = hitCollider.gameObject;
                }
                animator.SetTrigger("StartCollect");
                return;
            }
        }
    }

    private void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += agent.transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Exited Search State");
    }
}