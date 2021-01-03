using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticle : MonoBehaviour
{
    public float delayTime=6;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(destroy());
    }
    IEnumerator destroy()
    {
        yield return new WaitForSeconds(delayTime);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
