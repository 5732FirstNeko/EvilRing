using System.Collections;
using System.Collections.Generic;
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

    //TODO : Optimize the value Code
    [Header("Friendly")]
    //to anchoring the Plat Position
    [SerializeField] private List<UnitSiteFlag> friendlyUnitSiteFlag;

    //to Creat 4 Plat instance, the value will be optimized in future
    [SerializeField] private List<UnitPlat> friendlyUnitPlats;

    //TODO : Optimize the value Code
    [Header("Hostitly")]
    //to anchoring the Plat Position
    [SerializeField] private List<UnitSiteFlag> hostilityUnitSiteFlag;

    //to Creat 4 Plat instance, the value will be optimized in future
    [SerializeField] private List<UnitPlat> hostitlyUnitPlats;

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

    private PriorityQueue<UnitPlat> battleQueue;
    private Dictionary<Unit, UnitSkill> OnGameStartSkills;
    private Dictionary<Unit, UnitSkill> OnRoundStartSkills;
    private Dictionary<Unit, UnitSkill> OnRoundEndSkills;
    private Dictionary<Unit, UnitSkill> OnStrikeBackSkills;

    private Dictionary<Unit, byte> OnStrikeBackMap;
    private Dictionary<UnitBuff, ICollection<UnitPlat>> UnitBuffMap;

    private int friendlyDeadCount;
    private int hostilityDeadCount;

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
        OnGameStartSkills = new Dictionary<Unit, UnitSkill>();
        OnRoundStartSkills = new Dictionary<Unit, UnitSkill>();
        OnRoundEndSkills = new Dictionary<Unit, UnitSkill>();
        OnStrikeBackSkills = new Dictionary<Unit, UnitSkill>();

        OnStrikeBackMap = new Dictionary<Unit, byte>();
        UnitBuffMap = new Dictionary<UnitBuff, ICollection<UnitPlat>>();

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

                foreach (var flag in friendlyUnitSiteFlag)
                {
                    Debug.Log(flag.transform.position.x);
                }

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

    public void BattleInit(List<Unit> hostilityUnits, List<Unit> friendlyUnits)
    {
        StartCoroutine(BattleInitCoroutine(hostilityUnits, friendlyUnits));
    }

    private IEnumerator BattleInitCoroutine(List<Unit> hostilityUnits, List<Unit> friendlyUnits)
    {
        //TODO : Battle Start UI Animation

        FriendlyUnitPlatsQueue.Clear();
        HostilityUnitPlatsQueue.Clear();

        //TODO : Optimize the InitCode
        {
            for (int i = 0; i < hostilityUnits.Count; i++)
            {
                hostitlyUnitPlats[i].UnitPlatInit(hostilityUnits[i], GetUnitSiteByIndex(i));
                HostilityUnitPlatsQueue.unitPlatQueue.Add(hostitlyUnitPlats[i]);
            }
            for (int i = 0; i < friendlyUnits.Count; i++)
            {
                friendlyUnitPlats[i].UnitPlatInit(friendlyUnits[i], GetUnitSiteByIndex(i));
                FriendlyUnitPlatsQueue.unitPlatQueue.Add(friendlyUnitPlats[i]);
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
            battleQueue.Enqueue(plat, plat.unit.speed);
            foreach (var skill in plat.unit.unitSkills)
            {
                if (skill.TriggerTiming == TriggerTiming.OnLevelStart)
                {
                    OnGameStartSkills.Add(plat.unit,skill);
                }
                if (skill.TriggerTiming == TriggerTiming.OnRoundStart)
                {
                    OnRoundStartSkills.Add(plat.unit, skill);
                }
                if (skill.TriggerTiming == TriggerTiming.OnRoundEnd)
                {
                    OnRoundEndSkills.Add(plat.unit, skill);
                }
                if (skill.TriggerTiming == TriggerTiming.OnStrikeBack)
                {
                    OnStrikeBackSkills.Add(plat.unit, skill);
                    OnStrikeBackMap.Add(plat.unit, plat.unit.HP);
                }
            }
        }
        foreach (var plat in HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            battleQueue.Enqueue(plat, plat.unit.speed);
            foreach (var skill in plat.unit.unitSkills)
            {
                if (skill.TriggerTiming == TriggerTiming.OnLevelStart)
                {
                    OnGameStartSkills.Add(plat.unit, skill);
                }
                if (skill.TriggerTiming == TriggerTiming.OnRoundStart)
                {
                    OnRoundStartSkills.Add(plat.unit, skill);
                }
                if (skill.TriggerTiming == TriggerTiming.OnRoundEnd)
                {
                    OnRoundEndSkills.Add(plat.unit, skill);
                }
                if (skill.TriggerTiming == TriggerTiming.OnStrikeBack)
                {
                    OnStrikeBackSkills.Add(plat.unit, skill);
                    OnStrikeBackMap.Add(plat.unit, plat.unit.HP);
                }
            }
        }

        yield return null;

        StartCoroutine(Battle());
    }

    private IEnumerator Battle()
    {
        yield return LevelStartBattle();
        yield return AllBuffAction(TriggerTiming.OnLevelStart);

        int safecount = 0;
        while (friendlyDeadCount < 4 && hostilityDeadCount < 4)
        {
            safecount++;
            if (safecount > 1000)
            {
                yield break;
            }

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
                yield return RoundStartBattle();
                yield return AllBuffAction(TriggerTiming.OnRoundStart);

                UnitPlat unitPlat = battleQueue.Dequeue();
                UnitSkill OnRoundSkill = unitPlat.unit.UnitSkillChoice();
                ICollection<UnitPlat> targetPlats = GetActionTargetPlat(unitPlat.unit, OnRoundSkill);
                ICollection<Unit> targets = GetActionTarget(targetPlats);
                OnRoundSkill.Action(targets);
                foreach (var buff in OnRoundSkill.UnitBuffs)
                {
                    AddBuffToUnitPlat(buff, targetPlats);
                }
                yield return AllBuffAction(TriggerTiming.OnRound);
                yield return new WaitForSeconds(OnRoundSkill.SkillTime);

                Debug.Log("After Swap Site " + unitPlat.unit.faction + " " +unitPlat.unit);

                yield return RoundEndBattle();
                yield return AllBuffAction(TriggerTiming.OnRoundEnd);

                yield return StrikeBackBattle();
                yield return AllBuffAction(TriggerTiming.OnStrikeBack);
            }

            foreach (var plat in FriendlyUnitPlatsQueue.GetAllUnitPlat())
            {
                battleQueue.Enqueue(plat, plat.unit.speed);
            }
            foreach (var plat in HostilityUnitPlatsQueue.GetAllUnitPlat())
            {
                battleQueue.Enqueue(plat, plat.unit.speed);
            }
        }
    }

    private IEnumerator AllBuffAction(TriggerTiming timing)
    {
        List<UnitBuff> removeBuff = new List<UnitBuff>();
        foreach (var (buff, plats) in UnitBuffMap)
        {
            if (buff.TriggerTiming != timing)
                continue;

            ICollection<Unit> units = GetActionTarget(plats);
            buff.Action(units);
            foreach (var plat in plats)
            {
                int continueRound = plat.BuffActiuon(buff);
                if (continueRound <= 0)
                {
                    removeBuff.Add(buff);
                }
            }

            HPCheck(plats);
            yield return new WaitForSeconds(buff.BuffActionTime);
        }

        for (int i = removeBuff.Count - 1; i >= 0; i--)
        {
            UnitBuffMap.Remove(removeBuff[i]);
        }
    }

    private IEnumerator LevelStartBattle()
    {
        foreach (var (unit, gameStartSkill) in OnGameStartSkills)
        {
            ICollection<UnitPlat> unitPlats = GetActionTargetPlat(unit, gameStartSkill);
            ICollection<Unit> units = GetActionTarget(unitPlats);
            if (units != null)
            {
                gameStartSkill.Action(units);
                HPCheck(unitPlats);

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
        foreach (var (unit, roundStartSkill) in OnRoundStartSkills)
        {
            ICollection<UnitPlat> unitPlats = GetActionTargetPlat(unit, roundStartSkill);
            ICollection<Unit> units = GetActionTarget(unitPlats);
            if (units != null)
            {
                roundStartSkill.Action(units);
                HPCheck(unitPlats);

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
        foreach (var (unit, roundEndSkill) in OnRoundEndSkills)
        {
            ICollection<UnitPlat> unitPlats = GetActionTargetPlat(unit, roundEndSkill);
            ICollection<Unit> units = GetActionTarget(unitPlats);
            if (units != null)
            {
                roundEndSkill.Action(units);
                HPCheck(unitPlats);

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
                UnitSkill strikeBackSkill = OnStrikeBackSkills[unit];
                ICollection<UnitPlat> targetPlats = GetActionTargetPlat(unit, strikeBackSkill);
                ICollection<Unit> targets = GetActionTarget(targetPlats);
                strikeBackSkill.Action(targets);
                HPCheck(targetPlats);

                foreach (var buff in strikeBackSkill.UnitBuffs)
                {
                    AddBuffToUnitPlat(buff, targetPlats);
                }

                yield return new WaitForSeconds(strikeBackSkill.SkillTime);
            }
        }
    }

    private IEnumerator HPCheck(ICollection<UnitPlat> unitPlats)
    {
        float maxDeadTIme = 0;
        foreach (var plat in unitPlats)
        {
            if (plat.isDead) continue;

            if (plat.unit.HP <= 0)
            {
                plat.isDead = true;
                if (plat.unit.faction == Faction.Friendly)
                {
                    battleQueue.Remove(plat);
                    friendlyDeadCount++;
                }
                hostilityDeadCount++;

                if (hostilityDeadCount >= 4)
                {
                    //TODO : Game Win Logic
                }

                if (friendlyDeadCount >= 4)
                {
                    //TODO : Game Lose Logic
                }

                if (maxDeadTIme < plat.unit.DeadAnimationTime)
                {
                    maxDeadTIme = plat.unit.DeadAnimationTime;
                }
            }
        }

        yield return maxDeadTIme;
    }

    private void AddBuffToUnitPlat(UnitBuff buff, ICollection<UnitPlat> plats)
    {
        foreach (var plat in plats)
        {
            plat.AddBuff(buff);
        }
        UnitBuffMap.Add(buff, plats);
    }

    private ICollection<UnitPlat> GetActionTargetPlat(Unit self, UnitSkill skill)
    {
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

    public static int GetindexByUnitSite(UnitSite site)
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

        public ICollection<UnitPlat> GetAllUnitPlat()
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