using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 오브젝트 위에 표실될 체력바 클래스
public class UI_HPBar : UI_Base
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
        gameObject.SetActive(false);
    }

    void Update()
    {
        // 체력 설정
        Transform parent = transform.parent;
        transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y * 0.1f);
        GetObject((int)GameObjects.HPBar).transform.rotation = Camera.main.transform.rotation;

        float ratio = (float)_stat.Hp / _stat.MaxHp;
        SetHpRatio(ratio);
    }

    void SetHpRatio(float ratio)
    {
        GetObject((int)GameObjects.HPBar).GetComponent<Slider>().value = ratio;
    }
}
