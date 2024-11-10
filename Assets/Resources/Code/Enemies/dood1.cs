using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class dood1 : MonoBehaviour
{
    public float DetectionRange = 5f;
    public float MonsterSpeedWander = 5f;
    public float MonsterSpeedChase = 7.5f;
    public NavMeshAgent agent;
    public Transform[] points;
    public string tagString = "Player";

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                agent.enabled = true;
                GameObject[] players = GameObject.FindGameObjectsWithTag(tagString);

                GameObject target = null;

                // Set the target to the closest player
                if (players.Length > 0)
                {
                    float minDistance = float.MaxValue;
                    foreach (GameObject player in players)
                    {
                        float distance = Vector3.Distance(transform.position, player.transform.position);
                        if (distance < DetectionRange && distance < minDistance)
                        {
                            minDistance = distance;
                            target = player;
                        }
                    }

                    if (target != null)
                    {
                        agent.speed = MonsterSpeedChase;
                        Chase(target.transform);
                    }

                    else if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    {
                        agent.speed = MonsterSpeedWander;
                        Wander();
                    }
                }

                if (target != null)
                {
                    agent.destination = target.transform.position;
                }
            }
            else
            {
                agent.enabled = false;

            }
        }
    }

    void Chase(Transform target)
    {
        agent.destination = target.position;
    }

    void Wander()
    {
        if (points.Length == 0)
            return;

        int destPoint = Random.Range(0, points.Length);
        agent.destination = points[destPoint].position;
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);
    }
}
