using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerMoveByCameraPosition : MonoBehaviour
{
    [SerializeField] private float posModifier;
    private Transform cameraTr;

    private void Start()
    {
        cameraTr = Camera.main.transform;
    }

    private void Update()
    {
        transform.position = new Vector3(
            cameraTr.position.x * posModifier,
            cameraTr.position.y * posModifier,
            transform.position.z);
    }
}
