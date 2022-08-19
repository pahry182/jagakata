using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPageTextHandler : MonoBehaviour
{
    // Start is called before the first frame update

    private void Awake()
    {
        RemoveAllChild();
    }

    public void RemoveAllChild()
    {
        foreach (Transform item in transform)
        {
            Destroy(item.gameObject);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
