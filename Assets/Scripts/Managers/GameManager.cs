using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int numRoundsToWin = 5;
    public float startDelay = 3f;
    public float endDelay = 3f;
    public CameraControl cameraControl;
    public TMP_Text messageText;
    public GameObject tankPrefab;
    public TankManager[] tankManagers;

    private int _roundNumber;
    private WaitForSeconds _startWait;
    private WaitForSeconds _endWait;
    private TankManager _roundWinner;
    private TankManager _gameWinner;

    private void Start()
    {
        _startWait = new WaitForSeconds(startDelay);
        _endWait = new WaitForSeconds(endDelay);

        SpawnAllTanks();
        SetCameraTargets();

        StartCoroutine(GameLoop());
    }

    private void SpawnAllTanks()
    {
        for (int i = 0; i < tankManagers.Length; i++)
        {
            tankManagers[i].Instance =
                Instantiate(tankPrefab, tankManagers[i].spawnPoint.position, tankManagers[i].spawnPoint.rotation) as GameObject;
            tankManagers[i].playerNumber = i + 1;
            tankManagers[i].Setup();
        }
    }

    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[tankManagers.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = tankManagers[i].Instance.transform;
        }

        cameraControl.targets = targets;
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (_gameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }

    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();

        cameraControl.SetStartPositionAndSize();

        _roundNumber++;
        messageText.text = "ROUND " + _roundNumber;

        yield return _startWait;
    }

    private IEnumerator RoundPlaying()
    {
        EnableTankControl();

        messageText.text = string.Empty;

        while (!OneTankLeft())
        {
            yield return null;
        }
    }

    private IEnumerator RoundEnding()
    {
        DisableTankControl();

        _roundWinner = null;
        _roundWinner = GetRoundWinner();

        if (_roundWinner != null)
            _roundWinner.wins++;

        _gameWinner = GetGameWinner();

        string message = EndMessage();
        messageText.text = message;

        yield return _endWait;
    }

    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < tankManagers.Length; i++)
        {
            if (tankManagers[i].Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }

    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < tankManagers.Length; i++)
        {
            if (tankManagers[i].Instance.activeSelf)
                return tankManagers[i];
        }

        return null;
    }

    private TankManager GetGameWinner()
    {
        for (int i = 0; i < tankManagers.Length; i++)
        {
            if (tankManagers[i].wins == numRoundsToWin)
                return tankManagers[i];
        }

        return null;
    }

    private string EndMessage()
    {
        string message = "DRAW!";

        if (_roundWinner != null)
            message = _roundWinner.colouredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < tankManagers.Length; i++)
        {
            message += tankManagers[i].colouredPlayerText + ": " + tankManagers[i].wins + " WINS\n";
        }

        if (_gameWinner != null)
            message = _gameWinner.colouredPlayerText + " WINS THE GAME!";

        return message;
    }

    private void ResetAllTanks()
    {
        for (int i = 0; i < tankManagers.Length; i++)
        {
            tankManagers[i].Reset();
        }
    }

    private void EnableTankControl()
    {
        for (int i = 0; i < tankManagers.Length; i++)
        {
            tankManagers[i].EnableControl();
        }
    }

    private void DisableTankControl()
    {
        for (int i = 0; i < tankManagers.Length; i++)
        {
            tankManagers[i].DisableControl();
        }
    }
}