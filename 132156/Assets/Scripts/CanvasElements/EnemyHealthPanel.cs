using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthPanel : MonoBehaviour
{
    public Slider HealthPanelSlider;
    public Text HealthPanelText;

    private void Start()
    {
        //attach enemyhealthpanel to every enemy object
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemyObjects.Length == 0)
        {
            Debug.LogWarning("There are no enemies to attach the 'EnemyHealthPanel' on!");
        }
        else
        {
            foreach (GameObject enemy in enemyObjects)
            {
                enemy.GetComponent<EnemyController>().SetEnemyHealthPanel(this);
            }
        }
        
        ShowEnemyHealthPanel(false);
    }

    public void UpdateHealthPanel(float percentageValue)
    {
        HealthPanelSlider.value = percentageValue;
    }

    public void SetEnemyName(string name)
    {
        HealthPanelText.text = name;
    }

    public void ShowEnemyHealthPanel(bool value)
    {
        gameObject.SetActive(value);
    }

    public void ClearHealthPanel()
    {
        HealthPanelSlider.value = 0.0f;
        HealthPanelText.text = "";
    }
}
