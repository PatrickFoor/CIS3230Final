using UnityEngine;
using System.Collections;

public class TP_Camera : MonoBehaviour
{

    public static TP_Camera Instance;

    public Transform TargetLookAt;
    public Transform CameraPos;

    public float Distance = 5f;
    public float DistanceMin = 3f;
    public float DistanceMax = 10f;
    public float DistanceSmooth = 0.05f;
    public float DistanceResumeSmooth = 1f;
	public float X_MouseSensitivity = 5f;
	public float Y_MouseSensitivity = 5f;
	public float MouseWheelSensitivity = 5f;
    public float X_Smooth = 0.05f;
    public float Y_Smooth = 0.01f;
	public float Y_MinLimit = -40f;
	public float Y_MaxLimit = 80f;
    public float OcclusionDistanceStep = 0.5f;
    public int MaxOcclusionChecks = 10;

    private float mouseX = 0f;
    private float mouseY = 0f;
    private float velX = 0f;
    private float velY = 0f;
    private float velZ = 0f;
    private float velDistance = 0f;
    private float startDistance = 0f;
    private Vector3 position = Vector3.zero;
    private float desiredDistance = 0f;
    private Vector3 desiredPosition = Vector3.zero;
    private float distanceSmooth = 0f;
    private float preOccludedDistance = 0f;
    
    private Vector3 offset;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
        startDistance = Distance;

        offset = CameraPos.transform.position - Camera.main.transform.position;

        Reset();
    }

    // LateUpdate is called once per frame after Update
    void LateUpdate()
    {
        if (TargetLookAt == null)
        {
            return;
        }

        HandlePlayerInput();

        var count = 0;
        do 
        {
            CalculateDesiredPosition();
            count++;
        } while (CheckIfOccluded(count));

        UpdatePosition();
    }

    void HandlePlayerInput()
    {
		var deadZone = 0.01f;
		
		//if (Input.GetMouseButton(1)) {
			//RMB is down, get mouse axis input - NOTE: not currently implemented
		mouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity;
		mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;
		//}

        if (Input.GetMouseButton(1))
        {
            SnapBehindCharacter();
        }
		
		//Limit/Clamp mouseY
		mouseY = Helper.ClampAngle(mouseY, Y_MinLimit, Y_MaxLimit);
		
		if (Input.GetAxis("Mouse ScrollWheel") < -deadZone || Input.GetAxis("Mouse ScrollWheel") > deadZone) {
			desiredDistance = Mathf.Clamp (Distance - Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity, 
				DistanceMin, DistanceMax);

            preOccludedDistance = desiredDistance;
            distanceSmooth = DistanceSmooth;
		}
    }

    void SnapBehindCharacter()
    {
        float desiredAngle = TP_Controller.CharacterController.transform.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);

        CameraPos.rotation = rotation;
        CameraPos.position = new Vector3(TP_Controller.CharacterController.transform.position.x,
            TP_Controller.CharacterController.transform.position.y + 1, TP_Controller.CharacterController.transform.position.z - 5);

        transform.position = CameraPos.position - (rotation * offset);

        transform.LookAt(TargetLookAt);
    }

    void CalculateDesiredPosition()
    {
        //evaluate distance (smoothing calculations)
        ResetDesiredDistance();
        Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velDistance, distanceSmooth);
        
        //calculate desired position
        desiredPosition = CalculatePosition(mouseY, mouseX, Distance);
    }

    Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
    {
        Vector3 direction = new Vector3(0, 0, -distance);

        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);

        return TargetLookAt.position + rotation * direction;
    }

    bool CheckIfOccluded(int count)
    {
        var isOccluded = false;

        var nearestDistance = CheckCameraPoints(TargetLookAt.position, desiredPosition);

        if (nearestDistance != -1)
        {
            if (count < MaxOcclusionChecks)
            {
                isOccluded = true;
                Distance -= OcclusionDistanceStep;

                if (Distance < 0.25f)
                {
                    Distance = 0.25f;
                }
            }
            else
            {
                Distance = nearestDistance - Camera.main.nearClipPlane;
            }

            desiredDistance = Distance;
            distanceSmooth = DistanceResumeSmooth;
        }

        return isOccluded;
    }

    float CheckCameraPoints(Vector3 from, Vector3 to) 
    {
        var nearDistance = -1f;

        RaycastHit hitInfo;

        Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneAtNear(to);

        //Draw lines in the editor to make it easier to visualize
        Debug.DrawLine(from, to + transform.forward * -camera.nearClipPlane, Color.red);
        Debug.DrawLine(from, clipPlanePoints.UpperLeft);
        Debug.DrawLine(from, clipPlanePoints.LowerLeft);
        Debug.DrawLine(from, clipPlanePoints.UpperRight);
        Debug.DrawLine(from, clipPlanePoints.LowerRight);

        Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight);
        Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight);
        Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft);
        Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft);

        if (Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo) && !hitInfo.collider.tag.Equals("Player"))
        {
            nearDistance = hitInfo.distance;
        }
        if (Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo) && !hitInfo.collider.tag.Equals("Player"))
        {
            if (hitInfo.distance < nearDistance || nearDistance == -1)
                nearDistance = hitInfo.distance;
        }
        if (Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo) && !hitInfo.collider.tag.Equals("Player"))
        {
            if (hitInfo.distance < nearDistance || nearDistance == -1)
                nearDistance = hitInfo.distance;
        }
        if (Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo) && !hitInfo.collider.tag.Equals("Player"))
        {
            if (hitInfo.distance < nearDistance || nearDistance == -1)
                nearDistance = hitInfo.distance;
        }

        if (Physics.Linecast(from, to + transform.forward * -camera.nearClipPlane, out hitInfo) && !hitInfo.collider.tag.Equals("Player"))
        {
            if (hitInfo.distance < nearDistance || nearDistance == -1)
                nearDistance = hitInfo.distance;
        }

        return nearDistance;
    }

    void ResetDesiredDistance()
    {
        if (desiredDistance < preOccludedDistance)
        {
            var pos = CalculatePosition(mouseY, mouseX, preOccludedDistance);

            var nearestDistance = CheckCameraPoints(TargetLookAt.position, pos);

            if (nearestDistance == -1 || nearestDistance > preOccludedDistance)
            {
                desiredDistance = preOccludedDistance;
            }
        }
    }

    void UpdatePosition()
    {
        var posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, X_Smooth);
        var posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, Y_Smooth);
        var posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, X_Smooth);
        position = new Vector3(posX, posY, posZ);

        transform.position = position;

        transform.LookAt(TargetLookAt);
    }

    public void Reset()
    {
        mouseX = 0f;
        mouseY = 10f;
        Distance = startDistance;
        desiredDistance = Distance;
        preOccludedDistance = Distance;
    }

    public static void UseExistingOrCreateNewMainCamera()
    {
        GameObject tempCamera;
        GameObject targetLookAt;
        GameObject cameraPos;
        TP_Camera myCamera;

        if (Camera.main != null)
        {
            tempCamera = Camera.main.gameObject;
        }
        else
        {
            tempCamera = new GameObject("Main Camera");
            tempCamera.AddComponent("Camera");
            tempCamera.tag = "MainCamera";
        }

        tempCamera.AddComponent("TP_Camera");
        myCamera = (TP_Camera)tempCamera.GetComponent("TP_Camera");

        targetLookAt = (GameObject)GameObject.Find("targetLookAt");

        if (targetLookAt == null)
        {
            targetLookAt = new GameObject("targetLookAt");
            targetLookAt.transform.position = Vector3.zero;
        }

        cameraPos = (GameObject)GameObject.Find("CameraPos");

        if (cameraPos == null)
        {
            cameraPos = new GameObject("CameraPos");
            cameraPos.transform.rotation = TP_Controller.CharacterController.transform.rotation;
            cameraPos.transform.position = new Vector3(0, TP_Controller.CharacterController.transform.position.y + 1, 
                TP_Controller.CharacterController.transform.position.z);
        }
        else
        {
            cameraPos.transform.rotation = TP_Controller.CharacterController.transform.rotation;
            cameraPos.transform.position = new Vector3(0, TP_Controller.CharacterController.transform.position.y + 1, 
                TP_Controller.CharacterController.transform.position.z - 10);
        }

        myCamera.CameraPos = cameraPos.transform;
        myCamera.TargetLookAt = targetLookAt.transform;
    }
}
