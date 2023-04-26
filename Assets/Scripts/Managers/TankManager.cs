using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    public Color playerColour;
    public Transform spawnPoint;
    [HideInInspector] public int playerNumber;
    [HideInInspector] public string colouredPlayerText;
    [HideInInspector] public GameObject Instance;
    [HideInInspector] public int wins;

    private TankMovement _movement;
    private TankShooting _shooting;
    private GameObject _canvasGameObject;

    public void Setup()
    {
        _movement = Instance.GetComponent<TankMovement>();
        _shooting = Instance.GetComponent<TankShooting>();
        _canvasGameObject = Instance.GetComponentInChildren<Canvas>().gameObject;

        _movement.playerNumber = playerNumber;
        _shooting.playerNumber = playerNumber;

        colouredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(playerColour) + ">PLAYER " + playerNumber + "</color>";

        MeshRenderer[] renderers = Instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = playerColour;
        }
    }

    public void DisableControl()
    {
        _movement.enabled = false;
        _shooting.enabled = false;

        _canvasGameObject.SetActive(false);
    }

    public void EnableControl()
    {
        _movement.enabled = true;
        _shooting.enabled = true;

        _canvasGameObject.SetActive(true);
    }

    public void Reset()
    {
        Instance.transform.position = spawnPoint.position;
        Instance.transform.rotation = spawnPoint.rotation;

        Instance.SetActive(false);
        Instance.SetActive(true);
    }
}
