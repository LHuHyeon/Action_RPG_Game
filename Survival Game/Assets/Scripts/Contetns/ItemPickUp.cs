using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public Item item;
    public int itemCount = 1;

    void Start()
    {
        Managers.UI.MakeWorldSpaceUI<UI_NameBar>(transform);
    }
}
