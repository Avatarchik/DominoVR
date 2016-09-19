using UnityEngine;
using System.Collections;
using RTEditor;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.Find("GameManager").GetComponent<GameManager>();
            return _instance;
        }

        set { _instance = value; }
    }

    private GameObject choosingPrefab = null;
    protected Mouse _mouse = new Mouse();

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.Escape))
	    {
	        choosingPrefab = null;
	        Destroy(workingObject);
	        workingObject = null;
	    }

        _mouse.UpdateInfoForCurrentFrame();
	    if (_mouse.IsLeftMouseButtonDown)
	    {
	        if (workingObject != null)
	        {
	            workingObject.layer = 0;
                choosingPrefab = null;
                workingObject = null;
            }
	    }

        MouseRaycast();
	}

    //private Bounds objectWorldAABB;
    private GameObject workingObject;
    int Ignoremask = ~(1 << 2);
    private void MouseRaycast()
    {
        if (choosingPrefab != null)
        {
            Debug.Log("Mouse is down");

            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(EditorCamera.Instance.Camera.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, Ignoremask))
            {
                Debug.Log("Hit " + hitInfo.transform.gameObject.name);
                if (workingObject == null)
                {
                    workingObject = Instantiate(choosingPrefab, hitInfo.point, Quaternion.identity) as GameObject;
                    if (workingObject != null)
                    {
                        workingObject.layer = 2;
                        workingObject.transform.position = hitInfo.point + workingObject.GetWorldSpaceAABB().extents;
                    }
                }
                else
                {
                    workingObject.transform.position = hitInfo.point + workingObject.GetWorldSpaceAABB().extents;
                }

                //GameObject selectedObject = hitInfo.transform.gameObject;
                //objectWorldAABB = selectedObject.GetWorldSpaceAABB();
                //if (objectWorldAABB.IsValid())
                //{
                //}
            }
            else {
                Debug.Log("No hit");
            }
        }
    }

    public void SetPickedPrefab(GameObject prefab)
    {
        choosingPrefab = prefab;
    }

    //void OnDrawGizmos()
    //{
    //    if (!choosingPrefab) return;
    //    if(objectWorldAABB.IsValid())
    //        Gizmos.DrawCube(objectWorldAABB.center, new Vector3(1, objectWorldAABB.extents.y, 1));
    //    else
    //        Debug.Log("AABB Invalid");
    //}
}
