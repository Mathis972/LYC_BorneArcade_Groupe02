﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpaceShip : MonoBehaviour
{
    private int id;
    //private int lifePoints = 100;
    private HealthSystem healthSystem = new HealthSystem(100);
    public int movementSpeed = 100;
    public int projectileSpeed;
    private string color;
    public int shieldUpTime;
    private float nextFireTime = 0;
    private int maxAmmo = 10;
    private int currentAmmo;
    private int reloadTime = 5;
    private bool isReloading = false;
    private bool isShielded = false;
    public int shieldCD;
    public Transform pfhealthBar;

    public Shield shield;

    public Projectile projectile;
    public Transform shootingPoint;

    public Text ammoText;
    public Text maxAmmoText;
    public Text ShieldRemainingCooldownText;

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = maxAmmo;

        Transform healthBarTransform = Instantiate(pfhealthBar, transform.position + transform.right * -1.1f, Quaternion.Euler(0, 0, 90));

        healthBarTransform.parent = transform;
        HealthBar healthBar = healthBarTransform.GetComponent<HealthBar>();

        healthBar.Setup(healthSystem);

        //Physics.IgnoreLayerCollision(8, 8);

    }

    // Update is called once per frame
    void Update()
    {
        Move(movementSpeed);

        var remainingTime = nextFireTime - Time.time;
        if (remainingTime <= 0)
        {
            ShieldRemainingCooldownText.fontSize = 15;
            ShieldRemainingCooldownText.text = "Shield Up";
        }
        else
        {
            ShieldRemainingCooldownText.fontSize = 22;
            ShieldRemainingCooldownText.text = remainingTime.ToString("0.0");
        }

        ammoText.text = currentAmmo.ToString();
        maxAmmoText.text = maxAmmo.ToString();

        if (isReloading)
        {
            if (Input.GetButtonUp("Fire2"))
            {
                StartCoroutine(Protect());
            }
            else
            {
                return;
            }

        }
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
        if (Input.GetButtonUp("Fire2"))
        {
            StartCoroutine(Protect());
        }


    }
    void Shoot()
    {
        currentAmmo--;

        Projectile projectileObject = Instantiate(projectile, shootingPoint.position, shootingPoint.rotation);
        projectileObject.setSpeed(projectileSpeed);
    }
    IEnumerator Reload()
    {

        isReloading = true;
        Debug.Log("Reloading..");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
    }
    IEnumerator Protect()
    {
        if (Time.time > nextFireTime)
        {
            //GameObject prefab = Resources.Load("bouclier") as GameObject;

            Quaternion rotation = transform.rotation;
            var position = transform.position;
            //GameObject shieldObject = Instantiate(prefab, position, rotation);
            Shield shieldObject = Instantiate(shield, position, rotation);
            shieldObject.transform.parent = transform;
            //Shield shield = shieldObject.GetComponent<Shield>();
            shieldObject.setUpTime(shieldUpTime);
            shieldObject.tag = "Shield";
            isShielded = true;
            nextFireTime = Time.time + shieldCD;
            yield return new WaitForSeconds(shieldUpTime);
            isShielded = false;
        } else
        {
            Debug.Log("Ya pas shield");
        }

    }
    public void TakeDamage(int damage)
    {
        if(!isShielded)
        {
            healthSystem.Damage(damage);
            if (healthSystem.GetHealth() == 0)
            {
                Die();
            }
        }

    }
    void Die()
    {
        Destroy(gameObject);
    }

    public void Move(int speed)
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, vertical, 0.0f);
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.15F);
        }
        transform.Translate(direction * Time.deltaTime * speed, Space.World);
    }
}