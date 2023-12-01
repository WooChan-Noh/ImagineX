using System.Collections;
using UnityEngine;

/// <summary>
/// push -> chage tag to "RAY"
/// pull -> change tag to "RAYOFF"
/// special tag : PREVENT
/// </summary>
public class HandRayControll : MonoBehaviour
{

    public myRayControll leftRayControll;
    public myRayControll rightRayControll;

    void Start()
    {
        leftRayControll = new myRayControll(transform.GetChild(4).transform, transform.GetChild(6).GetComponent<LineRenderer>());
        rightRayControll = new myRayControll(transform.GetChild(5).transform, transform.GetChild(7).GetComponent<LineRenderer>());
    }
    void Update()
    {
        //OVRInput.Update(); What's this?
        leftRayControll.LeftRaycast();
        rightRayControll.RightRaycast();

    }
    public class myRayControll
    {
        private CameraManager cameraManager = CameraManager.cameraManager;

        private Transform handPosition;
        [HideInInspector] public LineRenderer lineRenderer;
        private Ray ray;
        private RaycastHit hit;

        private string startRecordTag = "RAY";
        private string endRecordTag = "RAYOFF";
        private string preventTag = "PREVENT";
        public myRayControll(Transform handTransform, LineRenderer lineRenderer)
        {
            this.handPosition = handTransform;
            this.lineRenderer = lineRenderer;
            ray = new Ray();
        }
        public void RightRaycast()
        {
            ray.origin = handPosition.position;
            ray.direction = handPosition.rotation * Vector3.forward;


            if (Physics.Raycast(ray, out hit))//drawing ray
            {
                SetLineRenderer();
            }

            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))//press button
            {
                ChangeLineColor(Color.green);
                if (Physics.Raycast(ray, out hit))
                {
                    SetLineRenderer();
                    ChangeTag(startRecordTag);
                }
            }
            if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger))//keep press 
            {
                ChangeLineColor(Color.green);
                if (Physics.Raycast(ray, out hit))
                {
                    SetLineRenderer();
                }
            }
            if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))//release button
            {
                ChangeLineColor(Color.blue);
                if (Physics.Raycast(ray, out hit))
                {
                    SetLineRenderer();
                    ChangeTag(endRecordTag);
                }
            }


        }
        public void LeftRaycast()
        {
            ray.origin = handPosition.position;
            ray.direction = handPosition.rotation * Vector3.forward;

            if (Physics.Raycast(ray, out hit))//drawing ray
            {
                SetLineRenderer();
            }
            if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))//press button
            {
                ChangeLineColor(Color.green);
                if (Physics.Raycast(ray, out hit))
                {
                    SetLineRenderer();
                    ChangeTag(startRecordTag);
                }
            }
            if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger))//keep press 
            {
                ChangeLineColor(Color.green);
                if (Physics.Raycast(ray, out hit))
                {
                    SetLineRenderer();
                }
            }
            if (OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger))//release button
            {
                ChangeLineColor(Color.red);
                if (Physics.Raycast(ray, out hit))
                {
                    SetLineRenderer();
                    ChangeTag(endRecordTag);
                }
            }


        }
        private void SetLineRenderer()
        {
            LineRendererCheckPOV();
            lineRenderer.SetPosition(0, ray.origin);
            lineRenderer.SetPosition(1, hit.point);

        }
        private void LineRendererCheckPOV()
        {
            if (cameraManager.thirdPOV == true)//Third Person View
                lineRenderer.enabled = false;//can't use Ray
            else
                lineRenderer.enabled = true;
        }
        private void ChangeLineColor(Color color)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
        private void ChangeTag(string tag)
        {   //prevent record for same frame
            if (tag == startRecordTag)
            {
                if (hit.collider.gameObject.tag == startRecordTag)
                    hit.collider.gameObject.tag = preventTag;
                else
                    hit.collider.gameObject.tag = tag;
            }
            else if (tag == endRecordTag)
            {
                if (hit.collider.gameObject.tag == preventTag)
                    hit.collider.gameObject.tag = startRecordTag;
                else
                    hit.collider.gameObject.tag = tag;
            }
        }
    }

}

