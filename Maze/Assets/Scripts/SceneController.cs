using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private Camera CameraUp, CameraPlayer;
    [SerializeField] private GameObject CanvasGame, CanvasMain, PauseMenu, WinMenu, OptionsMenu, LoadingPanel, BuyTipPanel, ReplyPanel, NewRecord;
    [SerializeField] private Transform FirstStar, SecondStar;
    [SerializeField] private Animator PlayerAnim, TimerAnim, StarsAnim, NotBuyTipAnim, TimeSliderAnim;
    [SerializeField] private PlayerController Controller;
    [SerializeField] private List<GameObject> GameButtons;
    [SerializeField] private TMP_Text GameTimerText, LookTimerText, PauseTime, FinalTime, StartGameText;
    [SerializeField] private Image TimerImage;
    [SerializeField] private float LookingTime;
    [SerializeField] private int FirstStarTime, SecondStarTime, ThirdStarTime;
    [SerializeField] private AudioSource TickTackAudio, StartAudio, StopwatchAudio;
    [SerializeField] private AudioSource Click;
    [SerializeField] private Slider TimeSlider, LoadingSlider;
    [SerializeField] private Money Money;
    private float gameTimeLeft, timerLeft;
    private int countStars;


    void Start()
    {
        PauseMenu.SetActive(false);
        WinMenu.SetActive(false);
        OptionsMenu.SetActive(false);
        BuyTipPanel.SetActive(false);
        ReplyPanel.SetActive(false);
        LoadingPanel.SetActive(false);
        countStars = 3;

        
        CameraUp.enabled = true;
        CanvasMain.SetActive(CameraUp.enabled);
        CameraPlayer.enabled = false;
        CanvasGame.SetActive(CameraPlayer.enabled);
        Controller.enabled = false;
        PlayerAnim.enabled = false;

        Invoke("ChangeCamera", LookingTime);
        Invoke("ChangeMove", LookingTime);
        StartCoroutine(StartLookTimer());
        gameTimeLeft = 0f;
        timerLeft = LookingTime;


        FirstStar.localPosition = new Vector3(ThirdStarTime*100/FirstStarTime-50,0f,0f);
        SecondStar.localPosition = new Vector3(SecondStarTime*100/FirstStarTime-50,0f,0f);
    }
    void Update(){
        
        if (countStars == 3){
            if(gameTimeLeft > ThirdStarTime) {
                TimeSliderAnim.SetBool("TwoStars", true);
                countStars = 2;
            }}
           
        if (countStars == 2){
            if(gameTimeLeft > SecondStarTime) {
                TimeSliderAnim.SetBool("OneStar", true);
                countStars = 1;
            }}
           
        if (countStars == 1){
            if(gameTimeLeft > FirstStarTime) {
                TimeSliderAnim.SetBool("ZeroStars", true);
                countStars = 0;
            }}
        
        TimeSlider.value = gameTimeLeft/FirstStarTime;
    }
    IEnumerator StartGameTimer(){
        StopwatchAudio.Play();
        while (true)
        {    
            gameTimeLeft += Time.deltaTime;
            UpdateGameTimer();
            yield return null;
            }
    }
    
    IEnumerator StartLookTimer(){
        timerLeft = LookingTime;
        TimerAnim.SetTrigger("Play");
        TimerAnim.SetFloat("SpeedAnim", 1/LookingTime);
        TickTackAudio.Play();
        while (timerLeft > 0)
        {
            timerLeft -= Time.deltaTime;
            UpdateLookTimer();
            yield return null;
        }
        TickTackAudio.Stop();
        StartAudio.Play();
        StartCoroutine(StartGameTimer());
    }

    void ChangeMove(){
        Controller.TurnScript();
        PlayerAnim.enabled = !PlayerAnim.enabled;
        Controller.enabled = !Controller.enabled;
    }
    void UpdateGameTimer()
    {
        float minutes = Mathf.FloorToInt(gameTimeLeft / 60);
        float seconds = Mathf.FloorToInt(gameTimeLeft % 60);
        GameTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void UpdateLookTimer(){
        var normalizedValue = Mathf.Clamp(timerLeft / LookingTime, 0.0f, 1.0f);
        TimerImage.fillAmount = 1 - normalizedValue;
        int time = Mathf.FloorToInt(timerLeft % 60) + 1;
        LookTimerText.text = string.Format("{0:0}", time);
    }


    private void ChangeCamera(){
        CameraUp.enabled = !CameraUp.enabled;
        CanvasMain.SetActive(CameraUp.enabled);
        CameraPlayer.enabled = !CameraPlayer.enabled;
        CanvasGame.SetActive(CameraPlayer.enabled);
    }

    public void FinishGame(){
        StopwatchAudio.Stop(); 
        StopAllCoroutines();
        GameButtonsOff();
        ChangeMove();
        WinMenu.SetActive(true);
        int finishTime = Mathf.FloorToInt(gameTimeLeft);
        FinalTime.text = GameTimerText.text;
        if (!PlayerPrefs.HasKey(SceneManager.GetActiveScene().name + "time")){
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "stars", countStars);
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "time", finishTime);
            int coins = countStars*20;
            if (coins > 0) Money.MoneyPlus(coins);
            int allStars = PlayerPrefs.GetInt("AllStars");
            int addAllStars = allStars + countStars;
            PlayerPrefs.SetInt("AllStars", addAllStars);
            }
        else if(PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "time") > finishTime){
            NewRecord.SetActive(true);
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "time", finishTime);
            if (PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "stars") < countStars){
                int allStars = PlayerPrefs.GetInt("AllStars");
                int countNewStars = countStars - PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "stars");
                int addAllStars = allStars + countNewStars;
                int coins = countNewStars*10;
                if (coins > 0) Money.MoneyPlus(coins);
                PlayerPrefs.SetInt("AllStars", addAllStars);
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "stars", countStars);
            }
        }
        StarsAnim.SetInteger("CountStars", countStars);
        PlayerPrefs.Save();
    }

    IEnumerator LoadingScreen(string NameLevel){
        CanvasGame.SetActive(false);
        AsyncOperation operation = SceneManager.LoadSceneAsync(NameLevel);
        LoadingPanel.SetActive(true);
        while (!operation.isDone){
            float progress = Mathf.Clamp01(operation.progress / .9f);
            LoadingSlider.value = progress;
            yield return null;
        }
    }

    //Buttons
    public void ButtonStart()
    {
        Click.Play();
        TickTackAudio.Stop();
        StartAudio.Play();
        CancelInvoke();
        StopAllCoroutines();
        ChangeMove();
        ChangeCamera();
        StartCoroutine(StartGameTimer());
    }
    public void ButtonTip0()
    {
        ChangeMove();
        StopAllCoroutines();
        StopwatchAudio.Stop(); 
        Click.Play();
        BuyTipPanel.SetActive(true);
    }
    public void ButtonBuyTip(){
        Click.Play();
        if (Money.Buy(50)){
            BuyTipPanel.SetActive(false);
            StartGameText.text = "Вернуться в игру";
            ChangeCamera();
            StartCoroutine(StartLookTimer());
            Invoke("ChangeCamera", LookingTime);
            Invoke("ChangeMove", LookingTime);
        }
        else{
            NotBuyTipAnim.SetTrigger("NotBuy");
            ChangeMove();
        }
    }
    public void ButtonNotBuyTip(){
        Click.Play();
        BuyTipPanel.SetActive(false);
        StartCoroutine(StartGameTimer());
        ChangeMove();
    }

    public void ButtonReply(){
        ChangeMove();
        StopAllCoroutines();
        StopwatchAudio.Stop(); 
        Click.Play();
        ReplyPanel.SetActive(true);
    }
    public void ButtonReplyLevel(){
        Click.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        StartCoroutine(LoadingScreen(SceneManager.GetActiveScene().name));
    }
    public void ButtonNotReplyLevel(){
        Click.Play();
        ReplyPanel.SetActive(false);
        StartCoroutine(StartGameTimer());
        ChangeMove();
    }

    public void ButtonNextScene(){
        Click.Play();
        int AllStars = PlayerPrefs.GetInt("AllStars");
        if ((SceneManager.GetActiveScene().name == "Level8" && AllStars<20) || (SceneManager.GetActiveScene().name == "Level16" && AllStars<40) || (SceneManager.GetActiveScene().name == "Level24" && AllStars<60)){
            SceneManager.LoadScene("Menu");
            StartCoroutine(LoadingScreen("Menu"));
        }
        else{
            int numberScene = SceneManager.GetActiveScene().buildIndex+1;
            SceneManager.LoadScene(numberScene);
            StartCoroutine(LoadingScreen(SceneManager.GetSceneByBuildIndex(numberScene).name));
        }
    }
    public void ButtonLoadScene (string nameScene)
    {
        Click.Play();
        SceneManager.LoadScene(nameScene);
        StartCoroutine(LoadingScreen(nameScene));
    }
    public void ButtonPauseTrue(){
        Click.Play();
        StopwatchAudio.Stop(); 
        StopAllCoroutines();
        PauseMenu.SetActive(true);
        PauseTime.text = GameTimerText.text;
        GameButtonsOff();
        ChangeMove();
    }
    public void ButtonContinueGame(){
        Click.Play();
        PauseMenu.SetActive(false);
        GameButtonsOn();
        StartCoroutine(StartGameTimer());
        ChangeMove();
    }

    public void ButtonOptions(){
        Click.Play();
        PauseMenu.SetActive(false);
        OptionsMenu.SetActive(true);
    }
    
    public void ButtonOptionsBack(){
        Click.Play();
        PauseMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }

    public void ButtonQuit(){
        Click.Play();
        Application.Quit();
    }

    void GameButtonsOff(){
        Click.Play();
        foreach(GameObject button in GameButtons){
            button.SetActive(false);
        }
    }
    void GameButtonsOn(){
        Click.Play();
        foreach(GameObject button in GameButtons){
            button.SetActive(true);
        }
    }
}
