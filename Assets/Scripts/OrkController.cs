using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class OrkController : MonoBehaviour
{
    public Vector3 destination;
    NavMeshAgent agent;
    public List<GameObject> Boyz;
    public GameObject Boss;
    public float Power;
    private float boyzPowerCoeff = 0.2f;
    private float GrowCycle = 2;
    public float GrowSpeed = 0.1f;
    public List<GameObject> Enemies;
    private bool dead = false;
    public float deadBodyDecayTimer = 5;
    private float attackCooldown=0;
    public float attackCooldownValue=1;
    public float StrikeRandomModifier=0.25f;
    public float DefenceRandomModifier = 0.25f;
    public float InjuryChanceModificator = 0.2f;
    public float InjuryPowerDecreaseModificator = 0.75f;
    public float EnemyKilledPowerIncreaseModificator = 0.1f;
    public float RebelAdjustedPowerCoefficient = 0.5f;
    public float SubjigationPowerCoefficient=1.5f;
    public float RebelTimeCooldownProtection = 0;
    public float RebelTimeCooldownProtectionValue = 5;
    public float BoringValue = 0;
    public float BoringModifier = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        Power = Random.Range(0.8f, 1.2f);
        agent = GetComponent<NavMeshAgent>();
        destination = transform.position;
        gameObject.GetComponent<Renderer>().material.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
        InvokeRepeating("Grow", GrowCycle, GrowCycle);
        InvokeRepeating("ChangeColorToBossColor", 10, 10);
        InvokeRepeating("RebelAgainstOldBoss", 10, 10);
        //InvokeRepeating("Die", 5, 100000);
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
        {
            deadBodyDecayTimer -= Time.deltaTime;
            if (deadBodyDecayTimer <= 0)
            {
                Destroy(gameObject);
            }
            return;
        }
        if ((destination - transform.position).sqrMagnitude < 0.5f)
        {
            if (Boss != null)
            {
                float zoneOfInterest = 2f * (Boss.GetComponent<OrkController>().GetPower() + GetPower());
                destination = Boss.transform.position + new Vector3(Random.Range(-zoneOfInterest, zoneOfInterest), 0, Random.Range(-zoneOfInterest, zoneOfInterest));
            }
            else
            {
                Enemies = Enemies.Where(x => x != null).ToList();
                if (Enemies.Count == 0)
                {
                    destination = transform.position + new Vector3(Random.Range(-2*Power, 2*Power), 0, Random.Range(-2*Power, 2*Power));
                }
                else
                {
                    
                    GameObject nearestEnemy = Enemies.OrderBy(x=>(x.transform.position-gameObject.transform.position).sqrMagnitude).First();
                    destination = nearestEnemy.transform.position;
                }
            }
            agent.destination = destination;
            //Debug.Log("New destination " + destination.ToString());
        }
        if (attackCooldown >= 0)
        {
            attackCooldown -= Time.deltaTime;
        }
        if (RebelTimeCooldownProtection > 0)
        {
            RebelTimeCooldownProtection -= Time.deltaTime;
        }
        BoringValue += BoringModifier;

    }
    private void RebelAgainstOldBoss()
    {
        if (Boss != null)
        {
            if ((GetPower()+BoringValue > Boss.GetComponent<OrkController>().GetPower()) && (GetAdjustedPower()+BoringValue > RebelAdjustedPowerCoefficient * Boss.GetComponent<OrkController>().GetAdjustedPower()))
            {
                Debug.Log(gameObject.name + " rebels against " + Boss.name + "!");
                Boss.GetComponent<OrkController>().Boyz.Remove(gameObject);
                Enemies.Add(Boss);
                Boss = null;
                gameObject.GetComponent<Renderer>().material.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
                foreach(GameObject boy in Boyz)
                {
                    boy.GetComponent<OrkController>().ChangeColorToBossColor();
                }
                RebelTimeCooldownProtection = RebelTimeCooldownProtectionValue;
            }
        }
    }
    private void Die()
    {
        dead = true;
        CancelInvoke();
        if (Boss != null)
        {
            Boss.GetComponent<OrkController>().Boyz.Remove(gameObject);
        }
        foreach(GameObject boy in Boyz)
        {
            boy.GetComponent<OrkController>().Boss = null;
        }
        gameObject.GetComponent<Renderer>().material.color = new Color32(1, 1, 1, 255);
        gameObject.GetComponent<NavMeshAgent>().enabled = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
       
    }
    private void Grow()
    {
        Power += GrowSpeed;
        AdjustSize();
    }

    private void AdjustSize()
    {
        gameObject.transform.localScale = new Vector3(Power, 1, Power);
    }

    public float GetPower()
    {
        return Power;
    }
    public float GetAdjustedPower()
    {
        float result = Power;
        foreach (GameObject Boy in Boyz)
        {
            if (Boy != null)
            {
                result += boyzPowerCoeff * Boy.GetComponent<OrkController>().GetAdjustedPower();
            }
        }
        return result;
    }
    public GameObject GetWarBoss()
    {
        if (Boss != null)
        {
            return Boss.GetComponent<OrkController>().GetWarBoss();
        }
        return gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(gameObject.name + " collides onTrigger with " + other.gameObject.name);
        if (other.gameObject != null && other.gameObject.TryGetComponent<OrkController>(out OrkController orkController))
        {
            if (GetWarBoss() != other.gameObject.GetComponent<OrkController>().GetWarBoss())
            {
                if (TryToSubjugate(other.gameObject.GetComponent<OrkController>().GetWarBoss()))
                {

                }
                else
                {
                    if (GetWarBoss() != other.gameObject.GetComponent<OrkController>().GetWarBoss())
                    {
                        Attack(other);
                    }
                }
            }
        }
    }

    private void Attack(Collider other)
    {
        if (attackCooldown <= 0)
        {
            attackCooldown = attackCooldownValue;
            BoringValue = 0;
            float BlowPower = Power * (1 + Random.Range(-StrikeRandomModifier, StrikeRandomModifier));
            if (other.gameObject.GetComponent<OrkController>().RecevieBlow(BlowPower))
            {
                Power += EnemyKilledPowerIncreaseModificator;
                Debug.Log(gameObject.name + " kills " + other.gameObject.name);
            }
        }
    }

    private bool RecevieBlow(float blowPower)
    {
        float defencePower= Power * (1 + Random.Range(-DefenceRandomModifier, DefenceRandomModifier));
        if (blowPower > defencePower)
        {
            Die();
            return true;
        }
        else
        {
            if (Random.Range(0f, 1f) < InjuryChanceModificator)
            {
                Power = Power * InjuryPowerDecreaseModificator;
                Debug.Log(gameObject.name + " was injured and now have power " + Power);
            }
        }
        return false;
    }

    private bool TryToSubjugate(GameObject target)
    {
        if (target.GetComponent<OrkController>().SubjugationBy(gameObject))
        {
            target.GetComponent<OrkController>().SetBoss(gameObject);
            Boyz.Add(target);
            return true;
        }
        else
        {
            if (Boss == null)
            {
                if (!Enemies.Contains(target))
                {
                    Enemies.Add(target);

                }
                return false;
            }
            else
            {
                Boss.GetComponent<OrkController>().TryToSubjugate(target);
            }
        }
        return false;
    }

    private bool SubjugationBy(GameObject pretenderToBoss)
    {
        if (RebelTimeCooldownProtection > 0)
        {
            return false;
        }
        if ((pretenderToBoss.GetComponent<OrkController>().GetPower() > GetPower()) && (pretenderToBoss.GetComponent<OrkController>().GetAdjustedPower() > SubjigationPowerCoefficient * GetAdjustedPower()))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SetBoss(GameObject newBoss)
    {
        Boss = newBoss;
        ChangeColorToBossColor();
        Enemies.Clear();
        if (Boss.GetComponent<OrkController>().Enemies.Contains(gameObject))
        {
            Boss.GetComponent<OrkController>().Enemies.Remove(gameObject);
        }
    }

    private void ChangeColorToBossColor()
    {
        if (Boss == null)
        {
            return;
        }
        gameObject.GetComponent<Renderer>().material.color = GetWarBoss().GetComponent<Renderer>().material.color;
        foreach (GameObject boy in Boyz)
        {
            boy.GetComponent<OrkController>().ChangeColorToBossColor();
        }
    }
}