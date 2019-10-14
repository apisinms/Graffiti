using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

namespace PathCreation.Examples {

    public class PathSpawner : MonoBehaviour {

        public PathCreator[] pathPrefab;
        public PathFollower[] followerPrefab;
        public Transform[] spawnPoints;
        public float delay;
        private PathCreator[] path;

        void Start () {
            path = new PathCreator[spawnPoints.Length];

            for(int i = 0; i < spawnPoints.Length; i++)
            { 
                path[i] = Instantiate(pathPrefab[i], spawnPoints[i].position, spawnPoints[i].rotation);
                path[i].transform.parent = spawnPoints[i];
            }

            //StartCoroutine(SpawnPrefabs());
        }

        public IEnumerator SpawnPrefabs()
        {
            while(true)
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
    }
}