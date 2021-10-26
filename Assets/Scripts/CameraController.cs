using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            //Debug.Log("W pressed");
            transform.Translate(new Vector3(0, 0.1f, 0));
        }
        if (Input.GetKey(KeyCode.A))
        {
            //Debug.Log("A pressed");
            transform.Translate(new Vector3(-0.1f, 0, 0));
        }
        if (Input.GetKey(KeyCode.S))
        {
            //Debug.Log("S pressed");
            transform.Translate(new Vector3(0, -0.1f, 0));
        }
        if (Input.GetKey(KeyCode.D))
        {
            //Debug.Log("D pressed");
            transform.Translate(new Vector3(0.1f, 0, 0));
        }

    }
}
