using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sinx {
public class SimpleMotion : MonoBehaviour {
    [SerializeField] SimpleMotionType simpleMotionType;
    
    Vector3 initialPosition;

    void Start() {
        initialPosition = transform.position;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            GetComponent<HairMovement>().FlipX = !GetComponent<HairMovement>().FlipX;
        }
        float t = Time.time*Mathf.PI*2f;
        switch (simpleMotionType) {
            case SimpleMotionType.Circular:
                transform.position = initialPosition + new Vector3(Mathf.Cos(t), Mathf.Sin(t), 0f)*2f;
                break;
            case SimpleMotionType.Jump:
                transform.position = initialPosition + new Vector3(0f, Mathf.Pow(Mathf.Max(Mathf.Sin(t), 0f), .5f), 0f)*2f;
                break;
            case SimpleMotionType.SideToSide:
                transform.position = initialPosition + new Vector3(Mathf.Pow(Mathf.Abs(Mathf.Sin(t)), .25f) * Mathf.Sign(Mathf.Sin(t)), 0f, 0f)*2f;
                break;
            default:
                break;
        }    
    }
}

public enum SimpleMotionType {
    Circular,
    Jump,
    SideToSide
}

}

