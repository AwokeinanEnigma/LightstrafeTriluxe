#region

using UnityEngine;

#endregion

public class PlayAudio : StateMachineBehaviour
{
    public float delay;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<AudioSource>().PlayDelayed(delay);
    }
}