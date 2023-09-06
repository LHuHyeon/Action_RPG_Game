using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BossHpBar : UI_Base
{
    Stat _stat;

    enum GameObjects
    {
        HPBar
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        _stat = transform.parent.GetComponent<Stat>();
        gameObject.transform.SetParent(null);
        GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        gameObject.SetActive(false);
    }

    void Update()
    {
        float ratio = (float)_stat.Hp / _stat.MaxHp;
        SetHpRatio(ratio);
    }

    void SetHpRatio(float ratio)
    {
        GetObject((int)GameObjects.HPBar).GetComponent<Slider>().value = ratio;
    }
}
