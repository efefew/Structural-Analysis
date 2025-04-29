using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    #region Fields

    private Transform tr;
    private Camera cam;
    [Range(0f, 100f)]
    public float moveButton, moveMouse, zoomButton, zoomScroll;
    private const float moveButtonScale = 1000,
                        moveMouseScale = 500,
                        zoomButtonScale = 1000,
                        zoomScrollScale = 1000;
    private const int minOrthographicSize = 3;

    #endregion Fields

    #region Methods

    private void Start()
    {
        cam = GetComponent<Camera>();
        tr = transform;
    }
    private void Update()
    {

        if (Input.GetAxis("Mouse ScrollWheel") >= 0.1 && cam.orthographicSize > minOrthographicSize)
            cam.orthographicSize -= zoomScroll * cam.orthographicSize / zoomScrollScale;

        if (Input.GetAxis("Mouse ScrollWheel") <= -0.1)
            cam.orthographicSize += zoomScroll * cam.orthographicSize / zoomScrollScale;

        if (Input.GetKey(KeyCode.Mouse1))
        {
            tr.position -= tr.up * Input.GetAxis("Mouse Y") * moveMouse * cam.orthographicSize / moveMouseScale;
            tr.position -= tr.right * Input.GetAxis("Mouse X") * moveMouse * cam.orthographicSize / moveMouseScale;
        }
    }
    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Equals) && cam.orthographicSize > minOrthographicSize)
            cam.orthographicSize -= zoomButton * cam.orthographicSize / zoomButtonScale;

        if (Input.GetKey(KeyCode.Minus))
            cam.orthographicSize += zoomButton * cam.orthographicSize / zoomButtonScale;

        if (Input.anyKey)
        {
            float force = moveButton * cam.orthographicSize / moveButtonScale;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                tr.position -= transform.right * force;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                tr.position += transform.right * force;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                tr.position += transform.up * force;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                tr.position -= transform.up * force;

            if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.Space))
            {
                tr.position = new Vector3(13, 11, -10);
                cam.orthographicSize = 14;
            }
        }
    }

    #endregion Methods
}