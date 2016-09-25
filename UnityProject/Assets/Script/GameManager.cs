using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RTEditor;
using UnityEditor;

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

    public GameObject LevelDesign;
    public bool ShouldUseRigidbody = true;

    void Awake()
    {
        if (LevelDesign == null)
            LevelDesign = GameObject.Find("LevelDesign");
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F2))
            TurnOnOffRigidbody();

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
            //Debug.Log("Mouse is down");

            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(EditorCamera.Instance.Camera.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, Ignoremask))
            {
                //Debug.Log("Hit " + hitInfo.transform.gameObject.name);
                if (workingObject == null)
                {
                    workingObject = Instantiate(choosingPrefab, hitInfo.point, Quaternion.identity) as GameObject;
                    if (workingObject != null)
                    {
                        workingObject.GetComponent<Rigidbody>().isKinematic = !ShouldUseRigidbody;
                        workingObject.transform.SetParent(LevelDesign.transform);
                        workingObject.layer = 2;
                        workingObject.transform.position = hitInfo.point + workingObject.GetWorldSpaceAABB().extents;
                    }
                }
                else
                {
                    workingObject.transform.position = hitInfo.point + workingObject.GetWorldSpaceAABB().extents;
                }
            }
            else
            {
//                Debug.Log("No hit");
                if (workingObject != null)
                {
                    choosingPrefab = null;
                    Destroy(workingObject);
                    workingObject = null;
                }
            }
        }
    }

    public void SetPickedPrefab(GameObject prefab)
    {
        choosingPrefab = prefab;
    }

    private void TurnOnOffRigidbody()
    {
        ShouldUseRigidbody = !ShouldUseRigidbody;

        foreach (GameObject o in GetAllObject())
        {
            o.GetComponent<Rigidbody>().isKinematic = !ShouldUseRigidbody;
        }
    }

    private List<GameObject> GetAllObject()
    {
        return FindObjectsOfType<ObjectIdentifier>().Select(o => o.gameObject).ToList();
    }
}
