using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SpeedBox : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }


    private List<LagCompensatedHit> _areaHits = new List<LagCompensatedHit>();
    public LayerMask collisionLayer;
    public void Init1()
    {
        life = TickTimer.CreateFromSeconds(Runner, 10.0f);

    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        else
        {
            checkCollosion();
        }
    }

    private void checkCollosion()
    {
        int col = Runner.LagCompensation.OverlapSphere(transform.position, 1.0f, Object.InputAuthority, _areaHits, collisionLayer, HitOptions.IncludePhysX);
        if (col > 0)
        {
            GameObject player = _areaHits[0].GameObject;
            if (player)
            {
                TankMovement target = player.GetComponent<TankMovement>();
                if (target != null)
                {

                    target.TakeSpeed(2);
                    print("Speed");
                    Runner.Despawn(Object);
                }
            }
        }

    }
}
