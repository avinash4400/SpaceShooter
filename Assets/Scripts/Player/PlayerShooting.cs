using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerShooting : MonoBehaviour, IGameComponent
{
    [Header("Dependencies")]
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private float fireRate = 0.2f;

    private IActor actor;
    private BulletInventory inventory;

    private bool isFiring = false;
    private float nextFireTime = 0f;

    public void Initialize(IActor actor)
    {
        this.actor = actor;
        inventory = actor.GetTransform().GetComponent<BulletInventory>();

        if (inventory == null)
        {
            Debug.LogError("[PlayerShooting] BulletInventory not found on Actor.");
        }
    }

    void OnEnable()
    {
        PlayerController.OnDashAttempt += OnShootingPerformed;
    }

    void OnDisable()
    {
        PlayerController.OnDashAttempt -= OnShootingPerformed;
        StopCoroutine(HandleContinuousFire());
        isFiring = false;
    }

    private void OnShootingPerformed()
    {
        if (!isFiring)
        {
            isFiring = true;
            StartCoroutine(HandleContinuousFire());
        }
    }

    private IEnumerator HandleContinuousFire()
    {
        if (Time.time >= nextFireTime)
        {
            TryFire();
            nextFireTime = Time.time + fireRate;
        }
        isFiring = false;
        yield return null;
    }

    private void TryFire()
    {
        if (inventory == null) return;

        // Delegate the entire firing logic (check ammo, consume, spawn) to the inventory
        // We pass the muzzle position and direction (Up for vertical shooter)
        inventory.AttemptFire(muzzlePoint.position, Vector3.up, actor.GetTransform().gameObject);
    }
}