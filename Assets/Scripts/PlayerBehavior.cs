using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector2 rotation;
    private float smooth = 1.2f;
    public Transform Target;
    [SerializeField] private float jumpForce;
    [SerializeField] private int vie;
    private int vieMax;
    [SerializeField] private Slider lifeBar;
    [SerializeField] private float stepInterval;
    private float actualStepInterval = 0;

    [SerializeField] private bool canJump = true;
    // Start is called before the first frame update
    private void Start()
    {
        rotation = new Vector2(transform.eulerAngles.x,transform.eulerAngles.y);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //DontDestroyOnLoad(this);
        vieMax = vie;
    }

    private void Update()
    {
        vie = Mathf.Clamp(vie, 0, 100);
        lifeBar.value = (float)vie / vieMax;

        if (Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadScene(0);
        }
    }

    void LateUpdate()
    {
        
        PlayerMovement();
        MouseLook();
        //Jumping();
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
        if (horz != 0 || vert != 0)
        {
            StepSoundInterval();
        }

    }

    private void MouseLook()
    {
        rotation.y += Input.GetAxis ("Mouse X");
        rotation.x += -Input.GetAxis ("Mouse Y");
        Camera.main.transform.LookAt(Target);
        rotation.x = Mathf.Clamp(rotation.x, -66, 66);
        Target.rotation = Quaternion.Euler(rotation.x, rotation.y,0);
        transform.rotation = Quaternion.Euler(transform.rotation.x, rotation.y,transform.rotation.z);


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
    }

    private void StepSoundInterval()
    {
        if (actualStepInterval > 0)
        {
            actualStepInterval -= 1 * Time.deltaTime;
        }
        else
        {
            actualStepInterval = stepInterval;
            FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Pas/Bruit de pas (ok)", transform.position);
        }
    }

    public void AddLifePoints(int lifeAmmount)
    {
        vie += lifeAmmount;
    }

}
