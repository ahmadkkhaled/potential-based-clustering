using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 100f;
    public Transform playerBody;

    private float rotationAroundX = 0.0f;
    private float rotationAroundY = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: handle mouse lock and unlock
    }

    // Update is called once per frame
    void Update()
    {
        // rotation around Y-axis 
        rotationAroundY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        // rotation around X-axis
        rotationAroundX -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        rotationAroundX = Mathf.Clamp(rotationAroundX, -90f, 90f);

        playerBody.localRotation = Quaternion.Euler(rotationAroundX, rotationAroundY, 0f); /// assign not update

    }
}
