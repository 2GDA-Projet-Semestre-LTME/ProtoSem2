﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector2 rotation = new Vector2 (0, 0);
    private float smooth = 1.2f;
    public Transform Target;
    [SerializeField] private float jumpForce;
    [SerializeField] private int vie;
    private int vieMax;
    [SerializeField] private Slider lifeBar;

    [SerializeField] private bool canJump = false;
    // Start is called before the first frame update
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        DontDestroyOnLoad(this);
        vieMax = vie;
    }

    private void Update()
    {
        lifeBar.value = (float)vie / vieMax;
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

        if (Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadScene(0);
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
            GetComponent<Animator>().Play("Jump");
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

    public bool GetJump()
    {
        return canJump;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Ground"))
        {
            canJump = true;
        }
        if(other.transform.CompareTag("LifeBox"))
        {
            vie += other.transform.GetComponent<LifeItem>().lifeToAdd;
            Destroy(other.gameObject);
        }
    }

    public void ApplyDammage(int dmg)
    {
        vie -= dmg;
        print("Application des dommages " + dmg);
    }

    // Update is called once per frame
    
}
