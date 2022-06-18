using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Symbiosinx
namespace Sinx {
[RequireComponent(typeof(SpriteRenderer))]
public class HairMovement : MonoBehaviour {
    
    [SerializeField] ComputeShader computeShader;
    [SerializeField, Range(1, 100)] int stiffness = 30;
    [SerializeField, Range(0f, 1f)] float minSeparation = .1f;
    [SerializeField, Range(0f, 1f)] float maxSeparation = .7f;
    [SerializeField] Color color = Color.white;
    [SerializeField, Min(1)] int pixelsPerUnit = 16;
    [SerializeField, Min(0)] int pixelBuffer = 1;
    [SerializeField] bool showGizmos = false;
    public bool FlipX {
        get { return flipX; }
        set {
            if (flipX != value) {
                for (int i = 0; i < hairBlobs.Count; i++) {
                    hairBlobs[i].offset.x *= -1f;
                }
            }
            flipX = value;
        }
    }
    public bool FlipY {
        get { return flipY; }
        set {
            if (flipY != value) {
                for (int i = 0; i < hairBlobs.Count; i++) {
                    hairBlobs[i].offset.y *= -1f;
                }
            }
            flipY = value;
        }
    }
    bool flipX = false;
    bool flipY = false;
    [SerializeField] public List<HairBlob> hairBlobs = new List<HairBlob>();
    [HideInInspector] public SpriteRenderer spriteRenderer;

    Texture2D previousTexture2D = null;

    public Sprite GenerateSprite() {

        Vector4 bounds = GetBoundingBox();
        Vector2 size = new Vector2(-bounds.x + bounds.z, -bounds.y + bounds.w);
        Vector2Int texSize = Vector2Int.RoundToInt(size*pixelsPerUnit);

        int kernel = computeShader.FindKernel("GenerateTexture");
        RenderTexture output = RenderTexture.GetTemporary(texSize.x, texSize.y, 24);
		output.enableRandomWrite = true;
		output.Create(); 

        computeShader.SetVector("texsize", new Vector4(texSize.x, texSize.y, 0f, 0f));
        computeShader.SetVector("bounds", bounds);
        computeShader.SetFloat("ppu", pixelsPerUnit);
        computeShader.SetInt("blobcount", hairBlobs.Count);
        computeShader.SetVector("color", color);
        computeShader.SetTexture(kernel, "output", output);

        // Create Buffer
        Vector3[] blobs = new Vector3[hairBlobs.Count];
        for (int i = 0; i < blobs.Length; i++) {
            HairBlob blob = hairBlobs[i];
            blobs[i] = new Vector3(blob.position.x, blob.position.y, blob.radius);
        }
		ComputeBuffer blobBuffer = new ComputeBuffer(blobs.Length, sizeof(float)*3);
		blobBuffer.SetData(blobs);
		// Set Position Buffer to Kernel
		computeShader.SetBuffer(kernel, "blobs", blobBuffer);

        Vector3Int threads = new Vector3Int(8, 8, 1);
        computeShader.Dispatch(kernel, Mathf.CeilToInt(texSize.x/(float)threads.x), Mathf.CeilToInt(texSize.y/(float)threads.y), threads.z);

        // Release ComputeBuffer from memory
        blobBuffer.Release();

        Texture2D tex = output.ToTexture2D();
        tex.filterMode = FilterMode.Point;
        Vector2 center = Sinx.InvLerp(bounds.xy(), bounds.zw(), (Vector2)transform.position);
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), center, pixelsPerUnit);
        
        // Release RenderTexture and Texture2D from memory
        RenderTexture.ReleaseTemporary(output);
        if (previousTexture2D)
            DestroyImmediate(previousTexture2D);
        previousTexture2D = tex;

        return sprite;
    }

    void UpdateBlobPositions() {
        for (int i = 0; i < hairBlobs.Count; i++) {
            HairBlob hairBlob = hairBlobs[i];
            if (i == 0) {
                hairBlob.position = transform.position + (Vector3)hairBlob.offset;
            } else {
                HairBlob prevHairBlob = hairBlobs[i-1];
                hairBlob.position = Vector3.Lerp(hairBlob.position, prevHairBlob.position + (Vector3)hairBlob.offset, Time.deltaTime*stiffness);

                Vector3 difference = hairBlob.position - prevHairBlob.position;
                float mag = difference.magnitude;

                float blobSpan = hairBlob.radius + prevHairBlob.radius;
                mag = Mathf.Max(mag, blobSpan*minSeparation);
                mag = Mathf.Min(mag, blobSpan*maxSeparation);

                hairBlob.position = prevHairBlob.position + difference.normalized * mag;
            }
        
        }
    }

    Vector4 GetBoundingBox() {
        Vector2 min = Vector2.positiveInfinity;
        Vector2 max = Vector2.negativeInfinity;
        for (int i = 0; i < hairBlobs.Count; i++) {
            Vector2 pos = hairBlobs[i].position;
            Vector2 radius = hairBlobs[i].radius * Vector2.one;
            min = Vector2.Min(pos - radius, min);
            max = Vector2.Max(pos + radius, max);
        }
        min = (Vector2)Vector2Int.FloorToInt(min*pixelsPerUnit)/(float)pixelsPerUnit - pixelBuffer*Vector2.one/(float)pixelsPerUnit;
        max = (Vector2)Vector2Int.CeilToInt(max*pixelsPerUnit)/(float)pixelsPerUnit + pixelBuffer*Vector2.one/(float)pixelsPerUnit;
        return new Vector4(min.x, min.y, max.x, max.y);
    }

    public void InitializeBlobs() {

        Vector2 offset = Vector2.zero;
        for (int i = 0; i < hairBlobs.Count; i++) {
            offset += hairBlobs[i].offset;
            hairBlobs[i].position = (Vector3)offset + transform.position;
        }
    }

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitializeBlobs();
    }

    void Update() {

        Destroy(spriteRenderer.sprite);
        if (hairBlobs.Count > 0) {
            UpdateBlobPositions();
            spriteRenderer.sprite = GenerateSprite();
        } else {
            spriteRenderer.sprite = null;
        }

    }


    void OnDrawGizmos() {
        if (showGizmos) {
            for (int i = 0; i < hairBlobs.Count; i++) {
                HairBlob hairBlob = hairBlobs[i];
                Gizmos.DrawWireSphere(hairBlob.position, hairBlob.radius);
            }    
        }
    }
}

[Serializable]
public class HairBlob {
    [HideInInspector] public Vector3 position;
    public Vector2 offset;
    [Min(0f)] public float radius = .5f;
    public HairBlob(Vector2 offset, float radius) {
        this.offset = offset;
        this.radius = radius;
    }
}

}