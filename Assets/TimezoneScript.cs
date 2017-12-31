using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Debug.Log("ES AM");
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

        this.HoursIndicator.transform.Rotate(new Vector3(0,30 * Format12h(this.fromHour), 0));
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

        Debug.Log("Hora inicial: " + FormatTwoDigits(this.fromHour) + ":" + FormatTwoDigits(this.fromMinutes));
        Debug.Log("Hora convertida: " + FormatTwoDigits(this.toHour) + ":" + FormatTwoDigits(this.toMinutes));

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
        Debug.Log("DE: " + UTC1 + " - A: " + UTC2 + " - DA: " + hour);
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
            Debug.Log("Pressed button before module has been activated!");
            GetComponent<KMBombModule>().HandleStrike();
        }
        else
        {
            Debug.Log("Pressed " + buttonNumber + " button");
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
            correctTime = FormatTwoDigits(Format12h(this.toHour)) + FormatTwoDigits(this.toMinutes);
        }
        else
        {
            correctTime = FormatTwoDigits(this.toHour) + FormatTwoDigits(this.toMinutes);
        }
        

        if (correctTime.Equals(this.TextDisplay.text))
        {
            BombModule.HandlePass();
        }
        else
        {
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
}

