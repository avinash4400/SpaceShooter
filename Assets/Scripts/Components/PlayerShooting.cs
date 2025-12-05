using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerShooting : MonoBehaviour, IGameComponent
{
    [Header("Muzzle Setup")]
    [Tooltip("Define all fire points on this ship.")]
    [SerializeField] private List<MuzzleDefinition> muzzles = new List<MuzzleDefinition>();


    private IActor actor;
    private BulletInventory inventory;

    private bool isFiring = false;
    private float nextFireTime = 0f;

    public void Initialize(IActor actor)
    {
        this.actor = actor;
        inventory = actor.GetAttachedComponent<BulletInventory>();

        if (inventory == null)
        {
            Debug.LogError("[PlayerShooting] BulletInventory not found on Actor.");
        }
    }

    void OnEnable()
    {
        PlayerController.OnShootInput += OnShootingInput;
    }

    void OnDisable()
    {
        PlayerController.OnShootInput -= OnShootingInput;
        StopCoroutine(HandleContinuousFire());
        isFiring = false;
    }

    private void OnShootingInput(bool isShooting)
    {
        if (isShooting)
        {
            if (!isFiring)
            {
                isFiring = true;
                StartCoroutine(HandleContinuousFire());
            }
        }
        else
        {
            isFiring = false;
        }
    }

    private IEnumerator HandleContinuousFire()
    {
        while (isFiring)
        {
            if (Time.time >= nextFireTime)
            {
                TryFire();

                float currentRate = 0.2f; // Default fallback
                if (inventory != null && inventory.SelectedBullet != null)
                {
                    currentRate = inventory.SelectedBullet.fireRate;
                }

                nextFireTime = Time.time + currentRate;
            }
            yield return null;
        }
    }

    private void TryFire()
    {
        if (inventory == null) return;

        inventory.AttemptFire(muzzles, Vector3.up, actor);
    }
}