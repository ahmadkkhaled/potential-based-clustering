using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 100f;
    public Transform playerBody;

    private float rotationAroundX = 0.0f;
    private float rotationAroundY = 0.0f;
    private int zoom = 20;
    private int normalFOV = 60;
    private float smooth = 5f;
    private bool isZoomed = false;

    // Update is called once per frame
    void Update()
    {
        // rotation around Y-axis 
        rotationAroundY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        // rotation around X-axis
        rotationAroundX -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        rotationAroundX = Mathf.Clamp(rotationAroundX, -90f, 90f);

        playerBody.localRotation = Quaternion.Euler(rotationAroundX, rotationAroundY, 0f); /// assign not update

        // camera zoom
        Camera camera = GetComponent<Camera>();
        if (Input.GetMouseButtonDown(1))
            isZoomed = !isZoomed;

        if (isZoomed)
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, zoom, smooth * Time.deltaTime);
        else
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, normalFOV, smooth * Time.deltaTime); 
    }
}
