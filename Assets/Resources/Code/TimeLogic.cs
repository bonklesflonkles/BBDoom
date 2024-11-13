using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.ProBuilder.MeshOperations;

public class TimeLogic : MonoBehaviour
{
    public float timeSpent;
    [SerializeField] TMP_Text Timer;

    public MonsterNavigation mn;
    public MonsterNavigation mn2;
    public MonsterNavigation mn3;
    public AudioSource outro;

    [SerializeField] TMP_Text finish;

    private void Start()
    {
        timeSpent = 0;
        mn.enabled = false;
        mn2.enabled = false;
        mn3.enabled = false;
        finish.gameObject.SetActive(false); 
    }

    private void Update()
    {
        timeSpent += Time.deltaTime;
        int minutes = Mathf.FloorToInt(timeSpent / 60);
        int seconds = Mathf.FloorToInt(timeSpent % 60);
        Timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        if (timeSpent >= 75)
        {
            mn.enabled = true;
        }
        if (timeSpent >= 115)
        {
            mn2.enabled = true;
        }
        if (timeSpent >= 175)
        {
            mn3.enabled = true;
        }

        if (timeSpent >= 360)
        {
            EndGame();  
        }   
    }

    void EndGame()
    {
        mn.enabled = false;
        mn2.enabled = false;
        finish.gameObject.SetActive(true);
        outro.Play();
        outro.volume = Mathf.Lerp(0, 1, 1);
        StartCoroutine(endGame());
    }

    IEnumerator endGame()
    {
        yield return new WaitForSeconds(10f);
        GameManager.Instance.Leave();
    }
}
