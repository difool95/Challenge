using UnityEngine;
using UnityEngine.UI;

 class GameMan : MonoBehaviour
{
    [Header("Managers")]
    public LineManager lineManager;
    public UIManager uIManager;

    [Space]
    bool timeup;
    bool popupWindowIsOpen;
    public Canvas canvas;
    public GameObject winPopup, loosePopup;
    
    public Text coinsText, TimerText;
    [Header("Slider")]
    public Slider slider;
    private float targetProgress = 0;
    private float fillSpeed = 0.5f;
    private float round = 1f;
    private float timeRemaining;
    private readonly int timerValue = 60;
    private int coins;
    private int attemptNumber = 1;

    // Start is called before the first frame update
    void Start()
    {
        coins = PlayerPrefs.GetInt("coins");
        coinsText.text = coins + "";
        timeRemaining = timerValue;
    }

    // Update is called once per frame
    void Update()
    {
        //SLIDER
        if (slider.value < targetProgress)
        {
            slider.value += fillSpeed * Time.deltaTime;
        }

        //TIMER
        if(timeRemaining > 1 && !popupWindowIsOpen)
        {
            timeRemaining -= Time.deltaTime;
            TimerText.text = Mathf.FloorToInt(timeRemaining %60).ToString();
        }
        else if(timeRemaining < 1 && !popupWindowIsOpen)
        {
            LoosePopup();
            timeRemaining = 1;
            popupWindowIsOpen = true;
        }
        if(timeRemaining < 5 && !timeup)
        {
            TimerText.GetComponent<Animator>().SetBool("timeUp", true);
            timeup = true;
        }
        /////////////////////////////////////////////////////////////////////////
    }

    public void CheckResults()
    {
        if (lineManager.canCheckResult && lineManager.didAnError)
        {
            LoosePopup();
        }
        else if (lineManager.canCheckResult && !lineManager.didAnError)
        {
            GameObject o = Instantiate(winPopup, canvas.transform.position, Quaternion.identity);
            o.transform.parent = canvas.transform;
            popupWindowIsOpen = true;
            if (attemptNumber == 1)
            {
                uIManager.StarUI(3, o);
                coins += 100;
                ResetTimerCoins();
                o.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 225, 100);
                o.transform.GetChild(1).GetComponent<Button>().interactable = false;

            }
            else if (1 < attemptNumber  && attemptNumber < 4)
            {
                coins += 60;
                uIManager.StarUI(2, o);
                o.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
                {
                    ResetTimerCoins();
                    ResetRound();
                    attemptNumber = 1;
                    popupWindowIsOpen = false;
                    o.GetComponent<popup>().close();
                });
            }
            else if (3 < attemptNumber && attemptNumber < 6)
            {
                coins += 30;
                uIManager.StarUI(1, o);
                o.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
                {
                    ResetTimerCoins();
                    ResetRound();
                    attemptNumber = 1;
                    popupWindowIsOpen = false;
                    o.GetComponent<popup>().close();
                });
            }
            else if (6 < attemptNumber)
            {
                coins += 10;
                o.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
                {
                    ResetTimerCoins();
                    ResetRound();
                    attemptNumber = 1;
                    popupWindowIsOpen = false;
                    o.GetComponent<popup>().close();
                });
            }
            o.transform.GetChild(0).GetComponent<TintedButton>().onClick.AddListener(() =>
            {
                //NEXT
                if(round == 10)
                {
                    //RESET LEVEL
                    LevelManager._instance.StartRetryMenu();
                }
                else
                {
                    IncrementProgress(0.1f);
                    round += 1;
                    ResetRound();
                    uIManager.DisplayImagesTexts();
                    attemptNumber = 1;
                    timeRemaining = timerValue;
                    timeup = false;
                    TimerText.GetComponent<Animator>().SetBool("timeUp", false);
                    popupWindowIsOpen = false;
                    o.GetComponent<popup>().close();
                }
            });
        }
    }

    public void ResetRound()
    {
        uIManager.ResetRoundTags();
        lineManager.ResetLineRenderers();
        lineManager.didAnError = false;
        lineManager.canCheckResult = false;
    }

    public void LoosePopup()
    {
        GameObject o = Instantiate(loosePopup, canvas.transform.position, Quaternion.identity);
        o.transform.parent = canvas.transform;
        popupWindowIsOpen = true;
        o.transform.GetChild(0).GetComponent<TintedButton>().onClick.AddListener(() =>
        {
            ResetRound();
            attemptNumber += 1;
            ResetTimerCoins();
            popupWindowIsOpen = false;
            o.GetComponent<popup>().close();
        });
    }

    public void IncrementProgress(float newProgress)
    {
        targetProgress = slider.value + newProgress;
    }

    public void ResetTimerCoins()
    {
        timeRemaining = timerValue;
        timeup = false;
        TimerText.GetComponent<Animator>().SetBool("timeUp", false);
        coinsText.text = coins + "";
        PlayerPrefs.SetInt("coins", coins);
    }
}
