using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public class CollectState : StateMachineBehaviour
{
    private NavMeshAgent agent;
    private GameObject targetItem;
    private float collectDistance = 1.5f;
    private bool isCollecting = false;
    private CharacterAI ai;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        ai = animator.GetComponent<CharacterAI>();

        if (ai != null)
        {
            targetItem = ai.targetItem;
        }

        if (agent != null && targetItem != null)
        {
            agent.isStopped = false;
            agent.SetDestination(targetItem.transform.position);
        }

        isCollecting = false;
        Debug.Log("Entered Collect State");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (targetItem == null)
        {
            // Если предмет исчез, возвращаемся в Idle
            animator.SetTrigger("StartSearch");
            return;
        }

        if (agent == null) return;

        // Проверяем, достиг ли агент цели
        if (!agent.pathPending && agent.remainingDistance <= collectDistance)
        {
            if (!isCollecting)
            {
                isCollecting = true;
                agent.isStopped = true;

                GameObject itemToDestroy = targetItem;
                ai.targetItem = null;

                Debug.Log("Collecting item: " + itemToDestroy.name);

                Object.Destroy(itemToDestroy);

                if (ai != null)
                {
                    ai.StartCoroutine(ai.ReturnToIdleAfterDelay(animator, 0.5f));
                }
                else
                {
                    animator.SetTrigger("StartSearch");
                }
            }
        }
        else if (targetItem != null)
        {
            // Обновляем цель на случай, если предмет двигается
            agent.SetDestination(targetItem.transform.position);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent != null)
        {
            agent.isStopped = true;
        }
        Debug.Log("Exited Collect State");
    }
}