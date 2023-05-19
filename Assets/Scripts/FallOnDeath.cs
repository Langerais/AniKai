using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallOnDeath : StateMachineBehaviour
{
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //Destroy(animator.gameObject, stateInfo.length);
        animator.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1f;
    }
}
