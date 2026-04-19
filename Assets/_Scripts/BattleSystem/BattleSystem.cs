using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DG.Tweening;
using PriorityQueue;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public static int unitPlatQueueCount = 4;

    public static BattleSystem Instance;
    public static BattleSystem instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject Object = new GameObject(typeof(BattleSystem).Name);
                Instance = Object.AddComponent<BattleSystem>();
                DontDestroyOnLoad(Object);
            }
            return Instance;
        }
    }

    [Header("Friendly")]
    public List<UnitSiteFlag> friendlyUnitSiteFlag;

    [Header("Hostitly")]
    public List<UnitSiteFlag> hostilityUnitSiteFlag;

    [Header("UnitPlayMoveTime")]
    [SerializeField] private float unitPlatMoveTime;

    /// <summary>
    /// All State use Value
    /// </summary>
    public UnitPlatQueue FriendlyUnitPlatsQueue { get; private set; }

    /// <summary>
    /// All State use Value
    /// </summary>
    public UnitPlatQueue HostilityUnitPlatsQueue { get; private set; }

    public Func<float> OnGameStart;
    public Func<int,float> OnRoundStart;
    public Func<int,float> OnRound;
    public Func<int,float> OnRoundEnd;

    public Action<UnitPlat> OnUnitplatDequeue;

    public Action OnGameEnd;

    private PriorityQueue<UnitPlat> battleQueue;
    private LinkedList<UnitPlat> dequeueList;

    private Dictionary<UnitSkill,Unit> OnGameStartSkills;
    private Dictionary<UnitSkill,Unit> OnRoundStartSkills;
    private Dictionary<UnitSkill,Unit> OnRoundEndSkills;
    private Dictionary<UnitSkill,Unit> OnStrikeBackSkills;

    private Dictionary<Unit, int> OnStrikeBackMap;
    private Dictionary<UnitBuff, List<UnitPlat>> UnitBuffMap;

    private Coroutine battleIEnumerator;

    public int friendlyDeadCount;
    public int hostilityDeadCount;

    public int currentRound = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        battleQueue = new PriorityQueue<UnitPlat>(true);
        dequeueList = new LinkedList<UnitPlat>();

        OnGameStartSkills = new Dictionary<UnitSkill, Unit>();
        OnRoundStartSkills = new Dictionary<UnitSkill, Unit>();
        OnRoundEndSkills = new Dictionary<UnitSkill, Unit>();
        OnStrikeBackSkills = new Dictionary<UnitSkill, Unit>();

        OnStrikeBackMap = new Dictionary<Unit, int>();
        UnitBuffMap = new Dictionary<UnitBuff, List<UnitPlat>>();

        FriendlyUnitPlatsQueue = new UnitPlatQueue();
        HostilityUnitPlatsQueue = new UnitPlatQueue();

        friendlyDeadCount = 0;
        hostilityDeadCount = 0;
    }

    public void UnitSiteChange(Faction faction, UnitSite fromSite, UnitSite toSite)
    {
        //TODO : UnitMove Animation make

        switch (faction)
        {
            case Faction.Friendly:
                List<UnitPlat> friendlyplats = FriendlyUnitPlatsQueue.UnitPlatSiteChange(fromSite, toSite);

                friendlyplats[0].transform.DOMoveX(friendlyUnitSiteFlag[0].transform.position.x, unitPlatMoveTime);
                friendlyplats[1].transform.DOMoveX(friendlyUnitSiteFlag[1].transform.position.x, unitPlatMoveTime);
                friendlyplats[2].transform.DOMoveX(friendlyUnitSiteFlag[2].transform.position.x, unitPlatMoveTime);
                friendlyplats[3].transform.DOMoveX(friendlyUnitSiteFlag[3].transform.position.x, unitPlatMoveTime);

                break;
            case Faction.Hostility:
                List<UnitPlat> hostitlyplats = HostilityUnitPlatsQueue.UnitPlatSiteChange(fromSite, toSite);

                hostitlyplats[0].transform.DOMoveX(hostilityUnitSiteFlag[0].transform.position.x, unitPlatMoveTime);
                hostitlyplats[1].transform.DOMoveX(hostilityUnitSiteFlag[1].transform.position.x, unitPlatMoveTime);
                hostitlyplats[2].transform.DOMoveX(hostilityUnitSiteFlag[2].transform.position.x, unitPlatMoveTime);
                hostitlyplats[3].transform.DOMoveX(hostilityUnitSiteFlag[3].transform.position.x, unitPlatMoveTime);

                break;
        }
    }

    public void UnitResurrection(UnitPlat unitPlat)
    {
        unitPlat.isDead = false;
        unitPlat.iconSpriteRender.DOColor(Color.white, 1f).SetEase(Ease.OutQuart);
        unitPlat.unit.HP = Mathf.RoundToInt(unitPlat.unit.HP * 0.5f);
        battleQueue.Enqueue(unitPlat, unitPlat.unit.speed);

        switch (unitPlat.unit.faction)
        {
            case Faction.Friendly:
                friendlyDeadCount--;
                break;
            case Faction.Hostility:
                hostilityDeadCount--;
                break;
        }

        foreach (var skill in unitPlat.unit.unitSkills)
        {
            if (skill.TriggerTiming == TriggerTiming.OnGameStart)
            {
                OnGameStartSkills.Add(skill, unitPlat.unit);
            }
            if (skill.TriggerTiming == TriggerTiming.OnRoundStart)
            {
                OnRoundStartSkills.Add(skill, unitPlat.unit);
            }
            if (skill.TriggerTiming == TriggerTiming.OnRoundEnd)
            {
                OnRoundEndSkills.Add(skill, unitPlat.unit);
            }
            if (skill.TriggerTiming == TriggerTiming.OnStrikeBack)
            {
                OnStrikeBackSkills.Add(skill, unitPlat.unit);
                OnStrikeBackMap.Add(unitPlat.unit, unitPlat.unit.HP);
            }
        }
    }

    public void UnitDead(UnitPlat unitPlat)
    {
        unitPlat.isDead = true;
        if (battleQueue.Contains(unitPlat))
        {
            battleQueue.Remove(unitPlat);
        }
        switch (unitPlat.unit.faction)
        {
            case Faction.Friendly:
                friendlyDeadCount++;
                break;
            case Faction.Hostility:
                hostilityDeadCount++;
                break;
        }

        foreach (var skill in unitPlat.unit.unitSkills)
        {
            if (skill.TriggerTiming == TriggerTiming.OnGameStart)
            {
                OnGameStartSkills.Remove(skill);
            }
            if (skill.TriggerTiming == TriggerTiming.OnRoundStart)
            {
                OnRoundStartSkills.Remove(skill);
            }
            if (skill.TriggerTiming == TriggerTiming.OnRoundEnd)
            {
                OnRoundEndSkills.Remove(skill);
            }
            if (skill.TriggerTiming == TriggerTiming.OnStrikeBack)
            {
                OnStrikeBackSkills.Remove(skill);
                OnStrikeBackMap.Remove(unitPlat.unit);
            }
        }

        unitPlat.unit.DeadAction();
    }

    public void UnitReEnqueue(UnitPlat unitPlat)
    {
        battleQueue.Enqueue(unitPlat,unitPlat.unit.speed);
    }

    public void UnitRemoveQueue(UnitPlat unitPlat)
    {
        battleQueue.Remove(unitPlat);
    }

    #region BattleFunction
    public void BattleEnd()
    {
        UnitBuffMap.Clear();
        foreach (var unitplat in FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            unitplat.UnitPlatClear();
        }
        foreach (var unitplat in HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            unitplat.UnitPlatClear();
        }

        currentRound = -1;
        OnGameStart = null;
        OnRoundStart = null;
        OnRound = null;
        OnRoundEnd = null;
        OnGameStartSkills = null;
        OnGameEnd = null;

        battleIEnumerator = null;
    }

    public void BattleInit()
    {
        //TODO : Battle Start UI Animation

        Debug.Log("BattleStart");

        FriendlyUnitPlatsQueue.Clear();
        HostilityUnitPlatsQueue.Clear();

        List<UnitPlat> hostitlyPlats = UnitCardSystem.instance.GetFinalHostitlyUnitPlats();
        List<UnitPlat> friendlyPlats = UnitCardSystem.instance.GetFinalFriendlyUnitPlats();
        {
            for (int i = 0; i < 4; i++)
            {
                foreach (var skill in hostitlyPlats[i].unitData.Skills)
                {
                    skill.GameStartInit();
                }
                if (hostitlyPlats[i].unitData.SpKillData != null)
                {
                    hostitlyPlats[i].unitData.SpKillData.GameStartInit();
                }
                hostitlyPlats[i].unitData.UnitDeadData?.PrefabInit();
                HostilityUnitPlatsQueue.unitPlatQueue.Add(hostitlyPlats[i]);
            }
            for (int i = 0; i < 4; i++)
            {
                foreach (var skill in friendlyPlats[i].unitData.Skills)
                {
                    skill.GameStartInit();
                }
                friendlyPlats[i].unitData.UnitDeadData?.PrefabInit();
                FriendlyUnitPlatsQueue.unitPlatQueue.Add(friendlyPlats[i]);
            }
        }

        OnGameStartSkills.Clear();
        OnRoundStartSkills.Clear();
        OnRoundEndSkills.Clear();
        OnStrikeBackSkills.Clear();
        OnStrikeBackMap.Clear();
        friendlyDeadCount = 0;
        hostilityDeadCount = 0;


        foreach (var plat in FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            foreach (var skill in plat.unit.unitSkills)
            {
                if (skill.TriggerTiming == TriggerTiming.OnGameStart)
                {
                    OnGameStartSkills.Add(skill, plat.unit);
                }
                if (skill.TriggerTiming == TriggerTiming.OnRoundStart)
                {
                    OnRoundStartSkills.Add(skill, plat.unit);
                }
                if (skill.TriggerTiming == TriggerTiming.OnRoundEnd)
                {
                    OnRoundEndSkills.Add(skill, plat.unit);
                }
                if (skill.TriggerTiming == TriggerTiming.OnStrikeBack)
                {
                    OnStrikeBackSkills.Add(skill, plat.unit);
                    OnStrikeBackMap.Add(plat.unit, plat.unit.HP);
                }
            }
        }
        foreach (var plat in HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            foreach (var skill in plat.unit.unitSkills)
            {
                if (skill.TriggerTiming == TriggerTiming.OnGameStart)
                {
                    OnGameStartSkills.Add(skill, plat.unit);
                }
                if (skill.TriggerTiming == TriggerTiming.OnRoundStart)
                {
                    OnRoundStartSkills.Add(skill, plat.unit);
                }
                if (skill.TriggerTiming == TriggerTiming.OnRoundEnd)
                {
                    OnRoundEndSkills.Add(skill, plat.unit);
                }
                if (skill.TriggerTiming == TriggerTiming.OnStrikeBack)
                {
                    OnStrikeBackSkills.Add(skill, plat.unit);
                    OnStrikeBackMap.Add(plat.unit, plat.unit.HP);
                }
            }
        }

        for (int i = 0; i < 4; i++)
        {
            foreach (var skill in hostitlyPlats[i].unitData.Skills)
            {
                skill.GameStartInit();
            }
            if (hostitlyPlats[i].unitData.SpKillData != null)
            {
                hostitlyPlats[i].unitData.SpKillData.GameStartInit();
            }
            hostitlyPlats[i].unitData.UnitDeadData?.PrefabInit();
        }
        for (int i = 0; i < 4; i++)
        {
            foreach (var skill in friendlyPlats[i].unitData.Skills)
            {
                skill.GameStartInit();
            }
            friendlyPlats[i].unitData.UnitDeadData?.PrefabInit();
        }
    }

    public void BattleFunction()
    {
        battleIEnumerator = StartCoroutine(Battle());
    }

    private IEnumerator Battle()
    {
        float ongameStarttime = OnGameStart?.Invoke() ?? 0;
        yield return new WaitForSeconds(ongameStarttime);

        yield return HPCheck(FriendlyUnitPlatsQueue.GetAllUnitPlat());
        yield return HPCheck(HostilityUnitPlatsQueue.GetAllUnitPlat());

        Debug.Log(friendlyDeadCount);
        Debug.Log(hostilityDeadCount);

        foreach (var plat in FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            battleQueue.Enqueue(plat, plat.unit.speed);
        }
        foreach (var plat in HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            battleQueue.Enqueue(plat, plat.unit.speed);
        }

        yield return LevelStartBattle();
        yield return AllBuffAction(TriggerTiming.OnGameStart);

        Debug.Log(battleQueue.Count);

        int safecount = 0;
        currentRound = 0;
        while (friendlyDeadCount < 4 && hostilityDeadCount < 4)
        {
            safecount++;
            if (safecount > 1000)
            {
                yield break;
            }
            yield return RoundStartBattle();
            yield return AllBuffAction(TriggerTiming.OnRoundStart);
            float onroundStarttime = OnRoundStart?.Invoke(currentRound) ?? 0;
            yield return new WaitForSeconds(onroundStarttime);

            while (battleQueue.Count > 0)
            {
                safecount++;
                if (safecount > 1000)
                {
                    yield break;
                }

                foreach (var unit in OnStrikeBackMap.Keys)
                {
                    OnStrikeBackMap[unit] = unit.HP;
                }

                UnitPlat unitPlat = battleQueue.Dequeue();
                Material litMaterial = unitPlat.iconSpriteRender.material;
                unitPlat.iconSpriteRender.material = GameManager.UnlitMaterial;
                if (!unitPlat.isDead)
                {
                    UnitSkill OnRoundSkill = unitPlat.unit.UnitSkillChoice();
                    if (OnRoundSkill != null)
                    {
                        ICollection<UnitPlat> targetPlats = GetActionTargetPlat(unitPlat.unit, OnRoundSkill);
                        OnRoundSkill.Action(targetPlats);

                        Debug.Log(unitPlat.name);
                        Debug.Log(unitPlat.unit.faction + " " + unitPlat.site + " Attack ");
                        foreach (var tar in targetPlats)
                        {
                            Debug.Log(tar.unit.faction + " " + tar.site);
                        }
                        Debug.Log("----------------");

                        foreach (var buff in OnRoundSkill.UnitBuffs)
                        {
                            AddBuffToUnitPlat(buff, targetPlats);
                        }

                        yield return new WaitForSeconds(OnRoundSkill.SkillTime);
                    }
                    yield return AllBuffAction(TriggerTiming.OnRound);
                    unitPlat.iconSpriteRender.material = litMaterial;
                    dequeueList.AddLast(unitPlat);
                    OnUnitplatDequeue?.Invoke(unitPlat);
                    Debug.Log(hostilityDeadCount);
                }

                yield return HPCheck(FriendlyUnitPlatsQueue.GetAllUnitPlat());
                yield return HPCheck(HostilityUnitPlatsQueue.GetAllUnitPlat());

                yield return StrikeBackBattle();
                yield return AllBuffAction(TriggerTiming.OnStrikeBack);
            }

            yield return RoundEndBattle();
            yield return AllBuffAction(TriggerTiming.OnRoundEnd);
            float onRoundEndtime = OnRound?.Invoke(currentRound) ?? 0;
            yield return new WaitForSeconds(onRoundEndtime);

            battleQueue.Clear();
            foreach (var plat in FriendlyUnitPlatsQueue.GetAllUnitPlat())
            {
                if(plat.isDead) continue;
                battleQueue.Enqueue(plat, plat.unit.speed);
            }
            foreach (var plat in HostilityUnitPlatsQueue.GetAllUnitPlat())
            {
                battleQueue.Enqueue(plat, plat.unit.speed);
            }
            currentRound++;
        }
    }

    private IEnumerator AllBuffAction(TriggerTiming timing)
    {
        List<UnitBuff> removeBuff = new List<UnitBuff>();
        foreach (var (buff, plats) in UnitBuffMap)
        {
            if (buff.TriggerTiming != timing)
                continue;

            ICollection<UnitPlat> units = plats;
            buff.Action(units);
            foreach (var plat in plats)
            {
                int continueRound = plat.BuffConsumption(buff);
                if (continueRound <= 0)
                {
                    removeBuff.Add(buff);
                }
            }

            yield return HPCheck(plats);
            yield return new WaitForSeconds(buff.BuffActionTime);
        }

        for (int i = removeBuff.Count - 1; i >= 0; i--)
        {
            UnitBuffMap.Remove(removeBuff[i]);
        }
    }

    private IEnumerator LevelStartBattle()
    {
        foreach (var (gameStartSkill, unit) in OnGameStartSkills)
        {
            ICollection<UnitPlat> unitPlats = GetActionTargetPlat(unit, gameStartSkill);
            if (unitPlats != null)
            {
                gameStartSkill.Action(unitPlats);
                yield return HPCheck(unitPlats);

                foreach (var buff in gameStartSkill.UnitBuffs)
                {
                    AddBuffToUnitPlat(buff,unitPlats);
                }

                yield return new WaitForSeconds(gameStartSkill.SkillTime);
            }

        }
    }

    private IEnumerator RoundStartBattle()
    {
        foreach (var (roundStartSkill, unit) in OnRoundStartSkills.ToList())
        {
            ICollection<UnitPlat> unitPlats = GetActionTargetPlat(unit, roundStartSkill);
            if (unitPlats != null)
            {
                roundStartSkill.Action(unitPlats);
                yield return HPCheck(unitPlats);

                foreach (var buff in roundStartSkill.UnitBuffs)
                {
                    AddBuffToUnitPlat(buff, unitPlats);
                }

                yield return new WaitForSeconds(roundStartSkill.SkillTime);
            }

        }
    }

    private IEnumerator RoundEndBattle()
    {
        foreach (var (roundEndSkill, unit) in OnRoundEndSkills)
        {
            ICollection<UnitPlat> unitPlats = GetActionTargetPlat(unit, roundEndSkill);
            if (unitPlats != null)
            {
                roundEndSkill.Action(unitPlats);
                yield return HPCheck(unitPlats);

                foreach (var buff in roundEndSkill.UnitBuffs)
                {
                    AddBuffToUnitPlat(buff, unitPlats);
                }
                yield return new WaitForSeconds(roundEndSkill.SkillTime);
            }

        }
    }

    private IEnumerator StrikeBackBattle()
    {
        foreach (var (unit, HP) in OnStrikeBackMap)
        {
            if (unit.HP < HP)
            {
                UnitSkill strikeBackSkill = null;
                foreach (var skill in unit.unitSkills)
                {
                    if (skill.TriggerTiming == TriggerTiming.OnStrikeBack)
                    {
                        strikeBackSkill = skill;
                    }
                }
                ICollection<UnitPlat> targetPlats = GetActionTargetPlat(unit, strikeBackSkill);
                strikeBackSkill.Action(targetPlats);
                yield return HPCheck(targetPlats);

                foreach (var buff in strikeBackSkill.UnitBuffs)
                {
                    AddBuffToUnitPlat(buff, targetPlats);
                }

                yield return new WaitForSeconds(strikeBackSkill.SkillTime);
            }
        }
    }

    #endregion

    private IEnumerator HPCheck(ICollection<UnitPlat> unitPlats)
    {
        float maxDeadTime = 0;
        foreach (var plat in unitPlats)
        {
            if (plat.isDead) continue;

            if (plat.unit.HP <= 0)
            {
                UnitDead(plat);

                if (hostilityDeadCount >= unitPlatQueueCount)
                {
                    //TODO : Game Win Logic
                    OnGameEnd?.Invoke();
                    if (hostilityDeadCount >= unitPlatQueueCount)
                    {
                        Debug.Log("win");
                        StopAllCoroutines();
                        BattleEnd();
                        GameManager.instance.GameBattleEnd(true);
                    }
                }

                if (friendlyDeadCount >= unitPlatQueueCount)
                {
                    //TODO : Game Lose Logic
                    OnGameEnd?.Invoke();
                    if (hostilityDeadCount >= unitPlatQueueCount)
                    {
                        StopAllCoroutines();
                        BattleEnd();
                        GameManager.instance.GameBattleEnd(false);
                    }
                }

                if (maxDeadTime < plat.unit.DeadAnimationTime)
                {
                    maxDeadTime = plat.unit.DeadAnimationTime;
                }
            }
        }

        yield return maxDeadTime;
    }

    private void AddBuffToUnitPlat(UnitBuff buff, ICollection<UnitPlat> plats)
    {
        foreach (var plat in plats)
        {
            plat.AddBuff(buff);
        }

        foreach (var (buf, plat) in UnitBuffMap)
        {
            if (buf.Equals(buff))
            {
                plat.AddRange(plats);
                return;
            }
        }

        UnitBuffMap.Add(buff, plats.ToList());
    }

    private ICollection<UnitPlat> GetActionTargetPlat(Unit self, UnitSkill skill)
    {
        Debug.Log(self.faction + " " + skill.SkillTarget);

        if (self.faction == Faction.Friendly)
        {
            if (skill.SkillTarget == Faction.Friendly)
            {
                return FriendlyUnitPlatsQueue.GetUnitPlats(skill.Range);
            }
            else
            {
                return HostilityUnitPlatsQueue.GetUnitPlats(skill.Range);
            }
        }
        else if(self.faction == Faction.Hostility)
        {
            if (skill.SkillTarget == Faction.Friendly)
            {
                return HostilityUnitPlatsQueue.GetUnitPlats(skill.Range);
            }
            else
            {
                return FriendlyUnitPlatsQueue.GetUnitPlats(skill.Range);
            }
        }

        return null;
    }

    private ICollection<Unit> GetActionTarget(ICollection<UnitPlat> unitCollection)
    {
        List<Unit> units = new List<Unit>();
        foreach (var plat in unitCollection)
        {
            units.Add(plat.unit);
        }

        return units;
    }

    private ICollection<Unit> GetActionTarget(Unit self, UnitSkill skill)
    {
        List<Unit> units = new List<Unit>();
        foreach (var plat in GetActionTargetPlat(self, skill))
        {
            units.Add(plat.unit);
        }

        return units;
    }

    public ICollection<UnitPlat> GetActionPlats(Faction faction, ICollection<UnitSite> range)
    {
        List<UnitPlat> reslutes = new List<UnitPlat>();
        switch (faction)
        {
            case Faction.Friendly:
                foreach (var ran in range)
                {
                    int index = GetIndexByUnitSite(ran);
                    reslutes.Add(FriendlyUnitPlatsQueue.GetUnitPlatByUnitSite(ran).plat);
                }
                break;
            case Faction.Hostility:
                foreach (var ran in range)
                {
                    int index = BattleSystem.GetIndexByUnitSite(ran);
                    reslutes.Add(HostilityUnitPlatsQueue.GetUnitPlatByUnitSite(ran).plat);
                }
                break;
        }

        return reslutes;
    }

    public static int GetIndexByUnitSite(UnitSite site)
    {
        switch (site)
        {
            case UnitSite.first:
                return 0;
            case UnitSite.second:
                return 1;
            case UnitSite.third:
                return 2;
            case UnitSite.fourth:
                return 3;
            default:
                return -1;
        }
    }

    public static UnitSite GetUnitSiteByIndex(int index)
    {
        switch (index)
        {
            case 0:
                return UnitSite.first;
            case 1:
                return UnitSite.second;
            case 2:
                return UnitSite.third;
            case 3:
                return UnitSite.fourth;
            default :
                return UnitSite.under;
        }
    }

    public UnitSite GetUnitSiteByUnit(Faction faction, Unit unit)
    {
        switch (faction)
        {
            case Faction.Friendly:
                return FriendlyUnitPlatsQueue.GetUnitSite(unit);
            case Faction.Hostility:
                return HostilityUnitPlatsQueue.GetUnitSite(unit);
            default:
                return UnitSite.under;
        }
    }

    public class UnitPlatQueue
    {
        public List<UnitPlat> unitPlatQueue;

        public List<UnitPlat> UnitPlatSiteChange(UnitSite fromUnitSite, UnitSite toUnitSite)
        {
            UnitPlat fromUnit = GetUnitPlatByUnitSite(fromUnitSite).plat;
            int insertIndex = GetUnitPlatByUnitSite(toUnitSite).index;

            unitPlatQueue.Remove(fromUnit);
            unitPlatQueue.Insert(insertIndex, fromUnit);

            for (int i = 0; i < 4; i++)
            {
                unitPlatQueue[i].site = GetUnitSiteByIndex(i);
            }
            return unitPlatQueue;
        }

        public (int index, UnitPlat plat) GetUnitPlatByUnitSite(UnitSite unitSite)
        {
            switch (unitSite)
            {
                case UnitSite.first:
                    return (0, unitPlatQueue[0]);
                case UnitSite.second:
                    return (1, unitPlatQueue[1]);
                case UnitSite.third:
                    return (2, unitPlatQueue[2]);
                case UnitSite.fourth:
                    return (3, unitPlatQueue[3]);
                default:
                    return (-1, null);
            }
        }

        public List<UnitPlat> GetAllUnitPlat()
        {
            return unitPlatQueue;
        }

        public ICollection<UnitPlat> GetUnitPlats(ICollection<UnitSite> sites)
        {
            List<UnitPlat> unitPlats = new List<UnitPlat>();
            foreach (var site in sites)
            {
                UnitPlat plat = GetUnitPlatByUnitSite(site).plat;
                unitPlats.Add(plat);
            }

            return unitPlats;
        }

        public UnitSite GetUnitSite(Unit unit)
        {
            foreach (var unitPlat in unitPlatQueue)
            {
                if (unitPlat.unit == unit)
                {
                    return unitPlat.site;
                }
            }

            return UnitSite.under;
        }

        public void Clear()
        {
            unitPlatQueue.Clear();
        }

        public UnitPlatQueue()
        {
            unitPlatQueue = new List<UnitPlat>(unitPlatQueueCount);
        }

        public UnitPlatQueue(List<UnitPlat> unitPlatQueue)
        {
            this.unitPlatQueue = new List<UnitPlat>();
            unitPlatQueue.AddRange(unitPlatQueue);
        }
    }
}

public enum UnitSite
{
    first,
    second, 
    third, 
    fourth,
    under
}