using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public GameObject UIPrefab;
    public LayerMask CameraMask;

    public GameObject CameraPivot;
    public GameObject ArmsPivot;
    public GameObject Hat;
    public Camera camera;

    float LookSpeedX = 400;
    float LookSpeedY = 200;
    float StrafeSpeed = 15;
    float ForwardSpeed = 10;

    [HideInInspector]
    [SyncVar]
    public int Score = 0;
    
    [HideInInspector]
    [SyncVar (hook = "OnHatColorChanged")]
    public Color hatColor;

    Vector3 verticalVelocity;
    float JumpSpeed = 6.5f;

    GameObject ui;
    CharacterController controller;
    Quaternion cameraRotation;
    Weapon weapon;
    
    void Start()
    {
        CameraPivot = transform.Find("CameraPivot").gameObject;
        ArmsPivot = transform.Find("ArmPivot").gameObject;
        Hat = transform.Find("Hat").gameObject;
        camera = GetComponentInChildren<Camera>();
        weapon = GetComponentInChildren<Weapon>();

        if (isServer)
        {
            hatColor = Random.ColorHSV();
        }

        if (isLocalPlayer == false)
        {
            GetComponentInChildren<AudioListener>().enabled = false;
            OnHatColorChanged(hatColor);
            Destroy(camera.gameObject);
            return;
        }

        controller = (CharacterController)GetComponent(typeof(CharacterController));

        cameraRotation = camera.transform.rotation;

        Camera mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        mainCamera.enabled = false;

        ui = Instantiate<GameObject>(UIPrefab);
        PlayerUI playerUI = ui.GetComponent<PlayerUI>();
        playerUI.player = this;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnHatColorChanged(Color color)
    {
        hatColor = color;
        MeshRenderer[] meshes = Hat.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.material.color = color;
        }
    }
    

	void Update ()
    {
        if (isLocalPlayer == false)
            return;

        // Camera rotation
        float mouseX = Input.GetAxis("Mouse X") * LookSpeedX;
        float mouseY = Input.GetAxis("Mouse Y") * -LookSpeedY;

        float camX = Mathf.Clamp((mouseY * Time.deltaTime + cameraRotation.eulerAngles.x + 90) % 360, 5, 175) - 90;
        float camY = mouseX * Time.deltaTime + cameraRotation.eulerAngles.y;
        cameraRotation.eulerAngles = new Vector3(camX, camY, 0);
        //cameraRotation.eulerAngles += new Vector3(mouseY, mouseX, 0) * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, cameraRotation.eulerAngles.y, 0); // Rotate body
        CameraPivot.transform.rotation = cameraRotation; // Rotate camera
        ArmsPivot.transform.localEulerAngles = new Vector3(cameraRotation.eulerAngles.x, 0, 0); // Rotate arms

        float cameraDistance = 3.2f;
        RaycastHit hit;
        //int mask = ~(1 << LayerMask.NameToLayer("LocalPlayer")); // TODO use LayerMask
        Transform cameraTrans = CameraPivot.transform;
        if (Physics.Raycast(cameraTrans.position, -cameraTrans.forward, out hit, cameraDistance, CameraMask))
            camera.transform.localPosition = new Vector3(0, 0, -hit.distance);
        else
            camera.transform.localPosition = new Vector3(0, 0, -cameraDistance);


        // Movement
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal2"), 0f, Input.GetAxis("Vertical2"));
        float len = Mathf.Min(1, moveInput.magnitude);
        moveInput = moveInput.normalized * len;
        moveInput.x *= ForwardSpeed;
        moveInput.z *= StrafeSpeed;
        moveInput = CameraPivot.transform.TransformDirection(moveInput);

        // Can only jump when on the ground
        if (controller.isGrounded)
        {
            // Jump
            if (Input.GetKeyDown("space"))
                verticalVelocity = new Vector3(0, JumpSpeed, 0);
        }
        else
            verticalVelocity += new Vector3(0, Physics.gravity.y * Time.deltaTime, 0);

        // add vertical velocity
        moveInput.y = verticalVelocity.y;

        controller.Move(moveInput * Time.deltaTime);
    }


    [Command]
    public void CmdFire(Vector3 endPoint)
    {
        Vector3 direction = (endPoint - weapon.barrelEnd.position).normalized;

        GameObject bulletGO = Instantiate<GameObject>(weapon.BulletPrefab, weapon.barrelEnd.position + 0.3f * weapon.barrelEnd.forward, Quaternion.identity);
        Rigidbody rigidBody = bulletGO.GetComponent<Rigidbody>();
        rigidBody.velocity = 40f * direction;// camera.transform.forward;

        Bullet bullet = bulletGO.GetComponent<Bullet>();
        bullet.player = this;

        NetworkColor color = bulletGO.GetComponent<NetworkColor>();
        color.SetColor(hatColor);
        
        NetworkServer.Spawn(bulletGO);

        Destroy(bulletGO, 2);
    }

    private void OnDestroy()
    {
        Destroy(ui);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        GameObject mainCameraGO = GameObject.Find("Main Camera");
        if (mainCameraGO)
        {
            Camera mainCamera = mainCameraGO.GetComponent<Camera>();
            mainCamera.enabled = true;
        }
    }
}
