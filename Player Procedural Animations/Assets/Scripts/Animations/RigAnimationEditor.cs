using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(RigAnimationController))]
public class RigAnimationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RigAnimationController rigAnimation = (RigAnimationController)target;

        if (rigAnimation == null)
            return;

        DrawDefaultInspector();

        Undo.RecordObject(rigAnimation, "Change Rig keyframes");

        if(GUILayout.Button("Add Rig Positions"))
        {
            SetKeyframes(rigAnimation.EditorAnimationName, rigAnimation.EditorAnimationKeyframe, rigAnimation.RigTransforms);
            rigAnimation.EditorAnimationKeyframe++;
        }

        if(GUILayout.Button("Change Key Frame"))
        {
            ChangeKeyframe(rigAnimation.EditorAnimationName, rigAnimation.EditorAnimationKeyframe, rigAnimation.RigTransforms);
        }
    }

    private void SetKeyframes(string name, int keyframeIndex, List<Transform> rigTransform)
    {
        RigAnimationController rigAnimation = (RigAnimationController)target;

        if (rigAnimation == null || rigTransform.Count == 0)
            return;

        var animation = rigAnimation.GetAnimation(name);

        animation.Keyframes.Add(new Keyframe());

        var keyframe = animation.Keyframes[keyframeIndex];

        keyframe.RigPosition = new List<RigTransform>();

        if(keyframe.RigPosition.Count != 0)
            keyframe.RigPosition.Clear();


        foreach (var rig in rigTransform)
        {
            var position = rig.localPosition;
            var rotation = rig.localEulerAngles;
            var transform = new RigTransform(position, rotation);

            keyframe.RigPosition.Add(transform);
        }
    }

    private void ChangeKeyframe(string name, int keyframeIndex, List<Transform> rigTransform)
    {
        RigAnimationController rigAnimation = (RigAnimationController)target;

        if (rigAnimation == null || rigTransform.Count == 0)
            return;

        var animation = rigAnimation.GetAnimation(name);
        var rigPos = animation.Keyframes[keyframeIndex].RigPosition;

        for (int i = 0; i < rigPos.Count; i++)
        {
            rigTransform[i].localPosition = rigPos[i].Position;
            rigTransform[i].localEulerAngles = rigPos[i].Rotation;
        }
    }
}
#endif
