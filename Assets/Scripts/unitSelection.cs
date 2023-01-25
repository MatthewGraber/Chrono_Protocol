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
    public Toggle choice1, choice2;
    public List<ScriptableUnit> Heroes;
    public List<BaseHero> DisplayedHeroes;
    selectedUnits unitSelect;

    private void Awake()
    {
        Heroes = Resources.LoadAll<ScriptableUnit>("Units/Heroes").ToList();
    }

    // Start is called before the first frame update
    void Start()
    {

        var randomHero1 = RandomUnit<BaseHero>(UnitType.Hero);
        var hero1 = Instantiate(randomHero1);
        DisplayedHeroes.Add(hero1);
        var randomHero2 = RandomUnit<BaseHero>(UnitType.Hero);
        var hero2 = Instantiate(randomHero2);
        DisplayedHeroes.Add(hero2);
        
    }

    // Update is called once per frame
    void Update()
    {
        unitNo.text = "Selecting Unit " + unitNum;
        option1.text = DisplayedHeroes[0].GetInfo();
        health1.text = "Health: "+DisplayedHeroes[0].GetHP().ToString();
        speed1.text = "Speed: " + DisplayedHeroes[0].GetSpeed().ToString();
        option2.text = DisplayedHeroes[1].GetInfo();
        health2.text = "Health: " + DisplayedHeroes[1].GetHP().ToString();
        speed2.text = "Speed: " + DisplayedHeroes[1].GetSpeed().ToString();

    }

    public void confirmButton()
    {
        if (unitNum <= 3)
        {
            if (choice1.isOn == true)
            {
                unitSelect.SpawnMe.Add(DisplayedHeroes[0]);
                DisplayedHeroes.Clear();

            }
            if (choice2.isOn == true)
            {
                unitSelect.SpawnMe.Add(DisplayedHeroes[1]);
                DisplayedHeroes.Clear();

            }
            var randomHero1 = RandomUnit<BaseHero>(UnitType.Hero);
            var hero1 = Instantiate(randomHero1);
            DisplayedHeroes.Add(hero1);
            var randomHero2 = RandomUnit<BaseHero>(UnitType.Hero);
            var hero2 = Instantiate(randomHero2);
            DisplayedHeroes.Add(hero2);



        }

        //option1 = RandomUnit();
        //option2 = RandomUnit2();

        if (unitNum == 3)
        {
            var randomHero1 = RandomUnit<BaseHero>(UnitType.Hero);
            var hero1 = Instantiate(randomHero1);
            DisplayedHeroes.Add(hero1);
            var randomHero2 = RandomUnit<BaseHero>(UnitType.Hero);
            var hero2 = Instantiate(randomHero2);
            DisplayedHeroes.Add(hero2);
            
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


}
