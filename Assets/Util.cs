using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Util : MonoBehaviour
{
    public TextMeshProUGUI fps;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fps.text = 1 / Time.deltaTime + "";
    }
}
