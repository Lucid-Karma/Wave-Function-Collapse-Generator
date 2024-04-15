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
            // Ray'in temas ettiði noktanýn z eksenindeki konumunu kontrol et
            float groundDepthThreshold = 0.3f; // Ot ve yol arasýndaki geçiþ derinlik eþiði
            float distance = hit.distance;

            if (hit.point.y <= groundDepthThreshold)
            {
                // Eðer ray'in temas ettiði noktanýn yüksekliði belirli bir eþiðin altýndaysa,
                // karakter otun üzerinde kabul edilir
                Debug.Log("Karakter yolun üzerinde.");
            }
            else
            {
                // Aksi halde, karakter yolun üzerinde kabul edilir
                Debug.Log("Karakter otun üzerinde.");
            }

            Debug.Log("name: " + hit.collider.name + " is: " + distance);
        }
    }
}
