using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum drawType { Line, Circle };
public class TerrainWaveManager : MonoBehaviour {

	public Vector3 baseRotation;
	public GameObject wavePrefab;
	public drawType terrainType;
	public enum modifierType { equal, randomEqual, randomSeperate, delayed, progressive }
	public modifierType generationType;
	public enum indexModifier { clock, anticlock, fix }
	public indexModifier indexOverTime;
	public float indexSpeed;
	public float randomSmooth;
	public float typeDelay;
	public float typeProgressive;
	public bool changeSettingsDynamicly;

	public float maxVisualScale = 25.0f;
	public float visualModifier = 50.0f;
	public float smoothSpeed = 10.0f;
	public float keepPercentage = 0.5f;

	[Range(0, 1)]
	public float startVisualIndex = 0;

	public Transform centerCircle;
	public float circleRadius = 10.0f;

	public AudioSource source;
	private float[] samples;
	private float[] spectrum;
	private float sampleRate;

	private Transform[] visualList;
	private float[] visualScale;
	public int amnVisual = 64;
	public float blocScales = 2.0f;

	public int width;
	public int length;

	private modifierType previousType;

	List<SoundVisual> ranges = new List<SoundVisual>();

	// Use this for initialization
	IEnumerator Start () {
		SpawnWaveRange();
		if (generationType == modifierType.randomSeperate)
		{
			for (int i =0; i<ranges.Count; i++)
			{
				ranges[i].startVisualIndex = Random.value;
			}
		}
		yield return null;
		transform.rotation = Quaternion.LookRotation(Vector3.up);
	}

	// Update is called once per frame
	void Update() {
		if (changeSettingsDynamicly)
		{ 
			for (int i = 0; i < ranges.Count; i++)
			{
				SoundVisual sd = ranges[i].GetComponent<SoundVisual>();
				ApplySettingsDynamicly(sd);
			}
		}
		ModifierTypeSelection();

		ModifierIndex();
	}

	void ModifierTypeSelection()
	{
		if (previousType != generationType && generationType == modifierType.randomSeperate)
		{
			for (int i = 0; i < ranges.Count; i++)
			{
				ranges[i].startVisualIndex = Random.value;
			}
		}

		if (generationType == modifierType.equal)
		{
			for (int i = 0; i < ranges.Count; i++)
				ranges[i].startVisualIndex = startVisualIndex;
		}
		else if (generationType == modifierType.randomEqual)
		{
			for (int i = 0; i < ranges.Count; i++)
			{
				float random = Mathf.Lerp(startVisualIndex, (Random.value), Time.deltaTime * randomSmooth);
				ranges[i].startVisualIndex = random;
			}
		}
		else if (generationType == modifierType.randomSeperate)
		{
			for (int i = 0; i < ranges.Count; i++)
			{
				float newValue = Mathf.Lerp(ranges[i].startVisualIndex, ranges[i].startVisualIndex + startVisualIndex > 1 ? ranges[i].startVisualIndex + startVisualIndex - 1 : ranges[i].startVisualIndex + startVisualIndex, Time.deltaTime * randomSmooth);
				ranges[i].startVisualIndex = newValue;
			}
		}
		else if (generationType == modifierType.delayed)
		{
			for (int i = 0; i < ranges.Count; i++)
			{
				float delay = 1f / width * typeDelay;
				float newValue = Mathf.Lerp(startVisualIndex, startVisualIndex + delay * i > 1 ? startVisualIndex + delay * i - 1 : startVisualIndex + delay * i, Time.deltaTime * randomSmooth);
				//float newValue = Mathf.Lerp(ranges[i].startVisualIndex, startVisualIndex + i * delay > 1 ? startVisualIndex + i * delay - 1 : startVisualIndex + i * delay, Time.deltaTime * randomSmooth);
				ranges[i].startVisualIndex = newValue;
			}
		}
		else if (generationType == modifierType.progressive)
		{
			for (int i = 0; i < ranges.Count; i++)
			{
				float addition = ranges[i].startVisualIndex + Random.Range(-typeProgressive, typeProgressive);
				float newValue = Mathf.Lerp(ranges[i].startVisualIndex, addition > 1 ? addition - 1 : addition, Time.deltaTime * randomSmooth);
				ranges[i].startVisualIndex = newValue;
			}
		}
		previousType = generationType;
	}
	void ModifierIndex()
	{
		if (indexOverTime == indexModifier.clock)
		{
			if (startVisualIndex >= indexSpeed)
				startVisualIndex -= indexSpeed;
			else
				startVisualIndex = 1f;
		}
		else if (indexOverTime == indexModifier.anticlock)
		{
			if (startVisualIndex <= 1-indexSpeed)
				startVisualIndex += indexSpeed;
			else
				startVisualIndex = 0f;
		}
		else if (indexOverTime == indexModifier.fix)
		{

		}
	}

	void SpawnWaveRange()
	{
		for (int i = 0; i < length; i++)
		{

			GameObject go = Instantiate(wavePrefab, transform.position, Quaternion.identity);
			go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, go.transform.position.z + (i) * blocScales * 1.1f);
			go.transform.parent = transform;
			go.gameObject.name = "Range " + i;
			SoundVisual sd = go.GetComponent<SoundVisual>();
			ApplySettingsDynamicly(sd);

			ranges.Add(go.GetComponent<SoundVisual>());
		}
	}
	void ApplySettingsDynamicly(SoundVisual sd)
	{
		sd.source = source;
		sd.drawParent = sd.transform;
		sd.centerCircle = sd.transform;
		sd.type = terrainType;
		sd.maxVisualScale = maxVisualScale;
		sd.visualModifier = visualModifier;
		sd.smoothSpeed = smoothSpeed;
		sd.keepPercentage = 0.5f;
		sd.startVisualIndex = startVisualIndex;
		sd.circleRadius = circleRadius;
		sd.amnVisual = width;
		sd.blocScales = blocScales;
	}
}
