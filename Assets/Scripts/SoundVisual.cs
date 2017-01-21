using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundVisual : MonoBehaviour {

	//public enum drawType {Line, Circle};
	[HideInInspector]
	public drawType type;
	[HideInInspector]
	public Transform drawParent;
	[HideInInspector]
	public GameObject prefab;

	private const int SAMPLE_SIZE = 1024;

	[HideInInspector]
	public float rmsValue;
	[HideInInspector]
	public float dbValue;
	[HideInInspector]
	public float pitchValue;

	[HideInInspector]
	public float maxVisualScale = 25.0f;
	[HideInInspector]
	public float visualModifier = 50.0f;
	[HideInInspector]
	public float smoothSpeed = 10.0f;
	[HideInInspector]
	public float keepPercentage = 0.5f;

	[HideInInspector]
	public float startVisualIndex = 0;

	[HideInInspector]
	public Transform centerCircle;
	[HideInInspector]
	public float circleRadius = 10.0f;

	[HideInInspector]
	public AudioSource source;
	private float[] samples;
	private float[] spectrum;
	private float sampleRate;

	private Transform[] visualList;
	private float[] visualScale;
	[HideInInspector]
	public int amnVisual = 64;
	[HideInInspector]
	public float blocScales = 2.0f;

	private void Start()
	{
		//source = GetComponent<AudioSource>();
		samples = new float[SAMPLE_SIZE];
		spectrum = new float[SAMPLE_SIZE];
		sampleRate = AudioSettings.outputSampleRate;

		if (type == drawType.Line)
			SpawnLine();
		else if (type == drawType.Circle)
			SpawnCircle();
	}
	private void SpawnLine()
	{
		visualScale = new float[amnVisual];
		visualList = new Transform[amnVisual];
		
			for (int i = 0; i < amnVisual; i++)
			{
				GameObject go = Instantiate(prefab, transform.position, Quaternion.identity);	
			//GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
				visualList[i] = go.transform;
				visualList[i].parent = drawParent;
				visualList[i].localPosition = Vector3.right * i * blocScales * 1.1f;
				visualList[i].localScale = new Vector3(blocScales, blocScales, blocScales);
			}
	}
	private void SpawnCircle()
	{
		visualScale = new float[amnVisual];
		visualList = new Transform[amnVisual];
		Vector3 center = centerCircle.position;
		float radius = circleRadius;
		
			for (int i = 0; i < amnVisual; i++)
			{
				float ang = i * 1.0f / amnVisual;
				ang = ang * Mathf.PI * 2;

				float x = center.x + Mathf.Cos(ang) * radius;
				float y = center.y + Mathf.Sin(ang) * radius;

				Vector3 pos = center + new Vector3(x, y, 0);
				GameObject go = Instantiate(prefab, transform.position, Quaternion.identity);
				go.transform.position = pos;
				go.transform.rotation = Quaternion.LookRotation(Vector3.forward, pos);
				visualList[i] = go.transform;
				visualList[i].localScale = new Vector3(blocScales, blocScales, blocScales);
				visualList[i].parent = drawParent;
			}
	}
	private void Update()
	{
		AnalyzeSound();
		UpdateVisual();
	}
	private void UpdateVisual()
	{
		int visualIndex = ChangeVisualIndex(startVisualIndex);
		int visualSpectrumIndex = 0;
		int spectrumIndex = 0;
		int averageSize = (int)(SAMPLE_SIZE * keepPercentage) / amnVisual;

		while (visualSpectrumIndex < amnVisual)
		{
			int j = 0;
			float sum = 0;
			while(j<averageSize)
			{
				sum += spectrum[spectrumIndex];
				spectrumIndex++;
				j++;
			}

			float scaleY = sum / averageSize * visualModifier;
			//Debug.Log("ScaleY: " + scaleY + " // length: " + visualScale.Length + " index : " + visualIndex + " // amnVisual : " + amnVisual);
			visualScale[visualIndex] = (visualScale[visualIndex] - Time.deltaTime * smoothSpeed) < scaleY ? scaleY : visualScale[visualIndex] - Time.deltaTime * smoothSpeed;
			//visualScale[visualIndex] -= Time.deltaTime * smoothSpeed;
			//if (visualScale[visualIndex] < scaleY)
			//	visualScale[visualIndex] = scaleY;

			visualScale[visualIndex] = visualScale[visualIndex] > maxVisualScale ? maxVisualScale : visualScale[visualIndex];
			//if (visualScale[visualIndex] > maxVisualScale)
			//	visualScale[visualIndex] = maxVisualScale;

			visualList[visualIndex].localScale = Vector3.one * blocScales + Vector3.up * visualScale[visualIndex];
			visualSpectrumIndex++;
			visualIndex++;
			if (visualIndex == amnVisual)
				visualIndex = 0;
		}
	}
	private void AnalyzeSound()
	{
		source.GetOutputData(samples, 0);

		//Get the RMS value
		int i = 0;
		float sum = 0;
		for (; i< SAMPLE_SIZE; i++)
		{
			sum += samples[i] * samples[i];
		}
		rmsValue = Mathf.Sqrt(sum / SAMPLE_SIZE);

		// Get de DB value
		dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);

		// Get sound Spectrum
		source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

		// Find pitch
		float maxV = 0;
		var maxN = 0;
		for (i=0; i< SAMPLE_SIZE; i++)
		{
			if (!(spectrum[i] > maxV) || !(spectrum[i] > 0.0f))
				continue;
			maxV = spectrum[i];
			maxN = i;
		}
		float freqN = maxN;
		if (maxN> 0 && maxN < SAMPLE_SIZE - 1)
		{
			var dL = spectrum[maxN - 1] / spectrum[maxN];
			var dR = spectrum[maxN + 1] / spectrum[maxN];
			freqN += 0.5f * (dR * dR - dL * dL);
		}
		pitchValue = freqN * (sampleRate / 2) / SAMPLE_SIZE;
	}
	private int ChangeVisualIndex(float index)
	{
		float ratio = index * amnVisual;
		int vi = (int)ratio;
		if (vi == amnVisual)
			vi = amnVisual-1;
		return vi;
	}
}
