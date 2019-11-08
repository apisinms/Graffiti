using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

namespace PathCreation.Examples
{

    public class PathSpawner : MonoBehaviour
    {

        public PathCreator[] pathPrefab;
        public PathFollower[] followerPrefab;
        public Transform[] spawnPoints;
        public float delay;
        private PathCreator[] path;

        public List<GameObject> activePrefabs;   // 켜진 프리팹 목록

        void Start()
        {
            path = new PathCreator[spawnPoints.Length];

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                path[i] = Instantiate(pathPrefab[i], spawnPoints[i].position, spawnPoints[i].rotation);
                path[i].transform.parent = spawnPoints[i];
            }
        }

        public IEnumerator Cor_SpawnPrefabs()
        {
            while (true)
            {
                int randome = Random.Range(0, spawnPoints.Length);
                //var follower = Instantiate(followerPrefab[Random.Range(0, followerPrefab.Length)]);

                var follower = ObjectPool.Instance.PopFromPool
                   (Random.Range(0, followerPrefab.Length));

                follower.GetComponent<PathFollower>().pathCreator = path[randome];
                follower.transform.position = path[randome].transform.position;

                follower.SetActive(true);

                yield return YieldInstructionCache.WaitForSeconds(delay);
            }
        }

        public void SpawnPrefabs()
        {
            int random = Random.Range(0, spawnPoints.Length);

            var follower = ObjectPool.Instance.PopFromPool
               (Random.Range(0, followerPrefab.Length));

            follower.GetComponent<PathFollower>().pathCreator = path[random];
            follower.transform.position = path[random].transform.position;

            follower.SetActive(true);

            activePrefabs.Add(follower);   // 활성화된 목록에 넣어줌
        }

        public void TurnOffAllPrefabs()
        {
            // 켜진 프리팹 목록을 전부 풀로 돌려보낸다.
            for (int i = 0; i < activePrefabs.Count; i++)
            {
                var pathFollower = activePrefabs[i].GetComponent<PathFollower>();
                pathFollower.ResetDistanceTravelled();
                ObjectPool.Instance.PushToPool(pathFollower.index, pathFollower.gameObject);
            }
        }
    }
}