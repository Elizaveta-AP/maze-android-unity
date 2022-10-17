using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private List<Button> ButtonsList;
    [SerializeField] private GameObject Menu, LevelsMenu, Options, LevelsPanelOne, LevelsPanel, LoadingPanel, SecondSeason, ThirdSeason, FourthSeason;
    [SerializeField] private List<GameObject> Stars;
    [SerializeField] private TMP_Text LevelOne, Level, BestTime, CountStars;
    [SerializeField] private AudioSource Click;
    [SerializeField] private Slider LoadingSlider;
    private string NameLevel;
    void Awake(){
        if (!PlayerPrefs.HasKey("Coins")) {ResetSaves();}
    }
    void Start()
    {
        int AllStars = PlayerPrefs.GetInt("AllStars");
        CountStars.text = AllStars.ToString();
        if (AllStars>=20){
            SecondSeason.SetActive(false);
            if (AllStars>=40){
                ThirdSeason.SetActive(false);
                if (AllStars>=60){
                    FourthSeason.SetActive(false);}}
        }
        foreach(Button levelButton in ButtonsList){
            int indexButton = ButtonsList.IndexOf(levelButton);
            int numberScene = indexButton + 1;
            string nameScene = $"Level{numberScene}";
            if ((numberScene == 32) || (numberScene == 24 && AllStars<60) || (numberScene == 16 && AllStars<40) || (numberScene == 8 && AllStars<20)) break;
            else if (PlayerPrefs.HasKey(nameScene + "time")){
                ButtonsList[indexButton+1].interactable = true;
                }
            else{
                break;
            }
        }
        Menu.SetActive(true);
        LevelsMenu.SetActive(false);
        Options.SetActive(false);
        LevelsPanelOne.SetActive(false);
        LevelsPanel.SetActive(false);
        LoadingPanel.SetActive(false);
    }

    public void ResetSaves(){
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("AllStars", 0);
        PlayerPrefs.SetInt("Coins", 300);
        PlayerPrefs.SetFloat("VolumeMusic", 0.5f);
        PlayerPrefs.SetFloat("VolumeSounds", 0.5f);
    }

    IEnumerator LoadingScreen(){
        AsyncOperation operation = SceneManager.LoadSceneAsync(NameLevel);
        LoadingPanel.SetActive(true);
        while (!operation.isDone){
            float progress = Mathf.Clamp01(operation.progress / .9f);
            LoadingSlider.value = progress;
            yield return null;
        }
    }

    public void ButtonSettings(){
        Click.Play();
        Menu.SetActive(false);
        Options.SetActive(true);
    }
    public void BUttonSettingsBack(){
        Click.Play();
        Options.SetActive(false);
        Menu.SetActive(true);
    }
    public void ButtonGame(){
        Click.Play();
        Menu.SetActive(false);
        LevelsMenu.SetActive(true);
    }
    public void ButtonLevel(int numberLevel){
        Click.Play();
        NameLevel = "Level" + numberLevel;
        if (PlayerPrefs.HasKey(NameLevel + "time")){
            LevelsPanel.SetActive(true);
            Level.text = "Уровень " + numberLevel;
            float best_time = PlayerPrefs.GetInt(NameLevel + "time");
            int minutes = Mathf.FloorToInt(best_time / 60);
            int seconds = Mathf.FloorToInt(best_time % 60);
            Debug.Log($"{NameLevel}, {best_time}, {minutes}, {seconds}");
            BestTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            int stars = PlayerPrefs.GetInt(NameLevel + "stars");
            foreach (GameObject star in Stars){
                if(Stars.IndexOf(star) < stars) star.SetActive(true);
                else star.SetActive(false);
            }
        }
        else{
            LevelsPanelOne.SetActive(true);
            LevelOne.text = "Уровень " + numberLevel;
        }
    }

    public void ButtonStartLevel(){
        Click.Play();
        SceneManager.LoadScene(NameLevel);
        StartCoroutine(LoadingScreen());
    }
    public void ButtonLevelsPanelOneBack(){
        Click.Play();
        LevelsPanelOne.SetActive(false);
    }
    public void ButtonLevelsPanelBack(){
        Click.Play();
        LevelsPanel.SetActive(false);
    }
    public void ButtonLevelsMenuBack(){
        Click.Play();
        LevelsMenu.SetActive(false);
        Menu.SetActive(true);
    }
    public void ButtonQuit(){
        Click.Play();
        Application.Quit();
    }
}
