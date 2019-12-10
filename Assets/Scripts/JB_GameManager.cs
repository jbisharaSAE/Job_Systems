using Unity.Jobs;
using UnityEngine.Jobs;
using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;
using System.Collections;
using System;
using Unity.Entities;
using System.Collections.Generic;
using Unity.Collections;


public class JB_GameManager : MonoBehaviour
{
    private List<GameObject> zombies = new List<GameObject>();

    [Header("Border numbers")]
    [SerializeField] private float topBounds;
    [SerializeField] private float bottomBounds;
    [SerializeField] private float leftBounds;
    [SerializeField] private float rightBounds;

    [Header("Min/Max speed of obj")]
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;


    [Header("Prefab to spawn")]
    [SerializeField] private Transform humanPrefab;

    [Header("Number of prefabs to spawn")]
    [SerializeField] private int maxSpawnAmount;

    [Header("Toggle Job System")]
    [SerializeField] private bool useJob;

    [Header("Inner Loop Batch Count")]
    [SerializeField] private int innerLoopBatchCount;

    private List<Human> humanList;

    NativeArray<float3> positions;
    NativeArray<float> directionsX;
    NativeArray<float> directionsY;
    NativeArray<float> speeds;

    
    //JobSystem moveJob;
    JobHandle moveHandle;

    public class Human
    {
        public Transform transform;
        public float dirX;
        public float dirY;
        public float speed;
        
    }


    private void OnDisable()
    {
        moveHandle.Complete();
        positions.Dispose();
        directionsX.Dispose();
        directionsY.Dispose();
        speeds.Dispose();
    }

    void Start()
    {
        humanList = new List<Human>();

        

        for (int i = 0; i < maxSpawnAmount; i++)
        {
            Transform human = Instantiate(humanPrefab, Vector3.zero, Quaternion.identity);

            humanList.Add(new Human
            {
                transform = human.transform,
                dirX = UnityEngine.Random.Range(-1f, 1f),
                dirY = UnityEngine.Random.Range(-1f, 1f),
                speed = UnityEngine.Random.Range(minSpeed, maxSpeed)

            });

    

        }

    }

    void Update()
    {
        float startTime = Time.realtimeSinceStartup;
        //moveX = Random.Range(-1f, 1f);
        //moveY = Random.Range(-1f, 1f);

        if (useJob)
        {
            positions = new NativeArray<float3>(humanList.Count, Allocator.TempJob);
            directionsX = new NativeArray<float>(humanList.Count, Allocator.TempJob);
            directionsY = new NativeArray<float>(humanList.Count, Allocator.TempJob);
            speeds = new NativeArray<float>(humanList.Count, Allocator.TempJob);


            for (int i = 0; i < humanList.Count; ++i)
            {
                positions[i] = humanList[i].transform.position;
                directionsX[i] = humanList[i].dirX;
                directionsY[i] = humanList[i].dirY;
                speeds[i] = humanList[i].speed;
            }
            JobSystem moveJob = new JobSystem {
                deltaTime = Time.deltaTime,
                topBounds = topBounds,
                rightBounds = rightBounds,
                leftBounds = leftBounds,
                bottomBounds= bottomBounds,
                positions = positions,
                directionsX = directionsX,
                directionsY = directionsY,
                speeds = speeds

            };

            moveHandle = moveJob.Schedule(humanList.Count, innerLoopBatchCount);
            moveHandle.Complete();

            for(int i = 0; i< humanList.Count; ++i)
            {
                humanList[i].transform.position = positions[i];
                humanList[i].dirX = directionsX[i];
                humanList[i].dirY = directionsY[i];
                humanList[i].speed = speeds[i];
            }

            positions.Dispose();
            directionsX.Dispose();
            directionsY.Dispose();
            speeds.Dispose();

            //    // use job system
            //    moveHandle.Complete();

            //    moveJob = new JobSystem()
            //    {
            //        deltaTime = Time.deltaTime,
            //        top = topBounds,
            //        left = leftBounds,
            //        right = rightBounds,
            //        bottom = bottomBounds,
            //        minSpeed = minSpeed,
            //        maxSpeed = maxSpeed,
            //        moveX = moveX,
            //        moveY = moveY

            //    };

            //    moveHandle = moveJob.Schedule(transforms);

            //    JobHandle.ScheduleBatchedJobs();

        }
        else
        {
            foreach (Human human in humanList)
            {
                human.transform.position += new Vector3(human.dirX, human.dirY, 0f) * (human.speed * Time.deltaTime);

                if (human.transform.position.x <= leftBounds)
                {
                    // left wall
                    human.dirX = -human.dirX;
                }
                if (human.transform.position.x >= rightBounds)
                {
                    // right wall
                    human.dirX = -human.dirX;
                }
                if (human.transform.position.y <= bottomBounds)
                {
                    // bottom wall
                    human.dirY = -human.dirY;
                }
                if (human.transform.position.y >= topBounds)
                {
                    // top wall
                    human.dirY = -human.dirY;
                }
            }

        }

        Debug.Log(((Time.realtimeSinceStartup - startTime) * 1000f) + "ms");
    }
}

[BurstCompile]
public struct JobSystem : IJobParallelFor
{

    public NativeArray<float3> positions;
    public NativeArray<float> directionsX;
    public NativeArray<float> directionsY;
    public NativeArray<float> speeds;
    

    public float topBounds;
    public float bottomBounds;
    public float leftBounds;
    public float rightBounds;
    public float deltaTime;

    public void Execute(int i)
    {
        positions[i] += new float3(directionsX[i], directionsY[i], 0f) * (speeds[i] * deltaTime);

        if (positions[i].x <= leftBounds)
        {
            // left wall
            directionsX[i] = -directionsX[i];
        }
        if (positions[i].x >= rightBounds)
        {
            // right wall
            directionsX[i] = -directionsX[i];
        }
        if (positions[i].y <= bottomBounds)
        {
            // bottom wall
            directionsY[i] = -directionsY[i];
        }
        if (positions[i].y >= topBounds)
        {
            // top wall
            directionsY[i] = -directionsY[i];
        }
    }
}