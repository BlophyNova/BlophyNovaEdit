using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Smart
{
    [System.Serializable]
    public class PlayerGroup
    {
        public int id;
        public int reference;
        public int inventory;
    }

    public class Player : MonoBehaviour
    {
        public float walkSpeed;
        public float runSpeed;
        public float speed;
        [Space]
        public float minHealth;
        public float maxHealth;
        public float helath;
        [Space]
        public float minMP;
        public float maxMP;
        public float mp;
        [Space]
        public float minSP;
        public float maxSP;
        public float sp;
        [Space]
        public Collider col;
        public Rigidbody rgbody;
        public Animator anim;
        public NavMeshAgent nav;
        [Space]
        public PlayerGroup group;

        public bool CustomProp { get; }

        void CustomFunc()
        {

        }
    }
}
