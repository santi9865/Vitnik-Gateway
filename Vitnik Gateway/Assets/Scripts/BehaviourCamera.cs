using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourCamera : MonoBehaviour
{
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        positionOffset = - playerTransform.position + gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        gameObject.transform.position = playerTransform.position + positionOffset;
    }
}
