using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation *= Quaternion.Euler(new Vector3(3 * Time.deltaTime, 0, 0));
    }
}
