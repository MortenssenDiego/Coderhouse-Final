using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{

    [Header("Camera View")]
    public Transform cameraTarget;
    public Transform viewPoint;
    public float mouseSensivity = 1f;
    public GameObject CM_Camera;
    private float verticalRotStore;
    private Vector2 mouseInput = new();
    private Animator cameraAnimator;
   
    

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float turboSpeed = 15f;
    private Vector3 moveDir, movement = new();

    [Header("Shooting")]
    public GameObject laserBeamPrefab;
    public Transform shootingPointRight;
    public Transform shootingPointLeft;


    private void Start()
    {
        if (!photonView.IsMine)
        {
            CM_Camera.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        cameraAnimator = cameraTarget.GetComponent<Animator>();
    }

    private void Update()
    {
        if(photonView.IsMine)
        {
            SetRotation();

            SetMovement();

            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }
    }
    private void Shoot()
    {
        GameObject laserBeam1 = PhotonNetwork.Instantiate(laserBeamPrefab.name, shootingPointRight.position, shootingPointRight.rotation);
        GameObject laserBeam2 = PhotonNetwork.Instantiate(laserBeamPrefab.name, shootingPointLeft.position, shootingPointRight.rotation);
        Destroy(laserBeam1, 5.0f);
        Destroy(laserBeam2, 5.0f);
    }

    private void SetMovement()
    {
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 1.0f);

        movement = transform.forward * moveDir.z * 3 + transform.forward * 1.5f;

        if(Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += movement * turboSpeed * Time.deltaTime;
            cameraAnimator.SetBool("isTurbo", true);
        } else
        {
            transform.position += movement * moveSpeed * Time.deltaTime;
            cameraAnimator.SetBool("isTurbo", false);
        }
    }

    private void SetRotation()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensivity;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

        verticalRotStore += mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -90f, 90f);

        viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(photonView.IsMine)
        {
            PlayerSpawner.instance.Die();
        }
    }
}
