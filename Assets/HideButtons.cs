using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideButtons : MonoBehaviour
{
    public GameObject ob;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HideElements()
    {
        if (ob != null)
        {
            ob.SetActive(false);
        }
    }
}
