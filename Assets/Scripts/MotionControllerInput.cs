using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class MotionControllerInput : MonoBehaviour
{
    public GameObject LeftPrefab;
    public GameObject RightPrefab;
    public Transform cameraRig;
    public TextMesh StatusText;
    public float StickDeadZone = 0.2f;
    public float TurnThreshold = 0.2f;
    public float MoveRate = 3f;
    public float TurnRate = 90f;

    private Dictionary<uint, GameObject> trackingObject;
    private Vector2 leftStick;
    private Vector2 rightStick;
    private Transform cameraTransform;
    private float curYaw;
    private float curPitch;
    private float prevYaw;
    private float prevPitch;
    private string keyString;

    void Start()
    {
        trackingObject = new Dictionary<uint, GameObject>();
        cameraTransform = Camera.main.transform;
        prevYaw = curYaw = 0f;
        prevPitch = curPitch = 0f;

        InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
        InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
        InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
        InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;
        InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;
    }

    void OnDestroy()
    {
        InteractionManager.InteractionSourceDetected -= InteractionManager_InteractionSourceDetected;
        InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;
        InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;
        InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;
        InteractionManager.InteractionSourceUpdated -= InteractionManager_InteractionSourceUpdated;
    }


    private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs obj)
    {
        uint id = obj.state.source.id;

        if (obj.state.source.kind != InteractionSourceKind.Controller)
        {
            return;
        }

        if (!trackingObject.ContainsKey(id))
        {
            GameObject objPrefab = null;
            if (obj.state.source.handedness == InteractionSourceHandedness.Left)
            {
                objPrefab = Instantiate(LeftPrefab);
            }
            else if (obj.state.source.handedness == InteractionSourceHandedness.Right)
            {
                objPrefab = Instantiate(RightPrefab);
            }
            if (objPrefab)
            {
                objPrefab.transform.SetParent(cameraRig);
                trackingObject.Add(id, objPrefab);
            }
        }
    }

    private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs obj)
    {
        uint id = obj.state.source.id;
        if (!trackingObject.ContainsKey(id))
        {
            Destroy(trackingObject[id]);
            trackingObject.Remove(id);
        }
    }

    private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs obj)
    {
        uint id = obj.state.source.id;

    }

    private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs obj)
    {
        uint id = obj.state.source.id;
    }

    private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs obj)
    {
        uint id = obj.state.source.id;
        Vector3 pos;
        Quaternion rot;


        // Tracked Object
        if (trackingObject.ContainsKey(id))
        {
            if (obj.state.sourcePose.TryGetPosition(out pos))
            {
                trackingObject[id].transform.localPosition = pos;
            }
            if (obj.state.sourcePose.TryGetRotation(out rot))
            {
                trackingObject[id].transform.localRotation= rot;
            }
        }

        // Hardware Based Movment - No dependecy on detection
        if (obj.state.source.handedness == InteractionSourceHandedness.Left)
        {
            leftStick = obj.state.thumbstickPosition;

        }
        else if (obj.state.source.handedness == InteractionSourceHandedness.Right)
        {
            rightStick = obj.state.thumbstickPosition;
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            keyString = "Down: Space";
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            keyString = "Up: Space";
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            keyString = "Down: Q";
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            keyString = "Up: Q";
        }

        var xlsx = deadZoneCheck(Input.GetAxis("Axis1"));
        var xlsy = deadZoneCheck(-Input.GetAxis("Axis2"));
        var xrsx = deadZoneCheck(Input.GetAxis("Axis4"));
        var xrsy = deadZoneCheck(-Input.GetAxis("Axis5"));

        var lsx = deadZoneCheck(leftStick.x);
        var lsy = deadZoneCheck(leftStick.y);
        var rsx = deadZoneCheck(rightStick.x);
        var rsy = deadZoneCheck(rightStick.y);

        curYaw += angleFix((rsx + xrsx) * TurnRate * Time.deltaTime);
        curPitch += angleFix((rsy + xrsy) * TurnRate * Time.deltaTime);

        var lsVector = new Vector3(lsx + xlsx, 0f, lsy + xlsy);
        var moveVector = Quaternion.Euler(0f, curYaw, 0f) * lsVector;
        cameraRig.position += (moveVector * MoveRate * Time.deltaTime);

        if ((Mathf.Abs(curPitch - prevPitch) > TurnThreshold) || (Mathf.Abs(curYaw - prevYaw) > TurnThreshold))
        {
            prevYaw = curYaw;
            prevPitch = curPitch;
            cameraRig.rotation = Quaternion.Euler(curPitch, curYaw, 0f);            
        }
        
        StatusText.text = $"LS:({leftStick.x:0.000},{leftStick.y:0.000}) RS:({rightStick.x:0.000},{rightStick.y:0.000})\nXLS:({xlsx:0.000},{xlsy:0.000})\nXRS:({xrsx:0.000},{xrsy:0.000})\nMoveVector: {moveVector}\n{keyString}";
    }


    // Utility Methods
    private float deadZoneCheck(float v)
    {
        return Mathf.Abs(v) > StickDeadZone ? v : 0f;
    }

    private float angleFix(float angle)
    {
        while (angle > 360f)
            angle -= 360;

        while (angle < 360f)
            angle += 360f;

        return angle;
    }
}
