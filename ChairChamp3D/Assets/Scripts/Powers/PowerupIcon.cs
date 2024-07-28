using UnityEngine;

public class PowerupIcon : MonoBehaviour
{
    [SerializeField] private BillBoardType billboardType;
    public enum BillBoardType { LookAtCamera, CameraForward };

    // Use late update so everything should have finished moving
    private void LateUpdate()
    {
        switch (billboardType)
        {
            case BillBoardType.LookAtCamera:
                transform.LookAt(Camera.main.transform.position, Vector3.up);
                break;
            case BillBoardType.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            default:
                break;
        }
    }
}
