using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    // Start is called before the first frame update
    void Start()
    {
        Power = Random.Range(0.8f, 1.2f);
        agent = GetComponent<NavMeshAgent>();
        destination = transform.position;
        gameObject.GetComponent<Renderer>().material.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
        InvokeRepeating("Grow", GrowCycle, GrowCycle);
        InvokeRepeating("ChangeColorToBossColor", 10, 10);
    }

    // Update is called once per frame
    void Update()
    {
        if ((destination - transform.position).sqrMagnitude < 0.5f)
        {
            if (Boss != null)
            {
                float zoneOfInterest = 2f * (Boss.GetComponent<OrkController>().GetPower() + GetPower());
                destination = Boss.transform.position + new Vector3(Random.Range(-zoneOfInterest, zoneOfInterest), 0, Random.Range(-zoneOfInterest, zoneOfInterest));
            }
            else
            {
                destination = transform.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
            }
            agent.destination = destination;
            //Debug.Log("New destination " + destination.ToString());
        }

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
            result += boyzPowerCoeff * Boy.GetComponent<OrkController>().GetAdjustedPower();
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
        Debug.Log(gameObject.name + " collides onTrigger with " + other.gameObject.name);

        if (GetWarBoss() != other.gameObject.GetComponent<OrkController>().GetWarBoss())
        {
            TryToSubjugate(other.gameObject.GetComponent<OrkController>().GetWarBoss());
        }
    }

    private bool TryToSubjugate(GameObject target)
    {
        if (target.GetComponent<OrkController>().SubjugationBy(gameObject))
        {
            target.GetComponent<OrkController>().SetBoss(gameObject);
            Boyz.Add(target);
        }
        else
        {
            if (Boss == null)
            {
                if (!Enemies.Contains(target))
                {
                    Enemies.Add(target);

                }
                return true;
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
        if (pretenderToBoss.GetComponent<OrkController>().GetAdjustedPower() > 1.5*GetAdjustedPower())
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