using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveTest : Singleton<MoveTest>
{
    public Queue<Transform> nextTarget = new Queue<Transform>();
    public float curSpeed;
    private void Awake()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnqueueRailPos(Transform transform)
    {
        nextTarget.Enqueue(transform);
    }
}