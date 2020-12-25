using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artillery : MonoBehaviour
{
    public GameObject projectile;
    public Transform firePoint;
    public GameObject fireEffect;
    public int countShoot;
    public float fireRate;
    public int fireInScene;
    public float firePower;
    public AudioSource fireAudio;
    public Animation[] canonAnimation;

    public void Fire()
    {
        if (countShoot < fireInScene && gameObject.activeSelf)
        {
            GameObject cloneprojectile = Instantiate(projectile, firePoint.transform.position, firePoint.transform.rotation);
            cloneprojectile.GetComponent<Rigidbody>().AddForce(cloneprojectile.transform.forward * 5000, ForceMode.Acceleration);
            fireEffect.GetComponent<ParticleSystem>().Play();
            cloneprojectile.GetComponent<projectile>().Artillery = this;
            cloneprojectile.GetComponent<projectile>().power = firePower;
            if (canonAnimation.Length > 0)
            {
                foreach (var item in canonAnimation)
                {
                    item.Play();
                }
            }
            if (fireAudio.isPlaying)
            {
                fireAudio.Stop();
                fireAudio.Play();
            }
            else { fireAudio.Play(); }
            countShoot++;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        fireAudio = GetComponent<AudioSource>();
        canonAnimation = gameObject.GetComponentsInChildren<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
