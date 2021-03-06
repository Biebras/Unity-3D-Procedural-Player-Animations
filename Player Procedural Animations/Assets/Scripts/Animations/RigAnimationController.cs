using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigAnimationController : MonoBehaviour
{
    public List<Transform> RigTransforms;

    [SerializeField] private RigAnimation[] _animations;
    [SerializeField] private bool _playOnStart;

    [Header("For Easy Keyframe Add")]
    public string EditorAnimationName;
    public int EditorAnimationKeyframe;

    private Animation _currentAnimation;

    private void Start()
    {
        PlayAnimationOnStart();
    }

    private void PlayAnimationOnStart()
    {
        if (_animations.Length == 0 || !_playOnStart)
            return;

        var animation = _animations[0];
        PlayAnimation(animation.Name);
    }

    public void PlayAnimation(string name)
    {
        var animation = GetAnimation(name);
        var keyframeIndex = animation.GetKeyframe();
        var keyframe = animation.Keyframes[keyframeIndex];
        animation.IncramentKeyframe();

        for (int i = 0; i < keyframe.RigPosition.Count; i++)
        {
            RigTransforms[i].localPosition = keyframe.RigPosition[i].Position;
            RigTransforms[i].localEulerAngles = keyframe.RigPosition[i].Rotation;
        }

    }

    public RigAnimation GetAnimation(string name)
    {
        RigAnimation animation = Array.Find(_animations, a => a.Name == name);

        if (animation == null)
            throw new Exception($"Couldn't find animation with the name: {name}");

        return animation;
    }
}
