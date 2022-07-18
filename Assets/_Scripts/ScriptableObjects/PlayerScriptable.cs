using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerScriptable", menuName = "Scriptables/PlayerScriptable", order = 1)]
public class PlayerScriptable : ScriptableObject
{
    [Header("Run")]
    public float runMaxSpeed;
    public float acceleration;
    public float deceleration;
    [Space(5)]
    [Range(0.5f, 2f)] public float accelPower;
    [Range(0.5f, 2f)] public float stopPower;
    [Range(0.5f, 2f)]  public float turnPower;

    [Header("Drag")]
    public float frictionAmmount;

    [Header("Other settings")]
    public bool doKeepRunMomentum;
}
