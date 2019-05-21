﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Random = UnityEngine.Random;

public class PerlinGenerator : MonoBehaviour
{

    public int mDivisions;

    public float mSize;

    public float mHeight;

    private Vector3[] mVerts;
    private int mVertCount;

    private int heightScale = 5;
    public float detailScale = 5.0f;
    private readonly Stopwatch _timer = new Stopwatch();
    
    
    // Start is called before the first frame update
    void Start()
    {
        CreateSurface();
    }

    void CreateSurface()
    {
        mVertCount = (mDivisions + 1) * (mDivisions + 1);
        mVerts = new Vector3[mVertCount];
        Vector2[] uvs = new Vector2[mVertCount];
        int[] tris = new int[mDivisions * mDivisions * 6];
        
        float halfSize = mSize * 0.5f;
        float divisionSize = mSize / mDivisions;

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        int triOffset = 0;

        for (int i = 0; i <= mDivisions; i++)
        {
            for (int j = 0; j <= mDivisions; j++)
            {
                mVerts[i * (mDivisions + 1) + j] = new Vector3(-halfSize + j * divisionSize, 0.0f, halfSize - i * divisionSize);
                uvs[i * (mDivisions + 1) + j] = new Vector2((float) i / mDivisions, (float) j / mDivisions);

                if (i < mDivisions && j < mDivisions)
                {
                    int topLeft = i * (mDivisions + 1) + j;
                    int bottomLeft = (i + 1) * (mDivisions + 1) + j;

                    tris[triOffset] = topLeft;
                    tris[triOffset + 1] = topLeft + 1;
                    tris[triOffset + 2] = bottomLeft + 1;

                    tris[triOffset + 3] = topLeft;
                    tris[triOffset + 4] = bottomLeft + 1;
                    tris[triOffset + 5] = bottomLeft;

                    triOffset += 6;
                }
            }

        }
        
        _timer.Start();
       
        
//        var perlin = new Perlin();
//        for (int v = 0; v < mVerts.Length; v++)
//        {
//            mVerts[v].y = (float)perlin.perlin(
//                              (mVerts[v].x ) / detailScale,
//                              (mVerts[v].y) / detailScale,
//                              (mVerts[v].z)/detailScale) * heightScale;
//        }
      
        

	    TimeSpan ts = _timer.Elapsed;
	    UnityEngine.Debug.Log(ts.TotalMilliseconds);
	    _timer.Stop ();
        mesh.vertices = mVerts;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();


    }
}


public class Perlin {

	public int repeat;
	
	public Perlin(int repeat = -1) {
		this.repeat = repeat;
	}

	public double OctavePerlin(double x, double y, double z, int octaves, double persistence) {
		double total = 0;
		double frequency = 1;
		double amplitude = 1;
		double maxValue = 0;			// Used for normalizing result to 0.0 - 1.0
		for(int i=0;i<octaves;i++) {
			total += perlin(x * frequency, y * frequency, z * frequency) * amplitude;
			
			maxValue += amplitude;
			
			amplitude *= persistence;
			frequency *= 2;
		}
		
		return total/maxValue;
	}
	
	private static readonly int[] permutation = { 151,160,137,91,90,15,					
		131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,	
		190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
		88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
		77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
		102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
		135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
		5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
		223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
		129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
		251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
		49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
		138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
	};
	
	private static readonly int[] p; 													// Doubled permutation to avoid overflow
	
	static Perlin() {
		p = new int[512];
		for(int x=0;x<512;x++) {
			p[x] = permutation[x%256];
		}
	}
	
	public double perlin(double x, double y, double z) {
		int xi = (int)x & 255;								
		int yi = (int)y & 255;								
		int zi = (int)z & 255;								
		double xf = x-(int)x;								
		double yf = y-(int)y;
		double zf = z-(int)z;
		double u = fade(xf);
		double v = fade(yf);
		double w = fade(zf);
															
		int aaa, aba, aab, abb, baa, bba, bab, bbb;
		aaa = p[p[p[xi] + yi] + zi];
		aba = p[p[p[xi] + yi] + zi];
		aab = p[p[p[xi] + yi] + zi];
		abb = p[p[p[xi] + yi] + zi];
		baa = p[p[p[xi] + yi] + zi];
		bba = p[p[p[xi] + yi] + zi];
		bab = p[p[p[xi] + yi] + zi];
		bbb = p[p[p[xi] + yi] + zi];
	
		double x1, x2, y1, y2;
		x1 = lerp(	grad (aaa, xf  , yf  , zf),	
					grad (baa, xf-1, yf  , zf),	
					u);							
		x2 = lerp(	grad (aba, xf  , yf-1, zf),	
					grad (bba, xf-1, yf-1, zf),	
			          u);
		y1 = lerp(x1, x2, v);

		x1 = lerp(	grad (aab, xf  , yf  , zf-1),
					grad (bab, xf-1, yf  , zf-1),
					u);
		x2 = lerp(	grad (abb, xf  , yf-1, zf-1),
		          	grad (bbb, xf-1, yf-1, zf-1),
		          	u);
		y2 = lerp (x1, x2, v);
		
		return (lerp (y1, y2, w)+1)/2;						// For convenience we bound it to 0 - 1 (theoretical min/max before is -1 - 1)
	}
	
	public int inc(int num) {
		num++;
		if (repeat > 0) num %= repeat;
		
		return num;
	}
	
	public static double grad(int hash, double x, double y, double z) {
		int h = hash & 15;									
		double u = h < 8  ? x : y;				
		double v;											
		if(h < 4 )								
			v = y;
		else if(h == 12 || h == 14 )
			v = x;
		else 												
			v = z;
		return ((h&1) == 0 ? u : -u)+((h&2) == 0 ? v : -v); 
	}
	
	public static double fade(double t) {
															// Fade function as defined by Ken Perlin.  This eases coordinate values
															// so that they will "ease" towards integral values.  This ends up smoothing
															// the final output.
		return t * t * t * (t * (t * 6 - 15) + 10);			// 6t^5 - 15t^4 + 10t^3
	}
	
	public static double lerp(double a, double b, double x) {
		return a + x * (b - a);
	}
}