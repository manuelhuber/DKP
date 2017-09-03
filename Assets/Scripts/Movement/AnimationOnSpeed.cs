using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace Movement {
    [RequireComponent(typeof(Animator))]
    public class AnimationOnSpeed : MonoBehaviour {
        public NavMeshAgent Agent;
        private Animator animator;

        private void Awake() {
            animator = GetComponent<Animator>();
        }

        private void Update() {
            if (Agent == null) return;
            animator.SetFloat("Speed", Agent.velocity.magnitude);
        }
    }
}