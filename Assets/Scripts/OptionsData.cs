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
        invertedMovement = MenuController.invertedMovement;
        musicVolume = MenuController.musicVolume;
        sfxVolume = MenuController.sfxVolume;
    }

    public void ChangeMovementSettings(bool _movementSetting)
    {
        invertedMovement = _movementSetting;
    }

    public void ChangeMusicVolume(float _volume)
    {
        musicVolume = _volume;
    }

    public void ChangeSfxVolume(float _volume)
    {
        sfxVolume = _volume;
    }
}
