using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Warlock_bullet))]
public class Warlock_bullet : UnitSkillDataSo
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bullethitPrefab;

    [SerializeField] private int skillListIndex;
    [SerializeField] private float firebuletSpeed;
    [SerializeField] private Vector3 bulletUpOffest;
    [SerializeField] private Vector3 bulletDownOffest;

    private List<GameObject> bulletEffects;
    private List<GameObject> bullethitEffects;

    private Dictionary<UnitPlat, int> damageCountMap;
    public override void GameStartInit()
    {
        base.GameStartInit();

        bulletEffects = new List<GameObject>();
        for (int i = 0; i < 8; i++)
        {
            GameObject effect = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
            effect.transform.localScale = Vector3.zero;
            effect.SetActive(false);
            bulletEffects.Add(effect);
            BattleSystem.instance.destoryEffect.Add(effect);
        }

        bullethitEffects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(bullethitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            bullethitEffects.Add(effect);
            BattleSystem.instance.destoryEffect.Add(effect);
        }

        damageCountMap = new Dictionary<UnitPlat, int>();
    }

    public override void GameEndAction()
    {
        bulletEffects = null;

        bullethitEffects = null;
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (user.costumvalue_first <= 0)
        {
            user.costumvalue_first = 2;
        }

        if (unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        if (user.costumvalue_first > bulletEffects.Count)
        {
            for (int i = 0; i < user.costumvalue_first - 8; i++)
            {
                GameObject effect = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
                effect.transform.localScale = Vector3.zero;
                effect.SetActive(false);
                bulletEffects.Add(effect);
            }
        }

        float offest = 0.25f;
        for (int i = 0; i < user.costumvalue_first;)
        {
            foreach (var unit in unitPlats)
            {
                if (unit.isDead ||
                    unit.unitData == FactorySystem.instance.EmptyHostitlyUnitData)
                {
                    continue;
                }

                int index = i;

                if (i % 2 == 0)
                {
                    float height = offest;
                    Vector3[] bulletPath = new Vector3[]
                    {
                        user.transform.position,
                        user.transform.position + new Vector3(-1.5f, height, 0),
                        unit.transform.position,
                    };

                    bulletEffects[index].transform.position = user.transform.position;
                    bulletEffects[index].SetActive(true);
                    bulletEffects[index].transform.DOPath(bulletPath, 1f, PathType.CatmullRom).
                        SetEase(Ease.InQuart).SetOptions(false).SetDelay(0.5f);
                }
                else
                {
                    float height = offest;
                    Vector3[] bulletPath = new Vector3[]
                    {
                        user.transform.position,
                        user.transform.position + new Vector3(-1.5f, -height, 0),
                        unit.transform.position,
                    };

                    bulletEffects[index].transform.position = user.transform.position;
                    bulletEffects[index].SetActive(true);
                    bulletEffects[index].transform.DOPath(bulletPath, 1f, PathType.CatmullRom).
                        SetEase(Ease.InQuart).SetOptions(false).SetDelay(0.5f);
                }
                offest += 0.25f;
                i++;

                if (i >= user.costumvalue_first)
                {
                    break;
                }
            }

        }

        damageCountMap.Clear();
        for (int i = 0; i < user.costumvalue_first;)
        {
            foreach (var unit in unitPlats)
            {
                if (unit.isDead || unit.unitData == FactorySystem.instance.EmptyHostitlyUnitData)
                {
                    continue;
                }

                if (damageCountMap.ContainsKey(unit))
                {
                    damageCountMap[unit]++;
                }
                else
                {
                    damageCountMap.Add(unit, 1);
                }
                i++;

                if (i >= user.costumvalue_first)
                {
                    break;
                }
            }
        }

        TimerManager.instance.StartTimer($"firebullethitanimation_{user.GetInstanceID()}", 1f + 0.6f,
            () =>
            {
                if (bulletEffects == null || unitPlats == null || damageCountMap == null) return;

                foreach (var bullet in bulletEffects)
                {
                    if (bullet != null) 
                    {
                        bullet.SetActive(false); 
                        bullet.transform.DOKill(); 
                    }
                }

                foreach (var unit in unitPlats)
                {
                    if (unit == null || 
                        unit.unitData == FactorySystem.instance.EmptyHostitlyUnitData)
                    {
                        continue;
                    }
                    
                    if (damageCountMap.TryGetValue(unit, out int damage))
                    {
                        unit.unit.HP -= 8 * damage;
                        unit.UnitPlatHurtAnimation();
                    }
                }

                int hitIndex = 0;
                foreach (var unit in unitPlats)
                {
                    if (hitIndex > bullethitEffects.Count ||
                        hitIndex >= user.costumvalue_first)
                    {
                        break; 
                    }

                    if (unit.isDead || 
                        unit.unitData == FactorySystem.instance.EmptyHostitlyUnitData)
                    {
                        continue;
                    }

                    var eff = bullethitEffects[hitIndex];
                    eff.transform.position = unit.transform.position;
                    eff.SetActive(true);
                    eff.GetComponent<PlayableDirector>()?.Play();
                    hitIndex++;
                }
            });


        float hitduration = (float)bullethitEffects[0].GetComponent<PlayableDirector>().duration;
        TimerManager.instance.StartTimer(name + "fireBulletClose", 1f + 0.6f + hitduration, 
            () => 
            {
                foreach (var effect in bullethitEffects)
                {
                    effect.SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1f,0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.costumvalue_first = 2;
        user.unit.unitSkills[skillListIndex].SkillTime = 1f + 0.6f + 0.3f + 0.6f;
    }
}
