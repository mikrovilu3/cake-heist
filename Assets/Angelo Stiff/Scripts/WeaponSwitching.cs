using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    public int weaponSelected;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        int previousSelectedWeapon = weaponSelected;
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (weaponSelected >= transform.childCount - 1)
                weaponSelected = 0;
            else
                weaponSelected++;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (weaponSelected <= 0)
                weaponSelected = transform.childCount - 1;
            else
                weaponSelected--;
        }
        if (previousSelectedWeapon != weaponSelected)
        {
            SelectWeapon();
        }
    }

    void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == weaponSelected)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }
}