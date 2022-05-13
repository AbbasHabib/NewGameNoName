using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunBar : MonoBehaviour
{
    [SerializeField]private Image gunHolder;

    private void Start()
    {
        gunHolder.enabled = false;
    }
    public void ChangeUIWeapon(Sprite wep)
    {
        gunHolder.enabled = true;
        gunHolder.sprite = wep;
    }
}
