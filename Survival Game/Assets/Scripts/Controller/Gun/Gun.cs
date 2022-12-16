using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "New Item/Gun")]
public class Gun : ScriptableObject
{
    public Item bullt;              // 사용 총알 아이템
    public int damage;              // 데미지
    public int maxBulletCount;      // 최대 총알
    public int currentBulletCount;  // 현재 총알
    public float reloadTime;        // 장전 시간
    public float accuracy;          // 총기 반동
    public float fireRange;         // 발사 거리
    public float fireTime;          // 발사 속도
    public AudioClip fireAudio;     // 발사 소리
}
