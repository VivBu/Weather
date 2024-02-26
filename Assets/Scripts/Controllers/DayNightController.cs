using System;
using System.Linq;
using UnityEngine;

public class DayNightController : MonoBehaviour
{
    // Properties
    public GameObject SunMoonProxy 
    { 
        get => _sunMoonProxy;
        set => _sunMoonProxy = value;
    }

    public double CycleSpeedModifier
    {
        get => _cycleSpeedModifier;
        set => _cycleSpeedModifier = value;
    }

    public int StartHour
    {
        get => _startHour;
        set => _startHour = value;
    }

    public int SunriseHour
    {
        get => _sunriseHour;
        set => _sunriseHour = value;
    }

    public int SunsetHour
    {
        get => _sunsetHour;
        set => _sunsetHour = value;
    }

    public Light SunLight
    {
        get => _sunLight;
        set => _sunLight = value;
    }

    public Light MoonLight
    {
        get => _moonLight;
        set => _moonLight = value;
    }

    // Serialized Fields
    [SerializeField]
    [Tooltip(PropertyGridTexts.TT_SUN_MOON_PROXY)]
    private GameObject _sunMoonProxy;

    [SerializeField]
    [Tooltip(PropertyGridTexts.TT_CYCLE_SPEED)]
    private double _cycleSpeedModifier = 1d;

    [SerializeField]
    [Tooltip(PropertyGridTexts.TT_START_HOUR)]
    private int _startHour;

    [SerializeField]
    [Tooltip(PropertyGridTexts.TT_SUNRISE_HOUR)]
    private int _sunriseHour;

    [SerializeField]
    [Tooltip(PropertyGridTexts.TT_SUNSET_HOUR)]
    private int _sunsetHour;

    [SerializeField]
    [Tooltip(PropertyGridTexts.TT_SUN_LIGHT)]
    private Light _sunLight;

    [SerializeField]
    [Tooltip(PropertyGridTexts.TT_MOON_LIGHT)]
    private Light _moonLight;

    // fields
    private DateTime _currentTime;
    private TimeSpan _sunriseTime;
    private TimeSpan _sunsetTime;

    // Start
    private void Start()
    {
        // Set values initially
        _currentTime = DateTime.Now.Date.AddHours(Convert.ToDouble(StartHour));
        _sunriseTime = TimeSpan.FromHours(Convert.ToDouble(SunriseHour));
        _sunsetTime = TimeSpan.FromHours(Convert.ToDouble(SunsetHour));

        // Setup sun & moon rotation & direction
        SetupSunAndMoon();
    }

    // Update
    private void Update()
    {
        // Update the current time
        UpdateCurrentTime();

        // so we can update rotation accordingly
        UpdateProxyRotation();
    }

    private void UpdateProxyRotation()
    {
        float rotation;

        // calculate the rotation depending on the time of day
        // if current time is during the day
        if (_currentTime.TimeOfDay > _sunriseTime && _currentTime.TimeOfDay < _sunsetTime)
        {
            TimeSpan sunriseToSunsetDuration = GetTimeDiff(_sunriseTime, _sunsetTime);
            TimeSpan timeSinceSunrise = GetTimeDiff(_sunriseTime, _currentTime.TimeOfDay);

            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            rotation = Mathf.Lerp(0, 180, (float)percentage);
        }
        // or during the night
        else 
        {
            TimeSpan sunsetToSunriseDuration = GetTimeDiff(_sunsetTime, _sunriseTime);
            TimeSpan timeSinceSunset = GetTimeDiff(_sunsetTime, _currentTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;

            rotation = Mathf.Lerp(180, 360, (float)percentage);
        }

        // finally apply the rotation
        SunMoonProxy.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);
    }

    // Current time manipulation based off deltaTime & modifier
    private void UpdateCurrentTime()
    {
        _currentTime.AddSeconds(Time.deltaTime * CycleSpeedModifier);
    }

    // Rotation Manipulation
    private void SetupSunAndMoon()
    {
        // initially, make sure the sun as well as the moon are looking at the proxy (rotation center)
        foreach (var light in new[] { SunLight, MoonLight })
            light.transform.LookAt(SunMoonProxy.transform);
    }

    // calculates the time difference with respect to 24h format
    private static TimeSpan GetTimeDiff(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan difference = toTime - fromTime;

        if (difference.TotalSeconds < 0)
        {
            difference += TimeSpan.FromHours(24);
        }

        return difference;
    }
}
