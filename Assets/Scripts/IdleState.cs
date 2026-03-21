using UnityEngine;
using UnityEngine.TextCore.Text;

public class IdleState : StateMachineBehaviour
{
    private float timer;
    private Animator animator;
    private CharacterAI characterAI;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        characterAI = animator.GetComponent<CharacterAI>();
        if (characterAI != null)
        {
            characterAI.StopMoving();
        }

        timer = 0f;
        this.animator = animator;
        Debug.Log("Entered Idle State");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        if (timer >= 5f)
        {
            animator.SetTrigger("StartSearch");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Exited Idle State");
    }
}