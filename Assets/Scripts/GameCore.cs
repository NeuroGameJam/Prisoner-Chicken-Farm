using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerChoice
{
    Chicken, Food, GiveChicken, None
}

public class GameCore : MonoBehaviour
{
    [SerializeField] private Player p1;
    [SerializeField] private Player p2;
    [SerializeField] private Chicken chickenPrefab;
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private Text counter;
    [SerializeField] private Text Winner;
    [SerializeField] private float turnTime;
    [SerializeField] private int initialFood = 5;
    [SerializeField] private int initialChickens = 0;
    [SerializeField] private int foodAskAmmount = 5;

    private float turnCounter;
    private bool gameOn = true;

    private void Start()
    {
        SetPlayersInitialValues(p1, p2);
        StartTurn();
        UpdateUI();
    }

    private void SetPlayersInitialValues(params Player[] players)
    {
        foreach (var p in players)
        {
            for (int i = 0; i < initialFood; i++)
            {
                var food = GameObject.Instantiate(foodPrefab, p.foodSpawnPoint.position + Vector3.up * (p.foods.Count + 0.5f), Quaternion.identity);
                p.AddFood(food);
            }
            p.foodCount = initialFood;
            p.chickensCount = initialChickens;
            p.ResetChoice();
        }
    }

    private void Update()
    {
        if(gameOn)
            TurnLoop();
    }

    private void StartTurn()
    {
        turnCounter = 0;
        p1.ResetChoice();
        p2.ResetChoice();
    }

    private void TurnLoop()
    {
        p1.UpdateChoice();
        p2.UpdateChoice();

        turnCounter += Time.deltaTime;
        counter.text = ((int)(turnTime - turnCounter)).ToString();
        if (turnCounter >= turnTime)
            FinishTurn();
    }

    private void FinishTurn()
    {
        DoPlayersActions(p1, p2);
        TurnResult(p1, p2);
        StartTurn();
        UpdateUI();
    }

    private void DoPlayersActions(params Player[] players)
    {
        for (int i = 0; i < players.Length; i++)
        {
            switch (players[i].choice)
            {
                case PlayerChoice.Chicken:
                    var chicken1 = Instantiate(chickenPrefab, players[i].chickenSpawnPoint.position + (Vector3.right * players[i].chickensCount), Quaternion.identity);
                    players[i].CreateChicken(chicken1, true);
                    break;
                case PlayerChoice.Food:
                    for (int j = 0; j < players.Length; j++)
                        for (int x = 0; x < foodAskAmmount; x++)
                        {
                            var food = GameObject.Instantiate(foodPrefab, players[j].foodSpawnPoint.position + Vector3.up * (players[j].foods.Count + .5f), Quaternion.identity);
                            players[j].AddFood(food);
                        }
                break;
                case PlayerChoice.GiveChicken:
                    if (players[i].chickensCount > 0)
                    {
                        players[i].RemoveChickens(1);
                        int nextPlayer = (i + 1) % players.Length;
                        var chicken = Instantiate(chickenPrefab, players[nextPlayer].chickenSpawnPoint.position + (Vector3.right * players[nextPlayer].chickensCount), Quaternion.identity);
                        players[nextPlayer].CreateChicken(chicken, false);
                    }
                    break;
                case PlayerChoice.None:
                    break;
            }
        }
    }

    private void TurnResult(params Player[] players)
    {
        foreach (var p in players)
        {
            if (p.foodCount == 0)
            {
                print(p.name + " has lose");
                gameOn = false;
                Winner.text = "PLAYER " + p.name + " LOSE :D";
                break;
            }

            int removeFood = p.chickensCount;
            if (removeFood > p.foodCount)
                removeFood = p.foodCount;

            print(removeFood);

            for (int i = 0; i < removeFood; i++)
            {
                p.RemoveFood();
            }
        }
    }

    private void UpdateUI()
    {
        //p1FoodText.text = p1.foodCount.ToString();
        //p2FoodText.text = p2.foodCount.ToString();
    }
}

[System.Serializable]
public class Player
{
    public string name = "Player";
    public KeyCode foodKey;
    public KeyCode chickenKey;
    public KeyCode giveChickenKey;
    public Transform chickenSpawnPoint;
    public Transform foodSpawnPoint;
    public List<Chicken> chickens = new List<Chicken>();
    public List<GameObject> foods = new List<GameObject>();

    [HideInInspector] public PlayerChoice choice;

    [HideInInspector] public int chickensCount;
    [HideInInspector] public int foodCount;

    public void ResetChoice()
    {
        choice = PlayerChoice.None;
    }

    public void UpdateChoice()
    {
        if (Input.GetKey(foodKey))
            choice = PlayerChoice.Food;
        if (Input.GetKey(chickenKey))
            choice = PlayerChoice.Chicken;
        if (Input.GetKey(giveChickenKey))
            choice = PlayerChoice.GiveChicken;
    }

    public void RemoveChickens(int ammount)
    {
        chickensCount -= ammount;
        ammount = Mathf.Min(ammount, chickens.Count);
        for (int i = chickens.Count -1; i > -1; i--)
        {
            chickens[i].Kill();
        }
    }

    public void CreateChicken(Chicken chicken, bool removeFood)
    {
        if (removeFood)
        {
            if (foodCount < 1)
                return;

            RemoveFood();
        }

        chickensCount++;
        chickens.Add(chicken);
    }

    public void AddFood(GameObject food)
    {
        foodCount++;
        foods.Add(food);
    }

    public void RemoveFood()
    {
        foodCount--;
        if(foods.Count > 0)
        {
            int index = foods.Count - 1;
            GameObject.Destroy(foods[index]);
            foods.RemoveAt(index);
        }
    }
}