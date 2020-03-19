using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector2 rotation = new Vector2 (0, 0);
    private float smooth = 1.2f;
    public Transform Target;
    [SerializeField] private float jumpForce;

    [SerializeField] private bool canJump = false;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void LateUpdate()
    {
        
        PlayerMovement();
        MouseLook();
        Jumping();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void PlayerMovement()
    {

        float horz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        Vector3 forwardVector = transform.forward * vert;
        Vector3 rightVector = transform.right * horz;
        GetComponent<Rigidbody>().position += Vector3.ClampMagnitude(forwardVector + rightVector, 1f) * speed * Time.deltaTime * smooth;

    }

    private void Jumping()
    {
        if (canJump && Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Rigidbody>().AddForce(transform.up * jumpForce);
            canJump = false;
        }
    }

    private void MouseLook()
    {
        rotation.y += Input.GetAxis ("Mouse X");
        rotation.x += -Input.GetAxis ("Mouse Y");
        Camera.main.transform.LookAt(Target);
        rotation.x = Mathf.Clamp(rotation.x, -66, 66);
        Target.rotation = Quaternion.Euler(rotation.x, rotation.y,0);
        transform.rotation = Quaternion.Euler(0, rotation.y,0);


    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Ground"))
        {
            canJump = true;
        }
    }

    // Update is called once per frame
    
}
