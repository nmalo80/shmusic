using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField] float SpeedOfSpinX = 0f;
    [SerializeField] float SpeedOfSpinY = 0f;
    [SerializeField] float SpeedOfSpinZ = 0f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(SpeedOfSpinX*Time.deltaTime, SpeedOfSpinY*Time.deltaTime, SpeedOfSpinZ * Time.deltaTime)); 
    }
}
