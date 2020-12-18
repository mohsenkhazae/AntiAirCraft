using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    public Transform explosionPrefab;
    public Artillery Artillery;
    public ManageAirCraft manageAirCraft;
    void Start()
    {
        manageAirCraft = GameObject.FindObjectOfType<ManageAirCraft>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "target")
        {
            manageAirCraft.TargetLeft--;
            if (collision.gameObject.TryGetComponent(out TargetBuilding targetBuilding))
            {
                //targetBuilding.InUse = true;
                targetBuilding.building.SetActive(false);
                targetBuilding.crashBuilding.SetActive(true);
            }
        }
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        Instantiate(explosionPrefab, pos, rot);
        Destroy(gameObject);

    }
    void OnBecameInvisible()
    {
        if (Artillery)
        {
         Artillery.countShoot--;
        //Debug.Log("OnBecameInvisible");
        Destroy(gameObject); 
        }
    }
}
