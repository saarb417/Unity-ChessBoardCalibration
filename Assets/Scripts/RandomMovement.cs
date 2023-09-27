using System.Collections;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public Transform chessboardCenter;

    [Header("Speed Limitations")]
    public float movementSpeed = 1000000000000.0f;
    public float rotationSpeed = 1000000000000.0f;

    [Header("Movement Area Limitations")]
    public float maxDistanceFromCenter = 3.25f;
    public float minDistanceFromCenter = 2.0f;
    public float maxAngle = 25.0f;
    public float waitTime = 0.0f;

    [Header("Tilt Limitations")]
    public float roll = 5.0f;
    public float yaw = 10f;
    public float pitch = 10f;
    private float _roll, _yaw, _pitch;

    private Vector3 targetPosition;
    private Vector3 previous_postion = Vector3.zero;
    private bool isMoving = false;

    void Start()
    {
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

        if (previous_postion.x != transform.position.x || previous_postion.y != transform.position.y || previous_postion.z != transform.position.z)
        {
            previous_postion = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            transform.LookAt(chessboardCenter);
            transform.Rotate(_roll, _yaw, _pitch);
        }
    }

    private Vector3 InsideUnitCircle3D()
    {

        float the_rest;
        float x_squared, y_squared, z_squared;

        x_squared = Random.Range(0f, 1f);
        the_rest = 1 - x_squared;
        y_squared = Random.Range(0f, 1f);
        while (y_squared > the_rest)
            y_squared = Random.Range(0f, 1f);
        z_squared = the_rest - y_squared;

        Vector3 v = new Vector3(
        Random.Range(-1f, 1f) < 0 ? -1 * Mathf.Sqrt(x_squared) : Mathf.Sqrt(x_squared),
        Random.Range(-1f, 1f) < 0 ? -1 * Mathf.Sqrt(y_squared) : Mathf.Sqrt(y_squared),
        Mathf.Sqrt(z_squared));

        if (AngleIsLower(v))
            return v;
        else
            return InsideUnitCircle3D();

    }

    private bool AngleIsLower(Vector3 randomVector)
    {
        // Calculate the dot product between the vector and the z-axis
        float dotProduct = Vector3.Dot(randomVector.normalized, Vector3.forward);

        // Calculate the angle in radians
        float angleRadians = Mathf.Acos(dotProduct);

        // Convert radians to degrees
        float angleDegrees = angleRadians * Mathf.Rad2Deg;

        if (maxAngle < 20f)
            maxAngle = 20f;

        return angleDegrees < maxAngle;

    }

    IEnumerator MoveRandomly()
    {
        // yield return new WaitForSeconds(waitTime);
        yield return new WaitForSeconds(0);

        float dist = Random.Range(minDistanceFromCenter, maxDistanceFromCenter);
        Vector3 randomVector = dist * InsideUnitCircle3D();

        targetPosition = new Vector3(chessboardCenter.position.x + randomVector.x,
        chessboardCenter.position.y + randomVector.y,
        chessboardCenter.position.z + randomVector.z);
        isMoving = true;


        RandomRotationConfiguration(dist);


    }

    private void RandomRotationConfiguration(float dist)
    {
        _roll = Random.Range(-roll, roll);
        _yaw = Random.Range(-yaw, yaw);
        _pitch = Random.Range(-pitch, pitch);

        float diffrence = maxDistanceFromCenter - minDistanceFromCenter;
        float coeffient = 1 + (dist - minDistanceFromCenter) * (diffrence - 1)/Mathf.Pow(diffrence,diffrence);

        if (dist > (maxDistanceFromCenter + minDistanceFromCenter) / 2)
        {
            _roll *= coeffient;
            _yaw *= coeffient;
            _pitch *= coeffient;
        }
        else
        {
            _roll /= coeffient;
            _yaw /= coeffient;
            _pitch /= coeffient;
        }
    }
}
