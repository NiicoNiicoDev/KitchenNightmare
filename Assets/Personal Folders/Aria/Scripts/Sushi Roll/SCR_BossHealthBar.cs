using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SCR_BossHealthBar : MonoBehaviour
{
    [SerializeField] int healthPerBar = 50;
    [SerializeField] GameObject defaultHealthSliderObject;
    [SerializeField] Color[] healthBarColours;
    [SerializeField] TextMeshProUGUI healthBarCount;

    GameObject bossObject;
    SCR_EnemyStats bossHealth;
    SCR_AI_SushiRoll bossController;
    Slider slider;
    Slider currentSlider;
    List<Slider> sliderList = new List<Slider>();

    int numOfHealthBars;
    bool bReady = false;

    [Header("Directioal Arrow")]
    [SerializeField] private Image directionalArrow;
    private Transform player;
    [SerializeField] private float distanceFromCentre = 200.0f;
    [SerializeField] private float hiddenDistanceProximity = 10.0f;
    private Vector3 bossPosition;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        bossObject = GameObject.FindGameObjectWithTag("Boss");
        bossHealth = bossObject.GetComponent<SCR_EnemyStats>();
        bossController = bossObject.GetComponent<SCR_AI_SushiRoll>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bossPosition = bossObject.transform.position;
        cam = Camera.main;

        ResetHealthBar();
    }

    public void ResetHealthBar()
    {
        if (bossHealth.CurrentHealth % healthPerBar > 0)
        {
            numOfHealthBars = bossHealth.CurrentHealth / healthPerBar;
            numOfHealthBars += 1;
        }
        else
        {

            numOfHealthBars = bossHealth.CurrentHealth / healthPerBar;
        }

        healthBarCount.text = numOfHealthBars.ToString();
        healthBarCount.enabled = false;

        /*Debug.Log(numOfHealthBars);
        Debug.Log("Remainder: " + bossHealth.CurrentHealth % healthPerBar);*/
        //numOfHealthBars = 1;

        //Instantiate a new health bar with each phase
        //Or Instantiate each health bar on start
        for (int i = 0; i < sliderList.Count; i++)
        {
            if (sliderList[i] != null)
            {
                Destroy(sliderList[i].gameObject);
            }
        }
        sliderList.Clear();

        for (int i = 0; i < numOfHealthBars; i++)
        {
            slider = Instantiate(defaultHealthSliderObject, this.gameObject.transform).GetComponent<Slider>();
            Image sliderImage = slider.gameObject.GetComponentsInChildren<Image>()[1];

            sliderImage.color = healthBarColours[i];
            slider.value = 50;
            //slider.gameObject.SetActive(true);
            sliderList.Add(slider);
        }

        currentSlider = sliderList[numOfHealthBars - 1];
        if (bossHealth.CurrentHealth % healthPerBar > 0)
        {
            currentSlider.maxValue = bossHealth.CurrentHealth % healthPerBar;
            currentSlider.value = currentSlider.maxValue;
        }
        numOfHealthBars--;
        bReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!bReady) return;
        //Debug.Log("Boss Ready: " + bossController.bIsReady);
        if (bossController.bIsReady)
        {
            UpdateDirectionalArrow();
        }

        if(bossController.bIsAwake)
        {
            healthBarCount.enabled = true;
            currentSlider.gameObject.SetActive(true);
        }
        else
        {
            healthBarCount.enabled = false;
            currentSlider.gameObject.SetActive(false);
        }

        if (currentSlider.value <= 0 && numOfHealthBars != 0) //Current health bar is 0 but it still needs to spawn more
        {
            Destroy(currentSlider.gameObject);
            currentSlider = sliderList[numOfHealthBars - 1];
            healthBarCount.text = numOfHealthBars.ToString();
            numOfHealthBars--;
        }
        else if (currentSlider.value <= 0 && numOfHealthBars == 0) //Current health bar is 0 but the boss is out of HP
        {
            Destroy(currentSlider.gameObject);
            healthBarCount.enabled = false;
            this.enabled = false;
        }

        currentSlider.value = bossHealth.CurrentHealth - (healthPerBar * numOfHealthBars);
        //Debug.Log(currentSlider.value);
    }

    private void UpdateDirectionalArrow()
    {
        if (!directionalArrow.gameObject.activeSelf) { directionalArrow.gameObject.SetActive(true); }
        if (Vector3.Distance(player.position, bossPosition) < hiddenDistanceProximity) 
        {
            //GameManager.gameManager.PlayBossMainLoopMusic();
            directionalArrow.gameObject.SetActive(false); 
            return; 
        }

        Vector3 playerPosition = player.position;
        Vector3 direction = bossPosition - playerPosition;
        direction.Normalize();

        Vector3 playerOffset = playerPosition + direction;

        Vector3 playerScreenPoint = cam.WorldToScreenPoint(playerPosition);
        Vector3 playerOffsetScreenPoint = cam.WorldToScreenPoint(playerOffset);

        Vector3 arrowDirection = playerOffsetScreenPoint - playerScreenPoint;
        arrowDirection.Normalize();


        Vector3 arrowPosition = arrowDirection * distanceFromCentre;
        directionalArrow.rectTransform.localPosition = arrowPosition;
        directionalArrow.rectTransform.up = arrowDirection;
    }
}
