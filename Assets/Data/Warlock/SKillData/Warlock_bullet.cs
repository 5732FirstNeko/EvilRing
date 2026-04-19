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
        }

        bullethitEffects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(bullethitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            bullethitEffects.Add(effect);
        }
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (user.costumvalue_first <= 0 || unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        if (user.costumvalue_first > 8)
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

        Dictionary<UnitPlat, int> damageCountMap = new Dictionary<UnitPlat, int>();
        for (int i = 0; i < user.costumvalue_first;)
        {
            foreach (var unit in unitPlats)
            {
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

        TimerManager.instance.StartTimer(name + "firebullethitanimation", 1f + 0.6f,
            () =>
            {
                for (int i = 0; i < bulletEffects.Count; i++)
                {
                    int j = i;
                    bulletEffects[j].SetActive(false);
                }

                int index = 0;
                foreach (var unit in unitPlats)
                {
                    unit.unit.HP -= Damage * damageCountMap[unit];
                    unit.UnitPlatHurtAnimation();

                    bullethitEffects[index].transform.position = unit.transform.position;
                    bullethitEffects[index].SetActive(true);
                    bullethitEffects[index].GetComponent<PlayableDirector>().Play();
                    index++;
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

        user.costumvalue_first = 0;
        user.unit.unitSkills[skillListIndex].SkillTime = 1f + 0.6f + 0.3f + 0.6f;
    }
}
