using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;

    public float lookSpeed = 2f;
    public float lookXLimit = 75f;
    public GameObject spectatorCam;

    PhotonView PV;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    public bool canMove = true;
    bool destroyed;
    bool dying;
    CharacterController characterController;

    public GameObject CameraSystem;
    void Start()
    {
        destroyed = false;
        dying = false;
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CameraSystem = GameObject.FindGameObjectWithTag("CamSystem");
        CameraSystem.SetActive(false);  
    }
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    void Update()
    {
        if (!PV.IsMine) { 
            if (destroyed) return;
            Destroy(GetComponentInChildren<Camera>().gameObject);
            destroyed = true;

        return; }

        #region Handles Movment
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        #endregion

        #region Handles Jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        #endregion

        #region Handles Rotation
        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        #endregion

        if (!dying)
        {
            CameraSystem.SetActive(false);
        }
    }

    public void Die()
    {
        dying = true;
        PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player", "deathP"), transform.position, Quaternion.identity);
        CameraSystem.SetActive(true);
        PhotonNetwork.Destroy(gameObject);
    }

}
