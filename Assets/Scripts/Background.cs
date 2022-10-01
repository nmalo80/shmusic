using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    // BG layers
    [SerializeField] float[] layerSpeeds = null;
    [SerializeField] GameObject[] layers = null;

    private Vector2 movementFromBase = new Vector2(0.0f, 0.0f);
    private Vector2 baseLocation;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
        if (layers.Length > 0)
        {
            int i = 0;
            foreach (GameObject layer in layers)
            {
                float offset = layerSpeeds[i] / 3 * Time.deltaTime;
                var material = layer.GetComponent<Renderer>().material;
                material.mainTextureOffset += new Vector2(offset, 0);

                //material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
                i++;
            }
        }
    }
}
