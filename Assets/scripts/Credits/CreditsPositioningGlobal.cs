using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class CreditsPositioningGlobal : Singleton<CreditsPositioningGlobal> {
    [Header("Only used renew with a screen size of 1920x1080")]
    [SerializeField] private bool renew = false;
    [SerializeField] private GameObject creditsTextPrefab;
    [SerializeField] private float initialOffset = 20;

    [SerializeField] private float titleFontSize = 110;
    [SerializeField] private float titleSeparatorDistance = 60;

    [SerializeField] private float categoryFontSize = 75;
    [SerializeField] private float categorySeparatorDistance = 25;

    [SerializeField] private float contributorFontSize = 50;
    [SerializeField] private float contributorSeparatorDistance = 10;

    [SerializeField] private List<CreditCategory> creditsCategories;

    [HideInInspector] private Vector3 textPosition;

    public override IEnumerator Initialize() {
        yield return null;
    }
    
    void Update() {
        if(!ScriptInitializationGlobal.Instance.ShouldUpdate) { return; }
        if(renew) {
            RenewCredits();
            renew = false;
        }
    }

    private void RenewCredits() {
        textPosition = new Vector3(Screen.width/2, Screen.height-initialOffset);
        DestroyAllText();
        CreateTitle("Credits");
        AddSeparator(titleSeparatorDistance);
        for(int i=0; i<creditsCategories.Count; i++) {
            CreateCategory(creditsCategories[i].GetCategoryName(), i+1);
            List<string> contributors = creditsCategories[i].GetConstributors();
            for(int j=0; j<contributors.Count; j++) {
                CreateContributor(contributors[j], j+1);
            }
            AddSeparator(titleSeparatorDistance);
        }
    }

    private void DestroyAllText() {
        TextMeshProUGUI[] textMeshProUGUIs = Utilities.TryGetComponentsInChildren<TextMeshProUGUI>(gameObject);
        foreach(TextMeshProUGUI textMeshProUGUI in textMeshProUGUIs) {
            DestroyImmediate(textMeshProUGUI.gameObject);
        }
    }

    private void CreateTitle(string text) {
        GameObject title = Instantiate(creditsTextPrefab, transform);
        title.name = text+"Title";
        TextMeshProUGUI textMeshProUGUI = Utilities.TryGetComponent<TextMeshProUGUI>(title);
        textMeshProUGUI.fontSize = titleFontSize;
        textMeshProUGUI.text = text;
        title.transform.position = textPosition;
        //Debug.Log($"{text} textPosition: {textPosition}");
        AddSeparator(titleSeparatorDistance);
    }
    private void CreateCategory(string text, int index) {
        GameObject category = Instantiate(creditsTextPrefab, transform);
        category.name = "Category"+index;
        TextMeshProUGUI textMeshProUGUI = Utilities.TryGetComponent<TextMeshProUGUI>(category);
        textMeshProUGUI.fontSize = categoryFontSize;
        textMeshProUGUI.text = text;
        category.transform.position = textPosition;
        //Debug.Log($"{text} textPosition: {textPosition}");

        AddSeparator(categorySeparatorDistance);
    }
    private void CreateContributor(string text, int index) {
        GameObject contributor = Instantiate(creditsTextPrefab, transform);
        contributor.name = "Contributor"+index;
        TextMeshProUGUI textMeshProUGUI = Utilities.TryGetComponent<TextMeshProUGUI>(contributor);
        textMeshProUGUI.fontSize = contributorFontSize;
        textMeshProUGUI.text = text;
        contributor.transform.position = textPosition;
        //Debug.Log($"{text} textPosition: {textPosition}");

        AddSeparator(contributorSeparatorDistance);
    }

    private void AddSeparator(float distance) {
        textPosition = new Vector3(textPosition.x, textPosition.y-distance);
    }
}

[Serializable]
public class CreditCategory {
    [SerializeField] private string categoryName;
    [SerializeField] private List<string> contributors;

    public CreditCategory() {
        categoryName = "";
        contributors = new List<string>();
    }
    public CreditCategory(string categoryName) {
        this.categoryName = categoryName;
        contributors = new List<string>();
    }
    public CreditCategory(string categoryName, List<string> contributors) {
        this.categoryName = categoryName;
        this.contributors = new List<string>();
        this.contributors = contributors;
    }

    public string GetCategoryName() {
        return categoryName;
    }

    public List<string> GetConstributors() {
        return contributors;
    }
}