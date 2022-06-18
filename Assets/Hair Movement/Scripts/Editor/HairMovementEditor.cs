using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Sinx {
    [CustomEditor(typeof(HairMovement)), CanEditMultipleObjects]
    public class HairMovementEditor : Editor {
        
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            HairMovement script = (HairMovement)target;
            if (!Application.isPlaying)
                script.InitializeBlobs();
            if (!script.spriteRenderer)
                script.spriteRenderer = script.GetComponent<SpriteRenderer>();
            if (script.hairBlobs.Count > 0) {
                script.spriteRenderer.sprite = script.GenerateSprite();
            } else {
                script.spriteRenderer.sprite = null;
            }
        }
    }
}