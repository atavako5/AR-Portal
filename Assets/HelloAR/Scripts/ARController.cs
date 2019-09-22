using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleARCore;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif
public class ARController: MonoBehaviour
{
    //We will fill this list with planes that ARCore detected in the current frame
    private List<DetectedPlane> m_NewDetectedPlanes = new List<DetectedPlane>();
    public GameObject GridPrefab;
    public GameObject Portal;
    public GameObject ARCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
        // Check AR core session status
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }
        //The following function will fill m_newTrackedPlanes with planes that ARCore deteced in the current frame
        Session.GetTrackables<DetectedPlane>(m_NewDetectedPlanes,TrackableQueryFilter.New);

        for (int i = 0; i < m_NewDetectedPlanes.Count; ++i) {
            GameObject grid = Instantiate(GridPrefab, Vector3.zero, Quaternion.identity, transform);

            //this function will set the position of the grid and modify the vertacies of the mesh
            grid.GetComponent<GridVisualizer>().Initialize(m_NewDetectedPlanes[i]);
        }

        //Check if the user touched the screen
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        //Let's now check if the user touched any of the tracked planes
        TrackableHit hit;
        if(Frame.Raycast(touch.position.x,touch.position.y,TrackableHitFlags.PlaneWithinPolygon,out hit)) {
            // Let's now place the portal on top of the tracked plane that we touched

            //Enable the portal

            Portal.SetActive(true);

            // Create a new anchor

            Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);

            //set the position and rotation of the portal

            Portal.transform.position = hit.Pose.position;
            Portal.transform.rotation = hit.Pose.rotation;

            // we want to always make the portal instatiate towards the scrteen
            Vector3 cameraPosition = ARCamera.transform.position;

            // only rotate around the Y axis
            cameraPosition.y = hit.Pose.position.y;

            Portal.transform.LookAt(cameraPosition, Portal.transform.up);

            Portal.transform.parent = anchor.transform;


        }


    }
}
