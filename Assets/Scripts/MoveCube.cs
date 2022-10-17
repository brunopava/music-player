using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCube : MonoBehaviour
{
    private float positionCube = 0;
        // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        positionCube++;
        transform.Translate(new Vector3(0,0,positionCube*Time.deltaTime));  
    }
}
