using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OppaPathing : MonoBehaviour
{
    [SerializeField] float MoveSpeed = 10f;

    private float screenRightBound;
    private float screenLeftBound;
    private float screenTopBound;
    private float screenBottomBound;

    private float targetX, targetY;
    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        screenLeftBound = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        screenRightBound = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        screenBottomBound = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        screenTopBound = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        
        ResetTargetPosition();
    }

    private void ResetTargetPosition()
    {
        targetX = Random.Range(screenLeftBound, screenRightBound);
        targetY = Random.Range(screenBottomBound, screenTopBound);
        targetPosition = new Vector3(targetX, targetY, 0);

    }

    // Update is called once per frame
    void Update()
    {
        var movementThisFrame = MoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementThisFrame);
        MoveFollowingRandomPath();
    }

    private void MoveFollowingRandomPath()
    {
        if (transform.position == targetPosition)
        {
            ResetTargetPosition();   
        }
    }
}
