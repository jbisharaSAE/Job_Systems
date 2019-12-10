//using Unity.Jobs;
//using UnityEngine.Jobs;
//using UnityEngine;
//using Unity.Burst;
//using Unity.Mathematics;
//using System.Collections;
//using System;
//using Unity.Entities;

//[BurstCompile]
//public struct JobSystem : IJobParallelForTransform
//{
    
//    public float maxSpeed;
//    public float moveX;
//    public float moveY;

//    public float top;
//    public float bottom;
//    public float left;
//    public float right;
//    public float deltaTime;

//    public void Execute(int i, TransformAccess transform)
//    {
//        Vector3 pos = transform.position;
//        float speed = Random.Range(minSpeed, maxSpeed);

//        moveX = Random.Range(-1f, 1f);
//        moveY = Random.Range(-1f, 1f);

//        if(pos.x <= left)
//        {
//            // left wall
//            moveX = -moveX;
//        }
//        if(pos.x >= right)
//        {
//            // right wall
//            moveX = -moveX;
//        }
//        if(pos.y <= bottom)
//        {
//            // bottom wall
//            moveY = -moveY;
//        }
//        if(pos.y >= top)
//        {
//            // top wall
//            moveY = -moveY;
//        }

//        pos += new Vector3(moveX, moveY, 0f) * (speed * deltaTime);
//    }
//}