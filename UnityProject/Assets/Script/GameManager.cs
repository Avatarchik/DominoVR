using UnityEngine;
using System.Collections;

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

    private bool choosingPrefab = false;
    

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.Escape))
	        choosingPrefab = false;

        MouseRaycast();
	}

    private void MouseRaycast()
    {
        if (choosingPrefab)
        {
            Debug.Log("Mouse is down");

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                Debug.Log("Hit " + hitInfo.transform.gameObject.name);
            }
            else {
                Debug.Log("No hit");
            }
        }
    }

    public void SetPickedPrefab()
    {
        choosingPrefab = true;
    }
}
