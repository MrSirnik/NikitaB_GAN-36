using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAI : MonoBehaviour
{
    [HideInInspector] public GameObject targetItem; // Целевой предмет для сбора

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Убедимся, что на старте агент остановлен
        if (agent != null) agent.isStopped = true;
    }

    public void StopMoving()
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    // Корутина для возврата в Idle после сбора предмета
    public IEnumerator ReturnToIdleAfterDelay(Animator anim, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (anim != null)
        {
            anim.SetTrigger("StartSearch");
        }
    }
}