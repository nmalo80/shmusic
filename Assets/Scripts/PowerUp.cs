using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.x > GameManager.Instance.GetScreenRightBound()
            || this.transform.position.x < GameManager.Instance.GetScreenLeftBound())
        {
            Destroy(gameObject);
        }
    }
}
