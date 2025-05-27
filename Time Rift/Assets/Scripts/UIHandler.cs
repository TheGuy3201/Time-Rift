using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
public class UIHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField playerName = null;
    [SerializeField] TMP_Dropdown programs = null;
    [SerializeField] TMP_Sprite image = null;
    [SerializeField] Slider slider = null;
    [SerializeField] TextMeshProUGUI gpaLabel = null;
    [SerializeField] TextMeshProUGUI usernameLabel = null;
    [SerializeField] TextMeshProUGUI programLabel = null;
    void Start()
    {
        playerName.text = "Joshua Desroches";
        programs.options.Clear();
        slider.value = 0.5f;
        List<string> progs = new List<string>{
            "Artificial Intelligence",
            "Game - Programming",
            "Health Informatics",
            "Software Engineering Technologist",
            "Software Engineering Technician"};
        foreach (string prog in progs)
            programs.options.Add(new TMP_Dropdown.OptionData() { text = prog });
    }

    public void OnSliderChangedHandler()
    {
        gpaLabel.text = $"GPA ({slider.value:F1})";
    }

    public void OnButtonExitHandler()
    {
        //clear all the fields
        print("exit handler");
        image.sprite = null;
        programs.options.Clear();
        playerName.text = string.Empty;
        slider.value = slider.minValue;
    }

    public void OnButtonOkHandler()
    {
        string playerProgram = programs.value > -1 ?
        programs.options[programs.value].text : "";
        usernameLabel.text = $"Name: {playerName.text}";
        programLabel.text = $"Program: {playerProgram}";
        gpaLabel.text = $"GPA: {slider.value:F1}";
    }
}