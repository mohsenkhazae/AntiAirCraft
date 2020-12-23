using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    public Transform explosionPrefab;
    public Artillery Artillery;
    public float power;

    Collider objCollider;
    Plane[] planes;
    // Start is called before the first frame update
    void Start()
    {
        planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        objCollider = GetComponent<Collider>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "AirCraft")
        {
            AirCraft air = collision.collider.gameObject.GetComponent<AirCraft>();
            air.manageAirCraft.Lines[air.currentLine] = false;
            air.health -= power;
            if (air.health == 0)
            {
                Destroy(collision.gameObject);
            }
        }
        if (Artillery)
        {
            Artillery.countShoot--;
        }
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        Instantiate(explosionPrefab, pos, rot);
        Destroy(gameObject);
    }
    //void OnBecameInvisible()
    //{
    //    if (Artillery)
    //    {
    //        Artillery.countShoot--;
    //        Debug.Log("OnBecameInvisible");
    //        Destroy(gameObject);
    //    }
    //}
    // Update is called once per frame
    void Update()
    {
        if (!GeometryUtility.TestPlanesAABB(planes, objCollider.bounds))
        {
            Debug.Log("Nothing has been detected");
            if (Artillery)
            {
                Artillery.countShoot--;
                Destroy(gameObject);
            }
        }
    }
}
