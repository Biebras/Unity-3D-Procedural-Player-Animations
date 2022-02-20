using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RigAnimation
{
    public string Name;
    public List<Keyframe> Keyframes;
    public AnimationCurve Ease;

    private int _currentKeyframe;

    public int GetKeyframe()
    {
        return _currentKeyframe;
    }

    public int IncramentKeyframe()
    {
        _currentKeyframe++;

        if (_currentKeyframe >= Keyframes.Count)
            _currentKeyframe = 0;

        return _currentKeyframe;
    }
}
