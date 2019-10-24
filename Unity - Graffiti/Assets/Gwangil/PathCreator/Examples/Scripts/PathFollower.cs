using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public int index;
        public float kNockback = 1000.0f;
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        float distanceTravelled;

        void Start()
        {
            if (pathCreator != null)
            {
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        void Update()
        {
            if (pathCreator != null)
            {
                distanceTravelled += speed * Time.deltaTime;
                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
            }
        }

        void OnPathChanged()
        {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("EndPoint"))
            {
                // 오브젝트 풀 반환
                // distanceTravelled 는 꼭 0으로 초기화 해줘야한다.(현재 얼만큼의 거리를 갔는지 저장하는 변수)
                distanceTravelled = 0;
                ObjectPool.Instance.PushToPool(index, gameObject);
            }
            if (other.gameObject.CompareTag("Player1") || other.gameObject.CompareTag("Player2") ||
                other.gameObject.CompareTag("Player3") || other.gameObject.CompareTag("Player4"))
            {
                /*Rigidbody rg = other.GetComponent<Rigidbody>();
                rg.AddForce(this.transform.localRotation * Vector3.forward * kNockback);
                bl_MiniMap.Instance.DoHitEffect();*/
            }
        }
        public void ResetDistanceTravelled()
        {
            distanceTravelled = 0;
        }
    }
}