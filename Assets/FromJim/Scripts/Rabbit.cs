using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : MonoBehaviour
{
    private CharacterController characterController;
    private int direction = 0;
    public float speed = 10f;
    public float rotateSpeed = 5f;

    // Use this for initialization
    void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();
        float interval = Random.value * 3f + 1f; // pick a random number between 1 and 4
        InvokeRepeating("ChangeDirection", 1f, interval);
    }

    // Update is called once per frame
    void Update()
    {
        // Move in the forward direction
        characterController.SimpleMove(transform.TransformDirection(Vector3.forward) * speed * Time.deltaTime);
    }

    private void ChangeDirection()
    {
        direction = (int)(Random.value * 4f);
        var rotation = transform.rotation;

        switch (direction)
        {
            case 0:
                // Already facing forward
                break;
            case 1:
                // Turn around
                transform.rotation = rotation * Quaternion.Euler(0, 180, 0);
                break;
            case 2:
                // Turn right
                transform.rotation = rotation * Quaternion.Euler(0, 90, 0);
                break;
            case 3:
                // Turn left
                transform.rotation = rotation * Quaternion.Euler(0, -90, 0);
                break;
            default:
                // By default face forward
                break;
        }
    }
}
