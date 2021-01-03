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

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "target")
        {
            manageAirCraft.TargetLeft--;
            if (collision.gameObject.TryGetComponent(out TargetBuilding targetBuilding))
            {
                StartCoroutine(ChangeBuilding(targetBuilding));
            }
        }
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        Instantiate(explosionPrefab, pos, rot);
    }

    IEnumerator ChangeBuilding(TargetBuilding targetBuilding)
    {
        yield return new WaitForSeconds(.2f);
        if (targetBuilding.building) targetBuilding.building.SetActive(false);
        if (targetBuilding.crashBuilding) targetBuilding.crashBuilding.SetActive(true);
        Destroy(gameObject);
        yield break;
    }
}
