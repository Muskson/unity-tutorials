﻿using UnityEngine;
using System.Collections;

public class Fractal : MonoBehaviour
{
	public Mesh[] meshes;
	public Material material;
	public int maxDepth;
	public float childScale;
	public float spawnProbability;
	public float maxRotationSpeed;
	public float maxTwist;

	private int depth;
	private static readonly Vector3[] childDirections = {
		Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
	private static readonly Quaternion[] childOrientations = {
		Quaternion.identity, Quaternion.Euler(0, 0, -90), Quaternion.Euler(0, 0, 90),
		Quaternion.Euler(90, 0, 0), Quaternion.Euler(-90, 0, 0)};
	private Material[,] materials;
	private float rotationSpeed;

	private void InitializeMaterials()
	{
		materials = new Material[maxDepth + 1, 2];
		for (int i = 0; i <= maxDepth; i++)
		{
			float t = i / (maxDepth - 1f);
			t *= t;
			materials[i, 0] = new Material(material);
			materials[i, 0].color = Color.Lerp(Color.white, Color.yellow, t);
			materials[i, 1] = new Material(material);
			materials[i, 1].color = Color.Lerp(Color.white, Color.cyan, t);
		}
		materials[maxDepth, 0].color = Color.magenta;
		materials[maxDepth, 1].color = Color.red;
	}

	void Start()
	{
		if (materials == null)
		{
			InitializeMaterials();
			new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, -1);
		}
		rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
		transform.Rotate(Random.Range(-maxTwist, maxTwist), 0f, 0f);
		gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
		gameObject.AddComponent<MeshRenderer>().material = material;
		GetComponent<MeshRenderer>().material = materials[depth, Random.Range(0, 2)];
		if (depth < maxDepth) StartCoroutine(CreateChildren());
	}

	private IEnumerator CreateChildren()
	{
		for (int i = 0; i < childDirections.Length; i++)
		{
			if (Random.value < spawnProbability)
			{
				yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
				new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, i);
			}
		}
	}

	private void Initialize(Fractal parent, int childIndex)
	{
		meshes = parent.meshes;
		materials = parent.materials;
		material = parent.material;
		maxDepth = parent.maxDepth;
		depth = parent.depth + 1;
		spawnProbability = parent.spawnProbability * 0.95f;
		maxRotationSpeed = parent.maxRotationSpeed;
		maxTwist = parent.maxTwist;
		childScale = parent.childScale;
		transform.parent = parent.transform;
		transform.localScale = Vector3.one * childScale;
		if (childIndex == -1)
		{
			transform.localRotation = Quaternion.Euler(0, 0, 180);
			transform.localPosition = Vector3.down * (0.5f + 0.5f * childScale);
		}
		else
		{
			transform.localRotation = childOrientations[childIndex];
			transform.localPosition = childDirections[childIndex] * (0.5f + 0.5f * childScale);
		}
	}

	private void Update()
	{
		transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
	}
}