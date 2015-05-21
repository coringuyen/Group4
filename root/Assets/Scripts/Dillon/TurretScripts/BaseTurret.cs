﻿using UnityEngine;
using System.Collections;

public class BaseTurret : MonoBehaviour 
{
    public GameObject bullet; //prefab for the bullet
    public GameObject turret;

    public GameObject target;
    public Transform[] barrelPos; //refrence to the pos of the barrel
    public Transform[] turretTop; //refrence to the pos of the top of the turret

    public float rotationSpeed = 5.0f; //Sets the rotation speed for the turret to travel to get to the targets position
    public float reloadSpeed = 5.0f; //The time for the turret to replinish its ammo (also will be a rest time no turret movement)
    public float rateOfFire = .25f; //how fast the turret will fire

    public int maxAmmo = 100; //how mmuch ammo the turret can hold
    public int currentAmmo = 100; //how many shots the turret has is decressed by one for every shot unless the turret has multiple barrels
    public int maxHP = 100;  // max amount of HP the turret can have at any moment
    public int currentHP = 100;   //is decreased by a certain amount when damage is taken and will vary form enemy to enemy how much damage is taken

    public bool isTargetInRadius = false; //checks to see if the target is in the radius of the turret
    public bool isReloading = false;

    public bool validTarget = false;
    private Quaternion rotationToGoal;

    public float fireDelay;
    public float reloadTime;

    public void OnTriggerEnter(Collider c)
    {
        if(c.gameObject.tag == "Target")
        {
            validTarget = true;
            target = c.gameObject;
            isTargetInRadius = true; 
        }
    }

    public void OnTriggerExit()
    {
        isTargetInRadius = false;
        turret.GetComponent<FieldOfView>().isTargetInView = false;
    }

    void distanceToTarget(Vector3 targetPos)
    {
        Vector3 aimPoint = new Vector3(targetPos.x, targetPos.y, targetPos.z);
        aimPoint.y -= target.transform.localScale.y / 2;
        rotationToGoal = Quaternion.LookRotation(aimPoint - transform.position);
        /*
            used to calculate the distance the turret must rotate till it reaches its targets position
         */
    }

    void bulletFire()
    {
        if(currentAmmo != 0 && turret.GetComponent<FieldOfView>().isTargetInView == true)
        {
            fireDelay = Time.time + rateOfFire;

            bullet.GetComponent<BulletMove>().isFired = true;
            currentAmmo -= 1;

            foreach (Transform theBarrelPos in barrelPos) 
            {
                Instantiate(bullet, theBarrelPos.position, theBarrelPos.rotation);
                print("Shoot");
            }

            if(currentAmmo == 0)
            {
                isReloading = true;
                turret.GetComponent<FieldOfView>().isTargetInView = false;
            }
            /*
                set a delay for the RateOfFire and this loops through each of the posistions of the 
             *  turrets barrels to spawn a new bullet in the barrel to be fired agian
             */
        }
    }

    void turretReload()
    {
        reloadTime = Time.time + reloadSpeed;
        currentAmmo = maxAmmo;
        isReloading = false;
        turret.GetComponent<FieldOfView>().isTargetInView = true;
   
        /*
  * checks to see if the turret has bullets to fire, if not it reloads.
  * if it has bullets then the turret will begin to fire at the target.
  */
    }

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () 
    {
            if (isTargetInRadius == true && isReloading == false && validTarget == true)
            {
                distanceToTarget(target.transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotationToGoal, Time.deltaTime * rotationSpeed);

                if (Time.time > fireDelay)
                {
                    bulletFire();
                }
                /*
                    when the target comes into the radius of the turret the turret will begin to rotate till 
                 * the target is in its field of view and once it comes into the field of view it will begin to fire
                 */
            }

        
        if(isReloading == true)
        {
            if(Time.time > reloadTime)
            {
                turretReload();
                print("Reloading");
            }
        }
	}
}