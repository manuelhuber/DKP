using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AnimationOnSpeed : MonoBehaviour {
    public Animator Animator;
    private NavMeshAgent agent;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        Animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void Update() {
        Animator.SetFloat("Speed", agent.velocity.magnitude);
    }
}