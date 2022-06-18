using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Sinx {
// Useful Functions
public struct Sinx {
    

    #region Vector2 Functions
        public static Vector2 Cis(float angle) {
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        public static Vector2 FromPolar(float angle, float magnitude) {
            return magnitude * Cis(angle);
        }

        public static float Dot(Vector2 v0, Vector2 v1) {
            return v0.x*v1.x + v0.y*v1.y;
        }
    #endregion


    #region Smoothstep
        public static float Smoothstep(float edge0, float edge1, float x) {
            float t = Mathf.Clamp01((x - edge0) / (edge1 - edge0));
            return t * t * (3f - 2f * t);
        }
    #endregion


    #region Lerp
        public static float Lerp(float a, float b, float t) {
            return (1f - t) * a + b * t;
        }
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t) {
            return (1f - t) * a + b * t;
        }
        public static Vector2 Lerp(Vector2 a, Vector2 b, Vector2 t) {
            return new Vector2(Lerp(a.x, b.x, t.x), Lerp(a.y, b.y, t.y));
        }
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t) {
            return (1f - t) * a + b * t;
        }
        public static Vector3 Lerp(Vector3 a, Vector3 b, Vector3 t) {
            return new Vector3(Lerp(a.x, b.x, t.x), Lerp(a.y, b.y, t.y), Lerp(a.z, b.z, t.z));
        }

        public static float LerpClamped(float a, float b, float t) {
            return Lerp(a, b, Mathf.Clamp01(t));
        }
        public static Vector2 LerpClamped(Vector2 a, Vector2 b, float t) {
            return Lerp(a, b, Mathf.Clamp01(t));
        }
        public static Vector2 LerpClamped(Vector2 a, Vector2 b, Vector2 t) {
            return new Vector2(LerpClamped(a.x, b.x, t.x), LerpClamped(a.y, b.y, t.y));
        }
        public static Vector3 LerpClamped(Vector3 a, Vector3 b, float t) {
            return Lerp(a, b, Mathf.Clamp01(t));
        }
        public static Vector3 LerpClamped(Vector3 a, Vector3 b, Vector3 t) {
            return new Vector3(LerpClamped(a.x, b.x, t.x), LerpClamped(a.y, b.y, t.y), LerpClamped(a.z, b.z, t.z));
        }
    #endregion


    #region InvLerp
        public static float InvLerp(float a, float b, float v) {
            // if (b - a == 0f) Debug.LogWarning("Warning: Division by Zero");
            if (b - a == 0f) return 0f;
            return (v - a) / (b - a);
        }
        public static Vector2 InvLerp(Vector2 a, Vector2 b, Vector2 v) {
            return new Vector2(InvLerp(a.x, b.x, v.x), InvLerp(a.y, b.y, v.y));
        }
        public static Vector3 InvLerp(Vector3 a, Vector3 b, Vector3 v) {
            return new Vector3(InvLerp(a.x, b.x, v.x), InvLerp(a.y, b.y, v.y), InvLerp(a.z, b.z, v.z));
        }

        public static float InvLerpClamped(float a, float b, float v) {
            return Mathf.Clamp01(InvLerp(a, b, v));
        }
        public static Vector2 InvLerpClamped(Vector2 a, Vector2 b, Vector2 v) {
            return new Vector2(InvLerpClamped(a.x, b.x, v.x), InvLerpClamped(a.y, b.y, v.y));
        }
        public static Vector3 InvLerpClamped(Vector3 a, Vector3 b, Vector3 v) {
            return new Vector3(InvLerpClamped(a.x, b.x, v.x), InvLerpClamped(a.y, b.y, v.y), InvLerpClamped(a.z, b.z, v.z));
        }
    #endregion

    #region Remap
        public static float Remap(float a0, float b0, float a1, float b1, float v) {
            return Lerp(a1, b1, InvLerp(a0, b0, v));
        }
        public static Vector2 Remap(Vector2 a0, Vector2 b0, Vector2 a1, Vector2 b1, Vector2 v) {
            return Lerp(a1, b1, InvLerp(a0, b0, v));
        }
        public static Vector3 Remap(Vector3 a0, Vector3 b0, Vector3 a1, Vector3 b1, Vector3 v) {
            return Lerp(a1, b1, InvLerp(a0, b0, v));
        }
    #endregion

    public static UnityEngine.Tilemaps.TileBase WorldToTile(Vector2 pos) {
        LayerMask layerMask = LayerMask.GetMask("Collision");
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f, layerMask);
        if (hit) {
            UnityEngine.Tilemaps.Tilemap tilemap = hit.transform.GetComponent<UnityEngine.Tilemaps.Tilemap>();
            Vector3Int cellPos = tilemap.WorldToCell(pos);
            return tilemap.GetTile(cellPos);
        }
        return null;
    }
}


public static class Extensions {

    public static Texture2D ToTexture2D(this RenderTexture rTex) {
    	RenderTexture oldRT = RenderTexture.active;
		Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA32, false);
		RenderTexture.active = rTex;
		tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
		tex.Apply();
		RenderTexture.active = oldRT;
		return tex;
    }

    public static bool ToBool(this int i) {
        return i != 0;
    }

    public static int ToInt(this bool b) {
        return b ? 1 : 0;
    }
    public static Vector2 ToPolar(this Vector2 vec) {
        return new Vector2(Mathf.Atan2(vec.y, vec.x), vec.magnitude);
    }

    public static float SnapToPixel(this float f, int pixelsPerUnit=16) {
        return Mathf.RoundToInt(f * pixelsPerUnit) / (float)pixelsPerUnit;
    }

    public static Vector2 SnapToPixel(this Vector2 v, int pixelsPerUnit=16) {
        return new Vector2(v.x.SnapToPixel(), v.y.SnapToPixel());
    }

    public static Vector2 Rotate(this Vector2 v, float angle, bool radians=true) {
        // Vector2 polar = v.ToPolar();
        // return Sinx.Cis(v.x + angle) * v.y;
        return v * Matrix2x2.CreateRotationMatrix(angle, radians);
    }

    public static Vector2 RotateAround(this Vector2 v, Vector2 pivot, float angle, bool radians=true) {
        return (v - pivot).Rotate(angle, radians) + pivot;
    }

    #region Swizzling
        public static Vector2 xx(this Vector2 v) { return new Vector2(v.x, v.x); }
        public static Vector2 xx(this Vector3 v) { return new Vector2(v.x, v.x); }
        public static Vector2 xy(this Vector2 v) { return new Vector2(v.x, v.y); }
        public static Vector2 xy(this Vector3 v) { return new Vector2(v.x, v.y); }
        public static Vector2 yx(this Vector2 v) { return new Vector2(v.y, v.x); }
        public static Vector2 yx(this Vector3 v) { return new Vector2(v.y, v.x); }
        public static Vector2 yy(this Vector2 v) { return new Vector2(v.y, v.y); }
        public static Vector2 yy(this Vector3 v) { return new Vector2(v.y, v.y); }

        public static Vector2 xo(this Vector2 v) { return new Vector2(v.x, 0f); }
        public static Vector2 ox(this Vector2 v) { return new Vector2(0f, v.x); }
        public static Vector2 yo(this Vector2 v) { return new Vector2(v.y, 0f); }
        public static Vector2 oy(this Vector2 v) { return new Vector2(0f, v.y); }

        public static Vector2 xx(this Vector4 v) { return new Vector2(v.x, v.x); }
        public static Vector2 xy(this Vector4 v) { return new Vector2(v.x, v.y); }
        public static Vector2 xz(this Vector4 v) { return new Vector2(v.x, v.z); }
        public static Vector2 xw(this Vector4 v) { return new Vector2(v.x, v.w); }
        public static Vector2 yx(this Vector4 v) { return new Vector2(v.y, v.x); }
        public static Vector2 yy(this Vector4 v) { return new Vector2(v.y, v.y); }
        public static Vector2 yz(this Vector4 v) { return new Vector2(v.y, v.z); }
        public static Vector2 yw(this Vector4 v) { return new Vector2(v.y, v.w); }
        public static Vector2 zx(this Vector4 v) { return new Vector2(v.z, v.x); }
        public static Vector2 zy(this Vector4 v) { return new Vector2(v.z, v.y); }
        public static Vector2 zz(this Vector4 v) { return new Vector2(v.z, v.z); }
        public static Vector2 zw(this Vector4 v) { return new Vector2(v.z, v.w); }
        public static Vector2 wx(this Vector4 v) { return new Vector2(v.w, v.x); }
        public static Vector2 wy(this Vector4 v) { return new Vector2(v.w, v.y); }
        public static Vector2 wz(this Vector4 v) { return new Vector2(v.w, v.z); }
        public static Vector2 ww(this Vector4 v) { return new Vector2(v.w, v.w); }
    #endregion
    
}

public struct Matrix2x2 {

    private Vector4 values;

    public Matrix2x2(float x, float y, float z, float w) {
        values.x = x; values.y = y; values.z = z; values.w = w;
    }

    public float this[int x, int y] {
        get => values[y*2 + x];
        set => values[y*2 + x] = value;
    }

    public static Matrix2x2 identity {
        get => new Matrix2x2(1f, 0f, 0f, 1f);
    }

    public float determinant {
        get => values.x*values.w - values.y*values.z;
    }

    public Matrix2x2 inverse {
        get => new Matrix2x2(values.w, -values.y, -values.z, values.x) / determinant;
    }

    public static Matrix2x2 operator* (Matrix2x2 m, float f) {
        return new Matrix2x2(f*m.values.x, f*m.values.y, f*m.values.z, f*m.values.w);
    }
    public static Matrix2x2 operator* (float f, Matrix2x2 m) {
        return m*f;
    }
    public static Matrix2x2 operator/ (Matrix2x2 m, float f) {
        return new Matrix2x2(m.values.x/f, m.values.y/f, m.values.z/f, m.values.w/f);
    }
    public static Matrix2x2 operator/ (float f, Matrix2x2 m) {
        return f * m.inverse;
    }
    public static Vector2 operator* (Vector2 v, Matrix2x2 m) {
        return new Vector2(v.x * m.values.x + v.y * m.values.y, v.x * m.values.z + v.y * m.values.w);
    }
    public static Matrix2x2 operator* (Matrix2x2 m0, Matrix2x2 m1) {
        return new Matrix2x2(
            m0.values.x*m1.values.x + m0.values.y*m1.values.z,
            m0.values.x*m1.values.y + m0.values.y*m1.values.w,
            m0.values.z*m1.values.x + m0.values.w*m1.values.z,
            m0.values.z*m1.values.y + m0.values.w*m1.values.w
        );
    }
    public static Matrix2x2 operator/ (Matrix2x2 m0, Matrix2x2 m1) {
        return m0 * m1.inverse;
    }

    public static Matrix2x2 CreateRotationMatrix(float angle, bool radians=true) {
        if (!radians) angle = angle * Mathf.Deg2Rad;
        Vector2 cis = Sinx.Cis(angle);
        return new Matrix2x2(cis.x, -cis.y, cis.y, cis.x);
    }

    public static Matrix2x2 CreateScaleMatrix(Vector2 scale) {
        return new Matrix2x2(scale.x, 0f, 0f, scale.y);
    }

    public static Matrix2x2 CreateSkewMatrix(Vector2 skew) {
        return new Matrix2x2(1f, skew.x, skew.y, 1f);
    }

    public override string ToString() => $"{values.x}, {values.y}, {values.z}, {values.w}";

}
}