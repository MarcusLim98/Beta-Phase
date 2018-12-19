using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningNoise : Noisemaker
{

    private PlayerLogic playerLog;
    float radiusSize;
    CapsuleCollider capColl;


    void Start()
    {
        playerLog = GameObject.Find("Player").GetComponent<PlayerLogic>();
        capColl = gameObject.GetComponent<CapsuleCollider>();
    }

    void Update()
    {

        if (playerLog.movingStyle == 1)
        {
            radiusSize = 5;
            transform.position = playerLog.gameObject.transform.position;
            active = true;
            capColl.radius = radiusSize;
        }

        else if (playerLog.movingStyle != 1)
        {
            radiusSize = 1;
            transform.position = transform.position;
            active = false;
            capColl.radius = radiusSize;
        }

    }
}
