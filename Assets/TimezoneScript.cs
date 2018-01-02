using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;


public class TimezoneScript : MonoBehaviour {
    public KMBombInfo BombInfo;
    public KMBombModule BombModule;
    public KMAudio KMAudio;
    public KMSelectable[] buttons;
    public KMSelectable InputButton;
    public TextMesh TextToCity;
    public TextMesh TextFromCity;
    public TextMesh TextDisplay;
    public MeshRenderer PM;
    public MeshRenderer AM;
    public MeshRenderer h12;
    public MeshRenderer h24;
    public GameObject MinutesIndicator;
    public GameObject HoursIndicator;
    public bool isam;
    public bool is12;

    Dictionary<string, int> cities = new Dictionary<string, int>();

    int fromHour;
    int fromMinutes;
    int toHour;
    int toMinutes;
    string from;
    string to;

    int correctIndex;
    bool isActivated = false;

    private static int cModuleID = 1;
    private int moduleID;

    // Use this for initialization
    void Start() {
        this.cities.Add("Alofi", -11);
        this.cities.Add("Papeete", -10);
        this.cities.Add("Unalaska", -9);
        this.cities.Add("Whitehorse", -8);
        this.cities.Add("Denver", -7);
        this.cities.Add("Managua", -6);
        this.cities.Add("Quito", -5);
        this.cities.Add("Manaus", -4);
        this.cities.Add("Buenos Aires", -3);
        this.cities.Add("Sao Paulo", -2);
        this.cities.Add("Praia", -1);
        this.cities.Add("Edinburgh", 0);
        this.cities.Add("Berlin", 1);
        this.cities.Add("Bujumbura", 2);
        this.cities.Add("Moscow", 3);
        this.cities.Add("Tbilisi", 4);
        this.cities.Add("Lahore", 5);
        this.cities.Add("Omsk", 6);
        this.cities.Add("Bangkok", 7);
        this.cities.Add("Beijing", 8);
        this.cities.Add("Tokyo", 9);
        this.cities.Add("Brisbane", 10);
        this.cities.Add("Sydney", 11);
        this.cities.Add("Tarawa", 12);

        this.is12 = (Random.value > 0.5f);

        this.moduleID = cModuleID++;

        Init();

        GetComponent<KMBombModule>().OnActivate += ActivateModule;
    }

    void Init()
    {

        if (is12)
        {
            h12.material.color = Color.green;
        } else
        {
            h24.material.color = Color.green;
        }

        List<string> keys = new List<string>(this.cities.Keys);
        this.fromHour = Random.Range(0, 23);
        this.fromMinutes = Random.Range(0, 12) * 5;

        

        if (this.fromHour < 12)
        {
            this.isam = true;
        }

        if (isam)
        {
            AM.material.color = Color.green;
        }
        else
        {
            PM.material.color = Color.green;
        }

        this.HoursIndicator.transform.Rotate(new Vector3(0,30 * Format12h(this.fromHour) + this.fromMinutes/2, 0));
        this.MinutesIndicator.transform.Rotate(new Vector3(0, 6 * this.fromMinutes, 0));

        this.from = keys[Random.Range(0, keys.Count)];
        this.to = keys[Random.Range(0, keys.Count)];
        while (string.Equals(from, to))
        {
            to = keys[Random.Range(0, keys.Count)];
        }
        
        // Get correct answer
        this.toHour = ConvertHour(this.fromHour, this.cities[from], this.cities[to]);
        this.toMinutes = this.fromMinutes;

        Debug.Log("[Timezones #" + this.moduleID + "] From: " + this.from + " (UTC " + this.cities[from] + ") - To: " + this.to + " (UTC " + this.cities[to] + ")");
        Debug.Log("[Timezones #" + this.moduleID + "] Initial time: " + FormatTwoDigits(this.fromHour) + ":" + FormatTwoDigits(this.fromMinutes));
        Debug.Log("[Timezones #" + this.moduleID + "] Converted time: " + FormatTwoDigits(this.toHour) + ":" + FormatTwoDigits(this.toMinutes));
        
        if (this.is12)
        {
            Debug.Log("[Timezones #" + this.moduleID + "] Answer requested in 12h format.");
        } else
        {
            Debug.Log("[Timezones #" + this.moduleID + "] Answer requested in 24h format.");
        }

        // Change cities display text
        this.TextToCity.text = this.to;
        this.TextFromCity.text = this.from;

        // Register buttons input
        for (int i = 0; i < this.buttons.Length; i++)
        {
            //TextMesh buttonText = buttons[i].GetComponentInChildren<TextMesh>();

            int j = i;
            buttons[i].OnInteract += delegate () { OnNumberPress(j); return false; };
        }

        // Register intro input
        this.InputButton.OnInteract += delegate () { OnIntroPress(); return false; };

    }

    // Update is called once per frame
    void Update() {

    }

    void ActivateModule()
    {
        isActivated = true;
    }

    int ConvertHour(int hour, int UTC1, int UTC2)
    {
        hour = hour - UTC1 + UTC2;
        hour = (hour + 24) % 24;

        return hour;
    }

    void pushNumberToDisplay(int number)
    {
        this.TextDisplay.text = this.TextDisplay.text.Substring(1) + number;
    }

    int HourAngle(int hour)
    {

        return 0;
    }


    void OnNumberPress(int buttonNumber)
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        GetComponent<KMSelectable>().AddInteractionPunch();
        if (!isActivated)
        {
            GetComponent<KMBombModule>().HandleStrike();
        }
        else
        {
            pushNumberToDisplay(buttonNumber);
        }
    }

    void OnIntroPress()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        GetComponent<KMSelectable>().AddInteractionPunch();
        string correctTime;

        if (is12)
        {
            string correctHour;
            if (this.toHour == 0) correctHour = "12";
            else correctHour = FormatTwoDigits(Format12h(this.toHour));
            correctTime = correctHour + FormatTwoDigits(this.toMinutes);
        }
        else
        {
            correctTime = FormatTwoDigits(this.toHour) + FormatTwoDigits(this.toMinutes);
        }

        

        if (correctTime.Equals(this.TextDisplay.text))
        {
            Debug.Log("[Timezones #" + this.moduleID + "] Module passed. Answer: " + correctTime + ".");
            BombModule.HandlePass();
        }
        else
        {
            Debug.Log("[Timezones #" + this.moduleID + "] Submitted answer: " + this.TextDisplay.text + ". Correct answer: " + correctTime + ". Issued strike.");
            BombModule.HandleStrike();
        }
    }

    string FormatTwoDigits(int num)
    {
        if (num / 10 < 1)
        {
            return "0" + num;
        }
        else
        {
            return num.ToString();
        }   
    }

    int Format12h(int num)
    {
        return num % 12;
    }

    KMSelectable[] ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant().Trim();

        if (Regex.IsMatch(command, @"^submit \d\d\d\d$"))
        {
            command = command.Substring(7).Trim();

            return new KMSelectable[] { this.buttons[(int)System.Char.GetNumericValue(command[0])], this.buttons[(int)System.Char.GetNumericValue(command[1])], this.buttons[(int)System.Char.GetNumericValue(command[2])], this.buttons[(int)System.Char.GetNumericValue(command[3])], InputButton };
        }

        return null;
    }

}

