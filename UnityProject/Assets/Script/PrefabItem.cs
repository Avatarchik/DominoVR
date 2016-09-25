using UnityEngine;
using System.Collections;

public class PrefabItem : MonoBehaviour
{

    public GameObject Prefab;

    private UIButton mBtn;
	// Use this for initialization
	void Start ()
	{
	    mBtn = gameObject.GetComponent<UIButton>();
//	    if (mBtn)
//	        mBtn.onClick.Add(new EventDelegate(OnClick));
	}

    void OnClick()
    {
//        Debug.Log("On Click");
        if (Prefab != null)
        {
            GameManager.instance.SetPickedPrefab(Prefab);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
