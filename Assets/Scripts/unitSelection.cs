using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class unitSelection : MonoBehaviour
{
    public Text option1, option2, speed1, speed2, health1, health2, unitNo;
    int unitNum = 1;
    [SerializeField] Toggle choice1, choice2;
    public List<ScriptableUnit> Heroes;
    public List<BaseHero> DisplayedHeroes;
    [SerializeField] selectedUnits unitSelect;

    private void Awake()
    {
        Heroes = Resources.LoadAll<ScriptableUnit>("Units/Heroes").ToList();
    }

    // Start is called before the first frame update
    void Start()
    {

        RandomizeSelection();


    }

    // Update is called once per frame
    void Update()
    {
        unitNo.text = "Selecting Unit " + unitNum;
        option1.text = DisplayedHeroes[0].UnitName;
        health1.text = "Health: "+DisplayedHeroes[0].GetMaxHP().ToString();
        speed1.text = "Speed: " + DisplayedHeroes[0].GetSpeed().ToString();
        option2.text = DisplayedHeroes[1].UnitName;
        health2.text = "Health: " + DisplayedHeroes[1].GetMaxHP().ToString();
        speed2.text = "Speed: " + DisplayedHeroes[1].GetSpeed().ToString();

    }

    public void confirmButton()
    {
        if (unitNum <= 3)
        {
            if (choice1.isOn == true)
            {
                unitSelect.SpawnMe.Add(DisplayedHeroes[0]);
                Debug.Log("Adding choice 1");

            }
            else
            {
                unitSelect.SpawnMe.Add(DisplayedHeroes[1]);
                Debug.Log("Adding choice 1");
            }

            // Ensure that the unit isn't destroyed when the next scene loads
            DontDestroyOnLoad(unitSelect.SpawnMe[unitNum - 1]);

            RandomizeSelection();



        }

        //option1 = RandomUnit();
        //option2 = RandomUnit2();

        if (unitNum == 3)
        {
            //RandomizeSelection();

            unitNum = 1;

            SceneManager.LoadScene(1);
        }
        else
        {
            unitNum += 1;
        }
       
    }

    private T RandomUnit<T>(UnitType unitType) where T : BaseUnit
    {
        return (T) Heroes.Where(u => u.unitType == unitType).OrderBy(o => Random.value).First().UnitPrefab;
    }

    private void RandomizeSelection()
    {
        var randomHero1 = RandomUnit<BaseHero>(UnitType.Hero);
        var hero1 = Instantiate(randomHero1);
        var randomHero2 = RandomUnit<BaseHero>(UnitType.Hero);
        var hero2 = Instantiate(randomHero2);
        
        // Ensure that both heroes are different
        while (hero1.UnitName == hero2.UnitName)
        {
            randomHero2 = RandomUnit<BaseHero>(UnitType.Hero);
            hero2 = Instantiate(randomHero2);
        }

        DisplayedHeroes.Clear();
        DisplayedHeroes.Add(hero1);
        DisplayedHeroes.Add(hero2);
    }


}
