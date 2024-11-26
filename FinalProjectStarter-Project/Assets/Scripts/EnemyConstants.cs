using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyConstants
{
    // Piranha Plant constants
    public const float PiranhaPlantOffsetY = 1.5f;
    public const float PiranhaPlantAnimationDuration = 0.75f;
    public const float PiranhaPlantActiveDuration = 2.5f;
    public const float PiranhaPlantHiddenDurationMin = 2.0f;
    public const float PiranhaPlantHiddenDurationMax = 4.0f;


    // Goomba constants
    public const float GoombaSpeed = 3.0f;
    public const float GoombaSquishedDuration = 0.6f;


    // Fire Plant constants
    public const float FirePlantOffsetY = 2.0f;
    public const float FirePlantAnimationDuration = 0.75f;
    public const float FirePlantActiveDuration = 2.5f;
    public const float FirePlantHiddenDurationMin = 2.0f;
    public const float FirePlantHiddenDurationMax = 4.0f;
    public const float FirePlantFireDuration = 0.75f;
    public const float FirePlantFireDelayDuration = 1.5f;
    public const float FirePlantHeadOffsetY = 1.5f;
    public const float FirePlantFireballSpeed = 2.35f;

    //Boo const
    public const float c_booSpeedx = 3.0f;
    public const float c_booSpeedy = 3.0f;

    //BoomBoom Const
    public const float c_boomBoomSpeed = 3.0f;
    public const float c_boomBoomNearDeathSpeed = 6.0f;
    public const float c_boomBoomStunnedDuration = 1.0f;
    public const Int32 c_boomBoomLives = 3;
    public const Int32 c_boomBoomNearDeathLives = 1;
    public const float c_boomBoomDirectionChangeInterval = 1.0f;
    public static Vector3 c_boomBoomAwakenRange { get { return new Vector3(10.0f, 10.0f, 10.0f); } }


}
