using UnityEngine;
using System.Collections;

public class DayNightController : MonoBehaviour
{
	public float dayCycleLength;

	public float currentCycleTime;

	public DayPhase currentPhase;

	public float hoursPerDay;

	public float dawnTimeOffset;

	public int worldTimeHour;

	public Color fullLight;

	public Color fullDark;

	public Material dawnDuskSkybox;

	public Color dawnDuskFog;

	public Material daySkybox;

	public Color dayFog;

	public Material nightSkybox;

	public Color nightFog;
	private float dawnTime; 


	private float dayTime;

	private float duskTime;


	private float nightTime;

	private float quarterDay;

	private float lightIntensity;
	public Light light;
	void Initialize()
	{
		//remainingTransition = skyTransitionTime; //Would indicate that the game should start with an active transition, if UpdateSkybox were used.
		quarterDay = dayCycleLength * 0.25f;
		dawnTime = 0.0f;
		dayTime = dawnTime + quarterDay;
		duskTime = dayTime + quarterDay;
		nightTime = duskTime + quarterDay;
		if (light != null)
		{ lightIntensity = light.intensity; }
	}

	/// <summary>
	/// Sets the script control fields to reasonable default values for an acceptable day/night cycle effect.
	/// </summary>
	void Reset()
	{
		dayCycleLength = 120.0f;
		//skyTransitionTime = 3.0f; //would be set if UpdateSkybox were used.
		hoursPerDay = 24.0f;
		dawnTimeOffset = 3.0f;
		fullDark = new Color(32.0f / 255.0f, 28.0f / 255.0f, 46.0f / 255.0f);
		fullLight = new Color(253.0f / 255.0f, 248.0f / 255.0f, 223.0f / 255.0f);
		dawnDuskFog = new Color(133.0f / 255.0f, 124.0f / 255.0f, 102.0f / 255.0f);
		dayFog = new Color(180.0f / 255.0f, 208.0f / 255.0f, 209.0f / 255.0f);
		nightFog = new Color(12.0f / 255.0f, 15.0f / 255.0f, 91.0f / 255.0f);
		Skybox[] skyboxes = AssetBundle.FindObjectsOfTypeIncludingAssets(typeof(Skybox)) as Skybox[];
		foreach (Skybox box in skyboxes)
		{
			if (box.name == "DawnDusk Skybox")
			{ dawnDuskSkybox = box.material; }
			else if (box.name == "StarryNight Skybox")
			{ nightSkybox = box.material; }
			else if (box.name == "Sunny2 Skybox")
			{ daySkybox = box.material; }
		}
	}

	// Use this for initialization
	void Start()
	{
		Initialize();
	}

	// Update is called once per frame
	void Update()
	{
		// Rudementary phase-check algorithm:
		if (currentCycleTime > nightTime && currentPhase == DayPhase.Dusk)
		{
			SetNight();
		}
		else if (currentCycleTime > duskTime && currentPhase == DayPhase.Day)
		{
			SetDusk();
		}
		else if (currentCycleTime > dayTime && currentPhase == DayPhase.Dawn)
		{
			SetDay();
		}
		else if (currentCycleTime > dawnTime && currentCycleTime < dayTime && currentPhase == DayPhase.Night)
		{
			SetDawn();
		}

		// Perform standard updates:
		UpdateWorldTime();
		UpdateDaylight();
		UpdateFog();
		//UpdateSkybox(); //would be called if UpdateSkybox were used.

		// Update the current cycle time:
		currentCycleTime += Time.deltaTime;
		currentCycleTime = currentCycleTime % dayCycleLength;
	}

	/// <summary>
	/// Sets the currentPhase to Dawn, turning on the directional light, if any.
	/// </summary>
	public void SetDawn()
	{
		RenderSettings.skybox = dawnDuskSkybox; //would be commented out or removed if UpdateSkybox were used.
		//remainingTransition = skyTransitionTime; //would be set if UpdateSkybox were used.
		if (light != null)
		{ light.enabled = true; }
		currentPhase = DayPhase.Dawn;
	}

	/// <summary>
	/// Sets the currentPhase to Day, ensuring full day color ambient light, and full
	/// directional light intensity, if any.
	/// </summary>
	public void SetDay()
	{
		RenderSettings.skybox = daySkybox; //would be commented out or removed if UpdateSkybox were used.
		//remainingTransition = skyTransitionTime; //would be set if UpdateSkybox were used.
		RenderSettings.ambientLight = fullLight;
		if (light != null)
		{ light.intensity = lightIntensity; }
		currentPhase = DayPhase.Day;
	}

	/// <summary>
	/// Sets the currentPhase to Dusk.
	/// </summary>
	public void SetDusk()
	{
		RenderSettings.skybox = dawnDuskSkybox; //would be commented out or removed if UpdateSkybox were used.
		//remainingTransition = skyTransitionTime; //would be set if UpdateSkybox were used.
		currentPhase = DayPhase.Dusk;
	}

	/// <summary>
	/// Sets the currentPhase to Night, ensuring full night color ambient light, and
	/// turning off the directional light, if any.
	/// </summary>
	public void SetNight()
	{
		RenderSettings.skybox = nightSkybox; //would be commented out or removed if UpdateSkybox were used.
		//remainingTransition = skyTransitionTime; //would be set if UpdateSkybox were used.
		RenderSettings.ambientLight = fullDark;
		if (light != null)
		{ light.enabled = false; }
		currentPhase = DayPhase.Night;
	}

	/// <summary>
	/// If the currentPhase is dawn or dusk, this method adjusts the ambient light color and direcitonal
	/// light intensity (if any) to a percentage of full dark or full light as appropriate. Regardless
	/// of currentPhase, the method also rotates the transform of this component, thereby rotating the
	/// directional light, if any.
	/// </summary>
	private void UpdateDaylight()
	{
		if (currentPhase == DayPhase.Dawn)
		{
			float relativeTime = currentCycleTime - dawnTime;
			RenderSettings.ambientLight = Color.Lerp(fullDark, fullLight, relativeTime / quarterDay);
			if (light != null)
			{ light.intensity = lightIntensity * (relativeTime / quarterDay); }
		}
		else if (currentPhase == DayPhase.Dusk)
		{
			float relativeTime = currentCycleTime - duskTime;
			RenderSettings.ambientLight = Color.Lerp(fullLight, fullDark, relativeTime / quarterDay);
			if (light != null)
			{ light.intensity = lightIntensity * ((quarterDay - relativeTime) / quarterDay); }
		}

		transform.Rotate(Vector3.up * ((Time.deltaTime / dayCycleLength) * 360.0f), Space.Self);
	}

	/// <summary>
	/// Interpolates the fog color between the specified phase colors during each phase's transition.
	/// eg. From DawnDusk to Day, Day to DawnDusk, DawnDusk to Night, and Night to DawnDusk
	/// </summary>
	private void UpdateFog()
	{
		if (currentPhase == DayPhase.Dawn)
		{
			float relativeTime = currentCycleTime - dawnTime;
			RenderSettings.fogColor = Color.Lerp(dawnDuskFog, dayFog, relativeTime / quarterDay);
		}
		else if (currentPhase == DayPhase.Day)
		{
			float relativeTime = currentCycleTime - dayTime;
			RenderSettings.fogColor = Color.Lerp(dayFog, dawnDuskFog, relativeTime / quarterDay);
		}
		else if (currentPhase == DayPhase.Dusk)
		{
			float relativeTime = currentCycleTime - duskTime;
			RenderSettings.fogColor = Color.Lerp(dawnDuskFog, nightFog, relativeTime / quarterDay);
		}
		else if (currentPhase == DayPhase.Night)
		{
			float relativeTime = currentCycleTime - nightTime;
			RenderSettings.fogColor = Color.Lerp(nightFog, dawnDuskFog, relativeTime / quarterDay);
		}
	}

	//Not yet implemented, but would be nice to allow a smoother transition of the Skybox material.
	//private void UpdateSkybox()
	//{
	//    if (remainingTransition > 0.0f)
	//    {
	//        if (currentPhase == DayCycle.Dawn)
	//        {
	//            //RenderSettings.skybox.Lerp(dawnDuskSkybox, nightSkybox, remainingTransition / skyTransitionTime);
	//        }
	//        if (currentPhase == DayCycle.Day)
	//        {

	//        }
	//        if (currentPhase == DayCycle.Dusk)
	//        {

	//        }
	//        if (currentPhase == DayCycle.Night)
	//        {

	//        }
	//        remainingTransition -= Time.deltaTime;
	//    }
	//}

	/// <summary>
	/// Updates the World-time hour based on the current time of day.
	/// </summary>
	private void UpdateWorldTime()
	{
		worldTimeHour = (int)((Mathf.Ceil((currentCycleTime / dayCycleLength) * hoursPerDay) + dawnTimeOffset) % hoursPerDay) + 1;
	}

	public enum DayPhase
	{
		Night = 0,
		Dawn = 1,
		Day = 2,
		Dusk = 3
	}
}