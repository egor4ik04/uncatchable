using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private GameObject target;

    private bool isIgnoringFreeSpace;

    [SerializeField] private float cameraSpeed = 2f;
    [SerializeField] private float wiggleDistance = 0.2f;
    [SerializeField] private float wiggleSpeed = 0.04f;
    [SerializeField] private Vector2 wiggleAddPosition;
    [SerializeField] private Vector2 realPositionWithoutWiggle;
    [SerializeField] private Vector2 wiggleDestination;
    bool isWiggling;

    private void Start()
    {
        if (target == null)
            SetTarget();
        wiggleAddPosition = Vector2.zero;
        realPositionWithoutWiggle = transform.position;
    }

    private void Update()
    {
        if (!isWiggling)
        {
            isWiggling = true;
            wiggleDestination = new Vector2(
                Random.Range(-wiggleDistance, wiggleDistance), 
                Random.Range(-wiggleDistance, wiggleDistance));
        }
        else
        {
            wiggleAddPosition = Vector2.MoveTowards(wiggleAddPosition, wiggleDestination, wiggleSpeed * Time.deltaTime);
            if ((wiggleAddPosition - wiggleDestination).magnitude < 0.01f)
                isWiggling = false;
        }

        CameraMoveAroundTarget();
    }

    public void SetTarget(GameObject target = null)
    {
        if (target != null)
            this.target = target;
        else
            this.target = GameObject.Find("CameraPrefab")?.transform.Find("CameraTarget").gameObject ?? Camera.main.gameObject;
    }

    private void CameraMoveAroundTarget()
    {
        Vector2 target = this.target.transform.position;
        Vector2 move = Vector2.Lerp(realPositionWithoutWiggle, target, cameraSpeed * Time.deltaTime);
        realPositionWithoutWiggle = move;
        if ((target - realPositionWithoutWiggle).magnitude < 0.01f)
            realPositionWithoutWiggle = target;
        var sumPos = realPositionWithoutWiggle + wiggleAddPosition;
        transform.position = new Vector3(sumPos.x, sumPos.y, transform.position.z);
    }
}
