using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class WeepingAngelAI : MonoBehaviour
{
    [Header("Assignables")]
    public NavMeshAgent ai;
    public Transform player;
    public GameObject playerobj;
    public Camera playerCam, jumpscareCam;
    public Animator anim;
    public Transform centrepoint;
    [Header("Settings")]
    public float range;
    public float aiSpeed;
    public float catchDistance;
    public float jumpscareTime;
    public string sceneAfterDeath;

    Vector3 dest;

    bool RandomPoint (Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }

    bool inLineOfSight()
    {
        RaycastHit hit;
        Vector3 playerDirection = player.position - transform.position;

        if (Physics.Raycast(transform.position, playerDirection, out hit, 50000f))
        {
            if (hit.transform.name == "PlayerBean")
            {
                Debug.DrawLine(transform.position, player.position, Color.green);
                return true;    
            }
        }

        return false;
    }

    void Update()
    {     
        inLineOfSight();
        
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCam);
        float distance = Vector3.Distance(transform.position, player.position);
        if (GeometryUtility.TestPlanesAABB(planes, this.gameObject.GetComponent<Renderer>().bounds)) {
            if (inLineOfSight())
            {
                anim.speed = 0;
                ai.speed = 0;
                ai.SetDestination(transform.position);
            }
            if (!inLineOfSight())
            {
                ai.speed = aiSpeed;
                anim.speed = 1;
                dest = player.position;
                ai.destination = dest;
            }
        }
        if (!GeometryUtility.TestPlanesAABB(planes, this.gameObject.GetComponent<Renderer>().bounds))
        {
            if (distance >= 10)
            {
                if (ai.remainingDistance <= ai.stoppingDistance)
                {
                    Vector3 point;
                    if (RandomPoint(centrepoint.position, range, out point))
                    {
                        ai.SetDestination(point);
                        ai.speed = aiSpeed / 2;
                        anim.speed = 1f;
                        return;
                    }
                }
            }
            if (distance <= 9.9)
            {
                ai.speed = aiSpeed;
                anim.speed = 1;
                dest = player.position;
                ai.destination = dest;
            }
        }
        if (distance <= catchDistance)
        {
            player.gameObject.SetActive(false);
            jumpscareCam.gameObject.SetActive(true);
            StartCoroutine(killPlayer());
        }
    }
    IEnumerator killPlayer()
    {
        yield return new WaitForSeconds(jumpscareTime); 
        SceneManager.LoadScene(sceneAfterDeath); 
    }
}