using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class OptionsData
{
    public bool invertedMovement;
    public float musicVolume;
    public float sfxVolume;

    public OptionsData()
    {

    }

    public void ChangeMovementSettings(bool _movementSetting)
    {
        if (_movementSetting)
            invertedMovement = _movementSetting;

    }
}
