using UnityEngine;

public class CameraController : Observer
{
    private const float Sensitivity = 100f;
    private const float MovementSpeed = 20f;

    private bool _mousePress = false;

    private GameController _gameController;

    protected override void Start()
    {
        base.Start();
        _gameController = (GameController) subject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
            _mousePress = true;

        if (Input.GetMouseButtonUp(2))
            _mousePress = false;
    }

    private void FixedUpdate()
    {
        if (_gameController.GetState() == GameState.NotStarted) return;
        
        if (_mousePress)
        {
            float horizontal = Sensitivity * Input.GetAxis("Mouse X") * Time.fixedDeltaTime;
            float vertical = Sensitivity * Input.GetAxis("Mouse Y") * Time.fixedDeltaTime;

            transform.Rotate(-vertical, horizontal, 0);

            //cancel zAxis rotation
            float zAxis = transform.eulerAngles.z;
            transform.Rotate(0, 0, -zAxis);
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move =  Time.fixedDeltaTime * MovementSpeed * (transform.right * x + transform.forward * z);
        transform.position += move;
    }
}

