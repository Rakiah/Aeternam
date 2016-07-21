using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;


public class RecoilRandomizer : EditorWindow 
{
	string buildUpKey;
	int nbBullet;
	int nbBulletByPattern;
	bool decrescendoX;
	bool decrescendoY;
	float decrescendoDecreaserX;
	float decrescendoDecreaserY;

	float minX;
	float maxX;

	float minY;
	float maxY;


	[MenuItem ("Aeternam/Recoil")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow (typeof(RecoilRandomizer));
	}

	void OnGUI ()
	{
		GUILayout.Label("Recoil Settings", EditorStyles.boldLabel);

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("nb bullets");
		nbBullet = EditorGUILayout.IntField(nbBullet);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Bullets by pattern");
		nbBulletByPattern = EditorGUILayout.IntField(nbBulletByPattern);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Decrescendo X");
		decrescendoX = EditorGUILayout.Toggle(decrescendoX);

		EditorGUILayout.LabelField("Decrescendo Y");
		decrescendoY = EditorGUILayout.Toggle(decrescendoY);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Decrescendo increasing X");
		decrescendoDecreaserX = EditorGUILayout.FloatField(decrescendoDecreaserX);

		EditorGUILayout.LabelField("Decrescendo increasing Y");
		decrescendoDecreaserY = EditorGUILayout.FloatField(decrescendoDecreaserY);

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("X Settings", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Min");
		minX = EditorGUILayout.FloatField(minX);
		EditorGUILayout.LabelField("Max");
		maxX = EditorGUILayout.FloatField(maxX);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Y Settings", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Min");
		minY = EditorGUILayout.FloatField(minY);
		EditorGUILayout.LabelField("Max");
		maxY = EditorGUILayout.FloatField(maxY);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
		if(GUILayout.Button("Build key"))
		{
			build();
		}

		for(int i = 0; i < 5; i++)
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("generated key",EditorStyles.boldLabel);
		buildUpKey = EditorGUILayout.TextArea(buildUpKey,GUILayout.Height(150));
	}

	void build ()
	{
		int bByPattern = nbBulletByPattern - 1;

		float newMinX = minX;
		float newMaxX = maxX;

		float newMinY = minY;
		float newMaxY = maxY;

		buildUpKey = "";
		for(int i = 0; i <= nbBullet; i++)
		{
			bByPattern++;
			if(bByPattern >= nbBulletByPattern)
			{
				buildUpKey += i.ToString();
				buildUpKey += '|';
				buildUpKey += (i+bByPattern).ToString();
				buildUpKey += '|';
				buildUpKey += System.Math.Round(Random.Range (minX,maxX), 2).ToString();
				buildUpKey += '|';
				buildUpKey += System.Math.Round(Random.Range (minY,maxY), 2).ToString();
				buildUpKey += '#';
				if(decrescendoX)
				{
					newMinX += decrescendoDecreaserX;
					newMaxX -= decrescendoDecreaserX;
				}
				if(decrescendoY)
				{
					newMinY += decrescendoDecreaserY;
					newMaxY -= decrescendoDecreaserY;
				}
				bByPattern = 0;
			}
		}
	}
}
