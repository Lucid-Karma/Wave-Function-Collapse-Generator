using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    #region CharacterController
    private CharacterController characterController;
    public CharacterController CharacterController { get { return (characterController == null) ? characterController = GetComponent<CharacterController>() : characterController; } }

    public float speed;
    public float rotationSpeed;
    Vector3 dir;

    private void FixedUpdate()
    {
        Look();
        Move();
    }
    private void Update()
    {
        GatherInput();
        //CheckGround();
    }

    void GatherInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        dir = new Vector3(h, 0, v);
        dir = Camera.main.transform.TransformDirection(dir);
    }

    void Look()
    {
        if (dir != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    void Move()
    {
        CharacterController.SimpleMove(dir * speed);
    }
    #endregion

    private void CheckGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2.5f))
        {
            float groundDepthThreshold = 0.3f; 
            float distance = hit.distance;

            if (hit.point.y <= groundDepthThreshold)
            {
                Debug.Log("road");
            }
            else
            {
                Debug.Log("green");
            }

            Debug.Log("name: " + hit.collider.name + " is: " + distance);
        }
    }
}
