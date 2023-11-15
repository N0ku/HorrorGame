using UnityEngine;
using TMPro;

public enum FlashlightState
{
    On,
    Off,
    OutOfBattery
}


[RequireComponent(typeof(AudioSource))]

public class FlashlightManager : MonoBehaviour
{
    TextMeshProUGUI mText;

    [Header("Options")]
    [SerializeField]
    float batteryLostTick = 0.1f;

    float batteryWinTick = 0.1f;

    [SerializeField]
    int startBattery = 100;

    private int currentBattery;

    public FlashlightState state;

    public static bool flashlightIsOn;

    public static bool isUsable = true;

    [SerializeField]
    KeyCode toggleFlashlightKey = KeyCode.F;

    [Header("References")]

    [SerializeField]
    private GameObject FlashlightLight;

    AudioClip FlashlightOn_FX, FlashlightOff_FX;

    public void Start()
    {
        currentBattery = startBattery;
        state = FlashlightState.On;
        flashlightIsOn = true;
        mText = FindObjectOfType<TextMeshProUGUI>();

        InvokeRepeating(nameof(LoseBattery), 0, batteryLostTick);
    }

    public void Update()
    {
        if (Input.GetKeyDown(toggleFlashlightKey)) ToggleFlashlight();

        if (state == FlashlightState.Off) FlashlightLight.SetActive(false);
        else if (state == FlashlightState.OutOfBattery) FlashlightLight.SetActive(false);
        else if (state == FlashlightState.On && isUsable == true) FlashlightLight.SetActive(true);

        if (state == FlashlightState.OutOfBattery && currentBattery >= startBattery)
        {
            isUsable = true;
            mText.color = Color.green;
        }

        if (isUsable == true && currentBattery >= 70)
        {
            mText.color = Color.green;
        }

        if (isUsable == true && currentBattery <= 30)
        {
            mText.color = Color.yellow;
        }

        int randomAmount = Random.Range(10, 25);
        if (didItBug == false && currentBattery == randomAmount)
        {
            bugFlashlight(FlashlightLight);
        }

        mText.text = "Flashlight: " + currentBattery + "%";
    }

    // On input pressed check the status of the flashlight and reload it
    public void ReloadFlashlight()
    {
        if (state == FlashlightState.Off)
        {
            // Debug.Log("Reloading Off " + currentBattery);
            InvokeRepeating(nameof(GainBattery), 0, batteryWinTick);
        }

        if (state == FlashlightState.OutOfBattery)
        {
            // Debug.Log("Reloading OutOfBattery " + currentBattery);
            InvokeRepeating(nameof(GainBattery), 0, batteryWinTick);

        }
    }

    private void GainBattery()
    {
        if (currentBattery > 50f)
        {
            didItBug = false;
        }
        // Debug.Log("Gaining battery " + currentBattery);
        if (currentBattery < startBattery)
        {
            if (state == FlashlightState.OutOfBattery)
            {
                isUsable = false;
                mText.color = Color.red;
            }
            currentBattery++;
        }
        else if (currentBattery >= startBattery)
        {
            currentBattery = startBattery;
            isUsable = true;
            flashlightIsOn = false;
            CancelInvoke(nameof(GainBattery));
        }
    }

    private void LoseBattery()
    {
        if (state == FlashlightState.On)
        {
            // Debug.Log("Losing battery " + currentBattery);
            currentBattery--;

            if (currentBattery <= 0)
            {
                // Debug.Log("OUT OF BATTERY " + currentBattery);
                currentBattery = 0;
                state = FlashlightState.OutOfBattery;
                isUsable = false;
                InvokeRepeating(nameof(GainBattery), 0, batteryWinTick);
                CancelInvoke(nameof(LoseBattery));
            }
        }
        else if (state == FlashlightState.Off)
        {
            Invoke(nameof(GainBattery), 0);
        }
    }

    private void ToggleFlashlight()
    {
        flashlightIsOn = !flashlightIsOn;

        if (isUsable)
        {
            if (IsInvoking(nameof(GainBattery))) CancelInvoke(nameof(GainBattery));
        }

        if (flashlightIsOn && isUsable)
        {
            // GetComponent<AudioSource>().PlayOneShot(FlashlightOn_FX);
            state = FlashlightState.On;
            if (!IsInvoking(nameof(LoseBattery)))
            {
                InvokeRepeating(nameof(LoseBattery), 0, batteryLostTick);
            }
        }
        else if (isUsable)
        {
            // GetComponent<AudioSource>().PlayOneShot(FlashlightOff_FX);
            state = FlashlightState.Off;
        }
    }

    public void bugFlashlight(GameObject FlashlightLight)
    {
        if (didItBug == false)
        {
            float timing = Random.Range(0.2f, 1.5f);
            disableFlashlight();
            Invoke(nameof(enableFlashlight), timing);

            didItBug = true;

            CancelInvoke(nameof(bugFlashlight));
        }

    }

    public void disableFlashlight()
    {
        FlashlightLight.SetActive(false);
        isUsable = false;
        flashlightIsOn = false;
        CancelInvoke(nameof(LoseBattery));
    }

    public void enableFlashlight()
    {
        FlashlightLight.SetActive(true);
        isUsable = true;
        flashlightIsOn = true;
        InvokeRepeating(nameof(LoseBattery), 0, batteryLostTick);
    }
}
