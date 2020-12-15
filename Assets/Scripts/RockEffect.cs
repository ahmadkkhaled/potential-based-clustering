using UnityEngine;

public class RockEffect : MonoBehaviour
{
    private float angle_x, angle_y, angle_z;
    private void Start()
    {
        angle_x = Random.Range(-35.0f, 35.0f);
        angle_y = Random.Range(-35.0f, 35.0f);
        angle_z = Random.Range(-35.0f, 35.0f);
    }

    void Update()
    {
        transform.Rotate(new Vector3(angle_x, angle_y, angle_z) * Time.deltaTime);
    }
}
