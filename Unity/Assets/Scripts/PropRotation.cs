using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRotation : MonoBehaviour
{
    [SerializeField] int direction;
    // Update is called once per frame
    void Update()
    {
        if (direction == 1){
            transform.Rotate(0f, 1000f, 0f, Space.Self);

        }
        else{
            transform.Rotate(0f, -1000f, 0f, Space.Self);
        }
    }
}
