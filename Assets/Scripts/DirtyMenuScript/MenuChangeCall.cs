using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuChangeCall : MonoBehaviour
{
    public int i;

    public void OnButtonClicked()
    {
        MenuPlantChange.Instance.StartChange(i);
    }
}
