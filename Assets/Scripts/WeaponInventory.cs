using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    [Header("Assign your weapon GameObjects here (children of this object)")]
    [SerializeField] private GameObject pistol;
    [SerializeField] private GameObject rifle;

    void Start()
    {
        // ������ �� �⺻ ����(����)�� Ȱ��ȭ
        SelectWeapon(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectWeapon(0);   // 0 �� pistol
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectWeapon(1);   // 1 �� rifle
    }

    private void SelectWeapon(int idx)
    {
        // idx == 0 �̸� pistol��, idx == 1 �̸� rifle�� Ȱ��ȭ
        pistol.SetActive(idx == 0);
        rifle.SetActive(idx == 1);
    }
}
