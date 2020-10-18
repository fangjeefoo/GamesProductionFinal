using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    //static member
    public static GameManager gm; 

    //public variables
    public GameObject shopCanvas;
    public GameObject upgradeCanvas;
    public GameObject pauseCanvas;
    public GameObject CancelCanvas;
    public GameObject[] enemyType;
    public GameObject[] towerType;
    public GameObject specialPickup;
    public Transform enemyParent;
    public Transform towerParent;
    public Button[] skillButton;
    public Button cogButton;
    public Button shopButton;
    public Button specialSkillButton;
    public Button damage;
    public Button range;
    public Button fire;
    public Button ice;
    public Text waveText;
    public Text countdownText;
    public Text moneyText;
    public Text notEnoughMoneyText;
    public Text notEnoughMoneyText_Upgrade;
    public Text upgradeTutorial;
    public Text bossWaveDisplay;
    public Text skillTutorial;
    public Text text_specialSkill;
    public Text[] towerDetail;
    public int money;
    public int moneyValue;
    [Tooltip("increase percentage of interest for power-up")]
    public int interest;
    [Tooltip("decrease percentage of firing speed for power-up")]
    public int fireRate;
    public float countdown;
    [HideInInspector]
    public float timer;
    [HideInInspector]
    public string state;
    [HideInInspector]
    public List<GameObject> towerList;
    [HideInInspector]
    public List<GameObject> enemyList;
    [HideInInspector]
    public GameObject[] nodeList;
    [HideInInspector]
    public List<GameObject> rocketList;

    //private variables
    private int currentWave;
    private int currentEnemy;
    private bool isFinished;
    private GameObject selectedNode;
    private int selectedEnemy;
    private List<string> pickUpSlot;  
    private string tower;
    private int currentMoneyValue;
    private float specialSkillCD;
    // Start is called before the first frame update

    void Start()
    {
        Time.timeScale = 1;
        gm = this.GetComponent<GameManager>();
        currentWave = 0;
        currentEnemy = 10;
        countdown = 25f;
        selectedEnemy = 2;
        timer = countdown;
        isFinished = true;
        state = "playing";
        currentMoneyValue = moneyValue;
        specialSkillCD = 0;

        enemyList = new List<GameObject>();
        towerList = new List<GameObject>();
        rocketList = new List<GameObject>();
        nodeList = GameObject.FindGameObjectsWithTag("Node");
        pickUpSlot = new List<string>();

        waveText.text = "Waves: " + currentWave.ToString();
        RefreshSkill();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentWave != 30)
            timer -= Time.deltaTime;
        if (specialSkillButton.interactable == false)
            specialSkillCD += Time.deltaTime;

        if(specialSkillCD >= 60f)
        {
            specialSkillCD = 0;
            specialSkillButton.interactable = true;
        }

        countdownText.text = "Countdown: " + Mathf.Round(timer).ToString();
        moneyText.text = "Money: " + money.ToString();

        if (currentWave == 30 && enemyList.Count == 0)
        {
            Win();
        }

        if (timer <= 0f && currentWave != 30)
        {
            currentWave++;
            waveText.text = "Waves: " + currentWave.ToString();
            timer = countdown;
            if (currentWave % 5 == 0)
            {
                currentEnemy = 20;
                selectedEnemy += 2;
                StartCoroutine(DisplayBossWave());
                if (selectedEnemy >= enemyType.Length)
                    selectedEnemy = enemyType.Length - 1;
            }
            else
                currentEnemy = 10;

            if ((currentWave - 1) % 5 == 0 && (currentWave - 1) != 0)
                Instantiate(specialPickup);

            isFinished = true;
        }     
        else
            isFinished = false;

        if (isFinished)
            StartCoroutine(NextWave());

        if (state == "lose")
            Invoke("Lose", 2.0f);
    }

    public void ShopButton()
    {
        shopCanvas.SetActive(true);
        notEnoughMoneyText.enabled = false;
        ButtonInteractable(false);
        NodeCollider(false);
    }

    public void CogButton()
    {
        Time.timeScale = 0;
        ButtonInteractable(false);
        NodeCollider(false);
        pauseCanvas.SetActive(true);
    }

    public void UpdateDetail()
    {
        if (selectedNode)
        {
            if (selectedNode.GetComponent<Node>().tower.tag == "Melee")
            {
                towerDetail[0].text = "Melee Tower";
                towerDetail[2].text = "Fire Rate: " + selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().currentFireRate;

                if (selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().fire)
                {
                    towerDetail[1].text = "Total Damage: " + (selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().damage +
                   (selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().damage * selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().fireDamage / 100));
                    towerDetail[4].text = "Fire Damage(%): " + selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().fireDamage;
                }
                else
                {
                    towerDetail[1].text = "Total Damage: " + selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().damage;
                    towerDetail[4].text = "Fire Damage(%): - ";
                }

                if (selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().ice)
                    towerDetail[3].text = "Ice Slow(%): " + selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().iceSlow;
                else
                    towerDetail[3].text = "Ice Slow(%): - ";

            }
            else if (selectedNode.GetComponent<Node>().tower.tag == "Range")
            {
                towerDetail[0].text = "Range Tower";
                towerDetail[2].text = "Fire Rate: " + selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().currentFireRate;

                if (selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().fire)
                {
                    towerDetail[1].text = "Total Damage: " + (selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().damage +
                    (selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().damage * selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().fireDamage / 100));
                    towerDetail[4].text = "Fire Damage(%): " + selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().fireDamage;
                }
                else
                {
                    towerDetail[1].text = "Total Damage: " + selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().damage;
                    towerDetail[4].text = "Fire Damage(%): - ";
                }

                if (selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().ice)
                    towerDetail[3].text = "Ice Slow(%): " + selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().iceSlow;
                else
                    towerDetail[3].text = "Ice Slow(%): - ";
            }
        }
    }

    public void UpgradeTower(GameObject node)
    {
        selectedNode = node;
        upgradeCanvas.SetActive(true);
        ButtonInteractable(false);
        NodeCollider(false);
        if (selectedNode)
        {
            UpdateDetail();
            if (selectedNode.GetComponent<Node>().tower.tag == "Melee")
            {              
                if (selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().damageUp)
                    damage.interactable = false;
                else
                    damage.interactable = true;

                if (selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().rangeUp)
                    range.interactable = false;
                else
                    range.interactable = true;

                if (selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().fire ||
                    selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().ice)
                {
                    fire.interactable = false;
                    ice.interactable = false;
                }
                else
                {
                    fire.interactable = true;
                    ice.interactable = true;
                }     
            }
            else if (selectedNode.GetComponent<Node>().tower.tag == "Range")
            {
                if (selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().damageUp)
                    damage.interactable = false;
                else
                    damage.interactable = true;

                if (selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().rangeUp)
                    range.interactable = false;
                else
                    range.interactable = true;

                if (selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().fire ||
                    selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().ice)
                {
                    fire.interactable = false;
                    ice.interactable = false;
                }
                else
                {
                    fire.interactable = true;
                    ice.interactable = true;
                }
            }
        }
    }

    public void CloseCanvas(string canvas)
    {
        switch (canvas)
        {
            case "shop":
                NodeCollider(true);
                Node.buyState = false;
                ButtonInteractable(true);
                shopCanvas.SetActive(false);
                CancelCanvas.SetActive(false);
                break;
            case "pause":
                Time.timeScale = 1;
                ButtonInteractable(true);
                NodeCollider(true);
                pauseCanvas.SetActive(false);
                break;
            case "upgrade":
                selectedNode = null;
                NodeCollider(true);
                upgradeCanvas.SetActive(false);
                ButtonInteractable(true);
                break;
            case "mainMenu":
                LoadScene();
                break;
        }
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Lose()
    {
        PlayerPrefs.SetInt("Wave", currentWave);
        PlayerPrefs.SetFloat("Duration", Time.timeSinceLevelLoad);
        SceneManager.LoadScene("Lose");
    }

    public void Win()
    {
        PlayerPrefs.SetInt("Wave", currentWave);
        PlayerPrefs.SetFloat("Duration", Time.timeSinceLevelLoad);
        SceneManager.LoadScene("Win");
    }

    public void CheckConstraint(string tower)
    {
        switch (tower)
        {
            case "range":
                if (money < RangeTower.price)
                {
                    StartCoroutine(DisplayNotEnoughMoney());
                    return;
                }
                break;
            case "melee":
                if (money < MeleeTower.price)
                {                    
                    StartCoroutine(DisplayNotEnoughMoney());
                    return;
                }
                break;
        }

        this.tower = tower;
        Node.buyState = true;
        shopCanvas.SetActive(false);

        foreach (GameObject node in nodeList)
        {
            node.GetComponent<MeshRenderer>().enabled = true;
            if (node.GetComponent<Node>() != null)
            {                
                node.GetComponent<Node>().CheckAvailability();
            }                
        }

        NodeCollider(true);
        CancelCanvas.SetActive(true);
    }

    public void CancelBuy()
    {
        Node.buyState = false;
        CancelCanvas.SetActive(false);
        ButtonInteractable(true);
        NodeCollider(true);
        ClearAvailability();
    }

    public void ClearAvailability()
    {
        foreach (GameObject node in nodeList)
        {
            node.GetComponent<MeshRenderer>().enabled = false;
            if (node.GetComponent<Node>() != null)
                node.GetComponent<Node>().Cancel();
        }
        NodeCollider(true);
    }

    public void ButtonInteractable(bool enabled)
    {
        cogButton.interactable = enabled;
        shopButton.interactable = enabled;
        specialSkillButton.interactable = enabled;
        skillButton[0].interactable = enabled;
        skillButton[1].interactable = enabled;
    }

    public GameObject BuyTower(GameObject node)
    {
        Vector3 pos = node.transform.position;
        pos.y += 0.5f;

        switch (tower)
        {
            case "range":
                towerList.Add(Instantiate(towerType[0], pos, node.transform.rotation, towerParent));
                money -= RangeTower.price;
                break;
            case "melee":
                towerList.Add(Instantiate(towerType[1], pos, node.transform.rotation, towerParent));
                money -= MeleeTower.price;
                break;
        }

        CancelBuy();
        
        return towerList[towerList.Count - 1];
    }

    public void CollectMoney()
    {
        money += currentMoneyValue;
    }
   
    public void UpgradeDamage()
    {
        if (money >= MeleeTower.upgradePrice)
        {
            if (selectedNode && selectedNode.GetComponent<Node>().tower.tag == "Melee")
            {
                selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().damage += selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().upgradeDamage;
                selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().damageUp = true;
            }                
            else if (selectedNode && selectedNode.GetComponent<Node>().tower.tag == "Range")
            {
                selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().damage += selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().upgradeDamage;
                selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().damageUp = true;
            }              
            money -= MeleeTower.upgradePrice;
            damage.interactable = false;
            UpdateDetail();
        }
        else
            StartCoroutine(DisplayNotEnoughMoney_Upgrade());
    }

    public void UpgradeRange()
    {
        if(money >= MeleeTower.upgradePrice)
        {
            if (selectedNode && selectedNode.GetComponent<Node>().tower.tag == "Melee")
            {
                selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().radius += selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().upgradeRange;
                selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().rangeUp = true;
            }                
            else if (selectedNode && selectedNode.GetComponent<Node>().tower.tag == "Range")
            {
                selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().radius += selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().upgradeRange;
                selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().rangeUp = true;
            }               
            money -= MeleeTower.upgradePrice;
            range.interactable = false;
            selectedNode.GetComponent<Node>().SetupRange();
            UpdateDetail();
        }
        else
            StartCoroutine(DisplayNotEnoughMoney_Upgrade());
    }

    public void UpgradeFireSkill()
    {
        if (money >= MeleeTower.skillPrice)
        {
            if (selectedNode && selectedNode.GetComponent<Node>().tower.tag == "Melee")
                selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().fire = true;
            else if (selectedNode && selectedNode.GetComponent<Node>().tower.tag == "Range")
                selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().fire = true;

            money -= MeleeTower.upgradePrice;
            ice.interactable = false;
            fire.interactable = false;
            UpdateDetail();
        }
        else
            StartCoroutine(DisplayNotEnoughMoney_Upgrade());
    }

    public void UpgradeIceSkill()
    {
        if (money >= MeleeTower.skillPrice)
        {
            if (selectedNode && selectedNode.GetComponent<Node>().tower.tag == "Melee")
                selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().ice = true;
            else if (selectedNode && selectedNode.GetComponent<Node>().tower.tag == "Range")
                selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().ice = true;

            money -= MeleeTower.upgradePrice;
            ice.interactable = false;
            fire.interactable = false;
            UpdateDetail();
        }
        else
            StartCoroutine(DisplayNotEnoughMoney_Upgrade());
    }

    public void Sell()
    {
        int refund = 0;

        if (selectedNode && selectedNode.GetComponent<Node>().tower.tag == "Melee")
        {
            if(selectedNode && selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().damageUp 
                && selectedNode && selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().rangeUp)
            {
                refund += MeleeTower.upgradePrice + MeleeTower.upgradePrice;
            }
            else if (selectedNode && selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().damageUp
                || selectedNode && selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().rangeUp)
            {
                refund += MeleeTower.upgradePrice;
            }

            if (selectedNode && selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().fire
                || selectedNode && selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().ice)
            {
                refund += MeleeTower.skillPrice;
            }

            refund += MeleeTower.price;
        }
        else if(selectedNode && selectedNode.GetComponent<Node>().tower.tag == "Range")
        {
            if (selectedNode && selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().damageUp
                && selectedNode && selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().rangeUp)
            {
                refund += RangeTower.upgradePrice + RangeTower.upgradePrice;
            }
            else if (selectedNode && selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().damageUp
                    || selectedNode && selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().rangeUp)
            {
                refund += RangeTower.upgradePrice;
            }

            if (selectedNode && selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().fire
                || selectedNode && selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().ice)
            {
                refund += RangeTower.skillPrice;
            }
            refund += RangeTower.price;
        }

        money += refund * 50 / 100;

        for(int i = 0; i < towerList.Count; i++)
        {
            if(GameObject.ReferenceEquals(selectedNode.GetComponent<Node>().tower, towerList[i]))
            {
                Destroy(towerList[i]);
                towerList.Remove(towerList[i]);
                selectedNode.GetComponent<Node>().available = true;
                selectedNode.GetComponent<Node>().tower = null;                
                break;
            }
        }
        selectedNode.GetComponent<Node>().DisableRange();
        CloseCanvas("upgrade");
    }

    public void Interest()
    {
        if(pickUpSlot.Count < skillButton.Length)
        {
            pickUpSlot.Add("Interest");
        }
    }

    public void SpeedUp()
    {
        if (pickUpSlot.Count < skillButton.Length)
        {
            pickUpSlot.Add("Speed Up");
        }
    }

    public void RefreshSkill()
    {
        if (pickUpSlot.Count == 0)
        {
            foreach (Button button in skillButton)
            {
                button.GetComponentInChildren<Text>().text = "-";
            }
        }
        else
        {
            for (int i = 0; i < pickUpSlot.Count; i++)
            {
                skillButton[i].GetComponentInChildren<Text>().text = pickUpSlot[i];
            }

            int notAssign = skillButton.Length - pickUpSlot.Count;
            if (notAssign > 0)
            {
                for (int i = notAssign; i < skillButton.Length; i++)
                {
                    skillButton[i].GetComponentInChildren<Text>().text = "-";
                }
            }
        }
    }

    public void UseItem()
    {
        if (EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text == "Speed Up")
            StartCoroutine(UseSpeedUp());
        else if (EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text == "Interest")
            StartCoroutine(UseInterest());

        pickUpSlot.Remove(EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text);
        RefreshSkill();
    }

    public void LongClick()
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text;

        if (buttonName == "Speed Up")
        {
            skillTutorial.enabled = true;
            skillTutorial.text = "Increase the fire rate of each tower by \n" + fireRate + "% for 10 seconds";
        }            
        else if(buttonName == "Interest")
        {
            skillTutorial.enabled = true;
            skillTutorial.text = "Increase the money value by \n" + interest + "% for 1 wave";
        }     
        else if(buttonName == "Damage + (200)")
        {
            upgradeTutorial.enabled = true;
            int damage = 0;

            if (selectedNode.GetComponent<Node>().tower.tag == "Melee")
                damage = selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().upgradeDamage;                
            else if (selectedNode.GetComponent<Node>().tower.tag == "Range")
                damage = selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().upgradeDamage;
            upgradeTutorial.text = "Increase the damage of \n the tower by " + damage;
        }
        else if (buttonName == "Range + (200)")
        {
            upgradeTutorial.enabled = true;
            float range = 0f;

            if (selectedNode.GetComponent<Node>().tower.tag == "Melee")
                range = selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().upgradeRange;
            else if (selectedNode.GetComponent<Node>().tower.tag == "Range")
                range = selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().upgradeRange;
            upgradeTutorial.text = "Increase the range of \n the tower by " + range;
        }
        else if (buttonName == "Fire Skill (200)")
        {
            upgradeTutorial.enabled = true;
            int fire = 0;

            if (selectedNode.GetComponent<Node>().tower.tag == "Melee")
                fire = selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().fireDamage;
            else if (selectedNode.GetComponent<Node>().tower.tag == "Range")
                fire = selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().fireDamage;
            upgradeTutorial.text = "Increase the damage of \n the tower by " + fire + "%";
        }
        else if (buttonName == "Ice Skill (200)")
        {
            upgradeTutorial.enabled = true;
            int slow = 0;

            if (selectedNode.GetComponent<Node>().tower.tag == "Melee")
                slow = selectedNode.GetComponent<Node>().tower.GetComponent<MeleeTower>().iceSlow;
            else if (selectedNode.GetComponent<Node>().tower.tag == "Range")
                slow = selectedNode.GetComponent<Node>().tower.GetComponent<RangeTower>().iceSlow;
            upgradeTutorial.text = "Decrease the speed of \n the enemy by " + slow + "%";
        }
        else if (buttonName == "Sell")
        {
            upgradeTutorial.enabled = true;
            upgradeTutorial.text = "Sell the tower for half price \n(include upgrade)";
        }
        else if(buttonName == "Boom (500)")
        {
            skillTutorial.enabled = true;
            skillTutorial.text = "Kill the first 10 enemies instantly (CD 1 minute)";
        }
    }

    public void DisableLongClick()
    {
        skillTutorial.enabled = false;
        upgradeTutorial.enabled = false;
    }

    public void OnClick()
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text;

        if (buttonName == "Speed Up" || buttonName == "Interest")
            UseItem();
        else if (buttonName == "Damage + (200)")
            UpgradeDamage();
        else if (buttonName == "Range + (200)")
            UpgradeRange();
        else if (buttonName == "Fire Skill (200)")
            UpgradeFireSkill();
        else if (buttonName == "Ice Skill (200)")
            UpgradeIceSkill();
        else if (buttonName == "Sell")
            Sell();
        else if (buttonName == "Boom (500)")
            SpecialSkill();
    }

    public void SpecialSkill()
    {
        if (money >= 500 && enemyList.Count > 0)
        {
            GameObject temp;
            if (enemyList.Count >= 10)
            {
                for (int i = 0; i < 10; i++)
                {
                    temp = enemyList[0];
                    enemyList.Remove(enemyList[0]);
                    temp.GetComponent<Enemy>().DestroyObject();
                }
                money -= 500;
            }
            else
            {
                int counter = enemyList.Count;
                for (int i = 0; i < counter; i++)
                {
                    temp = enemyList[0];
                    enemyList.Remove(enemyList[0]);
                    temp.GetComponent<Enemy>().DestroyObject();
                }
                money -= 500;
            }
            specialSkillButton.interactable = false;
        }
        else
            StartCoroutine(DisplayText_SpecialSkill());
    }
        

    public void NodeCollider(bool enable)
    {
        foreach (GameObject node in nodeList)
        {
            if (node.GetComponent<BoxCollider>() != null)
                node.GetComponent<BoxCollider>().enabled = enable;
        }
    }

    private void GenerateEnemy(int door)
    {
        int type = Random.Range(0, selectedEnemy);
        Vector3 position = new Vector3();
        Quaternion rotation;

        if(door == 1)
        {
            if (enemyType[type].name.Contains("AlienDestroyer"))
                position = new Vector3(-12.5f, 0.3f,0f);
            else if (enemyType[type].name.Contains("AlienFighter"))
                position = new Vector3(-12.5f, 0.6f, 0f);
            else if (enemyType[type].name.Contains("BioTorpedo"))
                position = new Vector3(-12.5f, 0.3f, 0f);
            else if (enemyType[type].name.Contains("Polygonal Metalon"))
                position = new Vector3(-12.5f, 0f, 0f);
            rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            if (enemyType[type].name.Contains("AlienDestroyer"))
                position = new Vector3(-13.5f, 0.3f, -10f);
            else if (enemyType[type].name.Contains("AlienFighter"))
                position = new Vector3(-13.5f, 0.6f, -10f);
            else if (enemyType[type].name.Contains("BioTorpedo"))
                position = new Vector3(-13.5f, 0.3f, -10f);
            else if (enemyType[type].name.Contains("Polygonal Metalon"))
                position = new Vector3(-13.5f, 0f, -10f);
            rotation = Quaternion.Euler(0, 90, 0);
        }

        if (enemyType[type])
        {
            var temp = Instantiate(enemyType[type], position, rotation, enemyParent);
            temp.GetComponent<Enemy>().door = door;
            enemyList.Add(temp);
        }            
    }

    IEnumerator NextWave()
    {
        int door = Random.Range(1, 3);
        for (int i = 0; i < currentEnemy; i++)
        {
            GenerateEnemy(door);
            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator DisplayText_SpecialSkill()
    {
        if (money < 500)
            text_specialSkill.text = "Not enough money";
        else if (enemyList.Count == 0)
            text_specialSkill.text = "No enemy exist in the game";

        text_specialSkill.enabled = true;
        yield return new WaitForSeconds(0.5f);
        text_specialSkill.enabled = false;
    }

    IEnumerator DisplayNotEnoughMoney()
    {
        notEnoughMoneyText.enabled = true;
        yield return new WaitForSeconds(0.5f);
        notEnoughMoneyText.enabled = false;
    }

    IEnumerator DisplayNotEnoughMoney_Upgrade()
    {
        notEnoughMoneyText_Upgrade.enabled = true;
        yield return new WaitForSeconds(0.5f);
        notEnoughMoneyText_Upgrade.enabled = false;
    }

    IEnumerator DisplayBossWave()
    {
        bossWaveDisplay.enabled = true;
        yield return new WaitForSeconds(2.0f);
        bossWaveDisplay.enabled = false;
    }

    IEnumerator UseInterest()
    {        
        currentMoneyValue = moneyValue + (moneyValue * interest / 100);
        yield return new WaitForSeconds(countdown);  
        currentMoneyValue = moneyValue;
    }

    IEnumerator UseSpeedUp()
    {
        for(int i = 0; i < towerList.Count; i++)
        {
            if(towerList[i].tag == "Melee")
            {
                towerList[i].GetComponent<MeleeTower>().currentFireRate = towerList[i].GetComponent<MeleeTower>().fireRate - 
                    (towerList[i].GetComponent<MeleeTower>().fireRate * fireRate / 100);
            }
            else if(towerList[i].tag == "Range")
            {
                towerList[i].GetComponent<RangeTower>().currentFireRate = towerList[i].GetComponent<RangeTower>().fireRate -
                    (towerList[i].GetComponent<RangeTower>().fireRate * fireRate / 100);
            }
        }
        yield return new WaitForSeconds(10.0f);
        for (int i = 0; i < towerList.Count; i++)
        {
            if (towerList[i].tag == "Melee")
            {
                towerList[i].GetComponent<MeleeTower>().currentFireRate = towerList[i].GetComponent<MeleeTower>().fireRate;
            }
            else if (towerList[i].tag == "Range")
            {
                towerList[i].GetComponent<RangeTower>().currentFireRate = towerList[i].GetComponent<RangeTower>().fireRate;
            }
        }
    }
}
