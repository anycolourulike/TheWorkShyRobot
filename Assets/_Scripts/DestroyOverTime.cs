using UnityEngine;
using System.Collections;

namespace Rambler
{
    public class DestroyOverTime : MonoBehaviour
    {
        public float timeToDestroy = 1.5f;

        void Start()
        {
            Destroy(gameObject, timeToDestroy);
        }
    }
}
