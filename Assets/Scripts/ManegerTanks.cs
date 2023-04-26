using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ManegerTanks : NetworkBehaviour
{
    [Networked] public TickTimer respawnDelay { get; set; }
    [Networked] public TickTimer hpBoxDelay { get; set; }
    [Networked] public TickTimer SpeedBoxDelay { get; set; }
    [Networked] public Color Color { get; set; }

    public List<GameObject> allPlayer = new List<GameObject>();
    public HPBox hpBox;
    public CameraControl cameraControl;
    public SpeedBox SpeedB;



    public void Awake()
    {
        
    }



    public override void FixedUpdateNetwork()
    {
        if (hpBoxDelay.ExpiredOrNotRunning(Runner))
        {
            hpBoxDelay = TickTimer.CreateFromSeconds(Runner, 15f);
            Runner.Spawn(hpBox, new Vector3(Random.Range(-35, 35), 3, Random.Range(-35, 35)), Quaternion.identity, Object.InputAuthority, (runner, o) => { o.GetComponent<HPBox>().Init(); });
        }
        if (SpeedBoxDelay.ExpiredOrNotRunning(Runner))
        {
            SpeedBoxDelay = TickTimer.CreateFromSeconds(Runner, 15f);
            Runner.Spawn(SpeedB, new Vector3(Random.Range(-35, 35), 3, Random.Range(-35, 35)), Quaternion.identity, Object.InputAuthority, (runner, o) => { o.GetComponent<SpeedBox>().Init1(); });
        }

        SetCameraTargets();
    }

   

    public void AddPlayer(GameObject player)
    {
        allPlayer.Add(player);
        print(player.name + "have login to sever now!");
    }

    public void SetPlayer(GameObject player)
    {
        if (allPlayer.Contains(player))
        {
            player.SetActive(false);
            respawnDelay = TickTimer.CreateFromSeconds(Runner, 3f);
            StartCoroutine(OnRespawnPlayer(player));
        }
    }

    IEnumerator OnRespawnPlayer(GameObject player)
    {
        yield return new WaitUntil(() => respawnDelay.ExpiredOrNotRunning(Runner));
        player.SetActive(true);
        player.GetComponent<TankHealth>().Respawn();
    }
    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[allPlayer.ToArray().Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = allPlayer[i].transform;
        }

        cameraControl.targets = targets;
    }
}
