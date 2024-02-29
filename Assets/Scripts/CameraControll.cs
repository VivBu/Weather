using UnityEngine;

public class CameraControll : MonoBehaviour
{

    [SerializeField]
    private float _mouseSensitivityX;
    [SerializeField]
    private float _mouseSensitivityY;
    [SerializeField]
    private Transform _orientation;
    [SerializeField]
    private UIController _uiController;

    private float _rotationX;
    private float _rotationY;
    private bool _canRotate;

    private void Awake() {

        // whenever options menu is toggled, make sure we update to know if we can rotate again
       _uiController.OptionsMenuToggled += (s, e) => _canRotate = !_uiController.IsOptionsMenuOpened;

        // also make sure to pull current state
       _canRotate = !_uiController.IsOptionsMenuOpened;
    }


    private void Update()
    {
        if (!_canRotate)
            return;

        //get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * _mouseSensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * _mouseSensitivityY;

        _rotationY += mouseX;

        _rotationX -= mouseY;
        _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);

        //rotate cam
        transform.rotation = Quaternion.Euler(_rotationX, _rotationY, 0);
        _orientation.rotation = Quaternion.Euler(0, _rotationY, 0);
    }
}
