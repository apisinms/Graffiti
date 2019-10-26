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

			// 차에 치인게 플레이어라면 
			if (other.gameObject.tag.Contains("Player"))
			{
				// 플레이어 번호를 얻어서
				int playerNum = int.Parse(other.gameObject.tag[other.gameObject.tag.Length - 1].ToString());    // "Player+@"자리에 @의 숫자를 얻음

				// 죽은 상태가 아닌 플레이어라면
				if (PlayersManager.instance.actionState[playerNum - 1] != _ACTION_STATE.DEATH)
				{
					// 저 멀리 내팽개쳐주고
					Rigidbody rg = other.GetComponent<Rigidbody>();
					Vector3 force = this.transform.localRotation * Vector3.forward * kNockback;
					rg.AddForce(force);

					// 그게 만약 나라면
					if ((playerNum - 1) == GameManager.instance.myIndex)
					{
#if NETWORK
						NetworkManager.instance.ImHitByCar(force);   // 차에 치였다고 프로토콜 서버로 보낸다.
#endif
						StateManager.instance.Death(true);  // 내가 죽은상태로 전환.
						int absoluteIdx = UIManager.instance.PlayerIndexToAbsoluteIndex(playerNum - 1);
						UIManager.instance.HealthUIChanger(absoluteIdx, 0.0f);  // 차에 치이면 즉사
						UIManager.instance.SetDeadUI();     // 죽은 UI로 전환
						bl_MiniMap.Instance.DoHitEffect();
					}
				}
			}
		}

		public void ResetDistanceTravelled()
        {
            distanceTravelled = 0;
        }
    }
}