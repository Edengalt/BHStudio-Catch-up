using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelEndedScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winnerName;
    [SerializeField] private GameObject BG;
    private void Start()
    {
        EventManager.Instance.AddListener<SessionEnded>(OnSessionEnded);
        BG.SetActive(false);
    }

    private void OnSessionEnded(SessionEnded e)
    {
        BG.SetActive(true);
        winnerName.text = e.winnerName;
        StartCoroutine(NextSession());
    }

    public IEnumerator NextSession()
    {
        yield return new WaitForSecondsRealtime(5f);
        BG.SetActive(false);
        EventManager.Instance.Raise(new SessionStarted());
    }
}
