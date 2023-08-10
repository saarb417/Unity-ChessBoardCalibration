using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public Transform chessboardCenter;
    public float movementSpeed = 1000.0f;
    public float rotationSpeed = 2.0f;
    public float waitTime = 0.01f;
    public float maxDistanceFromCenter = 5.0f;

    private Vector3 targetPosition;
    private Quaternion initialRotation;
    private bool isMoving = false;

    private float yawAddition = 0f;
    private float pitchAddition = 0f;
    private Vector3 desiredRotation;

    void Start()
    {
        initialRotation = transform.rotation;
        targetPosition = transform.position;
        StartCoroutine(MoveRandomly());
    }

    void Update()
    {
        if (isMoving)
        {
            float step = movementSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
                StartCoroutine(MoveRandomly());
            }
        }

        // Rotate towards the chessboard center during movement
        if (isMoving)
        {
            Vector3 directionToChessboard = chessboardCenter.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToChessboard, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        transform.LookAt(chessboardCenter);
    }

    IEnumerator MoveRandomly()
    {

        yawAddition = Random.Range(-30f,30f);
        pitchAddition = Random.Range(-30f,30f);

        yield return new WaitForSeconds(waitTime);

        Vector2 randomOffset = Random.insideUnitCircle * maxDistanceFromCenter;

        while (chessboardCenter.position.y + randomOffset.y < chessboardCenter.position.y)
        {
        randomOffset = Random.insideUnitCircle * maxDistanceFromCenter;
        } 
        Vector3 randomPosition = new Vector3(
            chessboardCenter.position.x + randomOffset.x,
            chessboardCenter.position.y + Random.Range(-1f * maxDistanceFromCenter, maxDistanceFromCenter),
            chessboardCenter.position.z + randomOffset.y
        );

        targetPosition = randomPosition;
        isMoving = true;
    }
}




 