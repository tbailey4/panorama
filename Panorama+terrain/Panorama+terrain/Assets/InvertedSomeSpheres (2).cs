using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Windows;

public class InvertedSomeSpheres : EditorWindow
{
    private int numSpheres = 16;
    //private int numSpheres = 5;
    //	private string st = "1.0";
    //	private string st1 = "1.0";
    //	private string st2 = "";
    //	private string st3 = "";
    //	private string st4 = "";
    //	private string st5 = "";
    //	private string st6 = "";

    //button in the menu, can be deleted to integrate this script with others.
    [MenuItem("GameObject/Create Other/Generate Panoramic Spheres")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(InvertedSomeSpheres));
	}
		
	public void OnGUI()
		{
			String[] location;

			// Enter panoramas name manually in unity 
			//		GUILayout.Label("Enter panoramas name:");
			//		st2 = GUILayout.TextField(st2);
			//		GUILayout.Label("Enter panoramas name:");
			//		st3 = GUILayout.TextField(st3);
			//		GUILayout.Label("Enter panoramas name:");
			//		st4 = GUILayout.TextField(st4);
			//		GUILayout.Label("Enter panoramas name:");
			//		st5 = GUILayout.TextField(st5);
			//		GUILayout.Label("Enter panoramas name:");
			//		st6 = GUILayout.TextField(st6);

			//Set panoramas names and amount in script. default 5 spheres
			location = new String[numSpheres];

        for (int i = 1; i <= numSpheres; i++)
        {
            string name = "x5_y5_" + i;
            location[i - 1] = name;
//            location[0] = "x5_y5_0";
//            location[0] = "two";
//            location[1] = "three";
//            location[2] = "four";
//            location[3] = "five";
//            location[4] = "six";
        }


			//Enter sphere size manually in unity 
			//		GUILayout.Label("Enter size:");
			//		st = GUILayout.TextField(st);
			
			//Used to set amount of spheres but no need now
			//		GUILayout.Label("Enter amount:");
			//		st1 = GUILayout.TextField(st1);

		float f;
		int s;
		// Initial sphere size		
		f = 124;
//			if (!float.TryParse(st, out f))
//				f = 1.0f;
			//		if (!float.TryParse(st1, out s))
			//			s = 1.0f;


		if (GUILayout.Button ("Create Inverted Spheres")) {//button in the menu, can be deleted to integrate this script with others.
				for (int i = 0; i < numSpheres; i++) { // default five spheres
					s = i;
					CreateInvertedSphere (f + i * 31, location[i], s); //distance between 2 spheres is 100 
				}
			}
		}

    private void CreateInvertedSphere(float size, string panorama, int s)
	{

		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		MeshFilter mf = go.GetComponent<MeshFilter>();
		Mesh mesh = mf.sharedMesh;

		GameObject goNew = new GameObject();
		goNew.name = "Inverted Sphere";
		MeshFilter mfNew = goNew.AddComponent<MeshFilter>();
		mfNew.sharedMesh = new Mesh();


		//Scale the vertices;
		Vector3[] vertices = mesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
			vertices[i] = vertices[i] * size;
		mfNew.sharedMesh.vertices = vertices;

		// Reverse the triangles
		int[] triangles = mesh.triangles;
		for (int i = 0; i < triangles.Length; i += 3)
		{
			int t = triangles[i];
			triangles[i] = triangles[i + 2];
			triangles[i + 2] = t;
		}
		mfNew.sharedMesh.triangles = triangles;

		// Reverse the normals;
		Vector3[] normals = mesh.normals;
		for (int i = 0; i < normals.Length; i++)
			normals[i] = -normals[i];
		mfNew.sharedMesh.normals = normals;


		mfNew.sharedMesh.uv = mesh.uv;
		mfNew.sharedMesh.uv2 = mesh.uv2;
		mfNew.sharedMesh.RecalculateBounds();

		//import Panorams and set transparency 
		Material material = new Material (Shader.Find ("Standard"));
		Texture2D tex = null;
        Texture2D texture = null;
        //TextureImporter A = (TextureImporter)AssetImporter.GetAtPath(panorama);
        //A.isReadable = true;
        texture = Resources.Load(panorama) as Texture2D; // get the texture here
        tex = texture;
//        for (int y = 0; y < tex.height; y++)
//        {
//            for (int x = 0; x < tex.width; x++)
//            {
//                if (tex.GetPixel(x, y).r == 1 && tex.GetPixel(x, y).g == 1 && tex.GetPixel(x, y).b == 1)
//                {
//                    tex.SetPixel(x, y, new Color(0, 0, 0, 0));
//                }
//            }
//        }
//        tex.Apply();
        		//tex.wrapMode.set(Clamp);
        tex.wrapMode = TextureWrapMode.Clamp;
		//Texture.filterMode.Point;
		tex.filterMode = FilterMode.Point;
		//tex.Resize(128, 128, TextureFormat.RGBA32, false);
		material.mainTexture = tex;
		material.SetFloat("_Mode", 1);
		material.SetFloat("_Cutoff", 1);
		material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		material.SetInt("_ZWrite", 0);
		material.DisableKeyword("_ALPHATEST_ON");
		material.EnableKeyword("_ALPHABLEND_ON");
		material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		material.renderQueue = 3000;

		//Renderer rend = goNew.GetComponent<Renderer> ();
		Shader shader = Shader.Find("Transparent/Diffuse");
		material.shader = shader;

		AssetDatabase.CreateAsset (material, "Assets/Sphere"+s+".mat");

		// Add the same material that the original sphere used
		MeshRenderer mr = goNew.AddComponent<MeshRenderer>();
		mr.sharedMaterial = material;

		DestroyImmediate(go);
	}
}

