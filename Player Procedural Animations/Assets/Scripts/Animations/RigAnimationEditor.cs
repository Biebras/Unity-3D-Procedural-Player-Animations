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
            SetKeyframe(rigAnimation.EditorAnimationName, rigAnimation.EditorAnimationKeyframe, rigAnimation.RigTransforms);
            rigAnimation.EditorAnimationKeyframe++;
        }
    }

    private void SetKeyframe(string name, int keyframeIndex, List<Transform> rigTransform)
    {
        RigAnimationController rigAnimation = (RigAnimationController)target;

        if (rigAnimation == null || rigTransform.Count == 0)
            return;

        var animation = rigAnimation.GetAnimation(name);

        animation.Keyframes.Add(new Keyframe());

        var keyframe = animation.Keyframes[keyframeIndex];

        keyframe.RigPosition = new List<Vector3>();

        if(keyframe.RigPosition.Count != 0)
            keyframe.RigPosition.Clear();


        foreach (var rig in rigTransform)
        { 
            keyframe.RigPosition.Add(rig.position);
        }
    }
}
#endif
