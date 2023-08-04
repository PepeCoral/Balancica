using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchChat;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{
    //Configuration
    [SerializeField] string chanelName;
    [SerializeField] public float timeBtwBallMovement;
    [SerializeField] int maxChangePerMessage;
    [Space(10)]
    [SerializeField] Vector2 leftPos, rightPos;
    [SerializeField] Vector2 offset;
    [SerializeField] GameObject prefabNumber;
    [SerializeField] Transform canvasForNumbers;
   
    
    VisualController visualController;

    [HideInInspector]public Vector2Int leftRightCount = Vector2Int.zero;
    [HideInInspector] public float timeSinceLastRefresh = 0;
    [HideInInspector] public int points = 0;
    [HideInInspector] public int currentPos = 0;
    private float timeSinceLastRandom = 0;
    private float timeForNextRandom;
    void Start()
    {
        visualController = GetComponent<VisualController>();
        
        visualController.UpdateHighscore();
        timeForNextRandom = Random.Range(5f, 10f);
        Screen.SetResolution(500,500,false);
        Application.targetFrameRate = 30;
    }

    private void OnEnable()
    {
        TwitchController.onTwitchMessageReceived += OnMessageRecieved;
    }
    private void OnDisable()
    {
        TwitchController.onTwitchMessageReceived -= OnMessageRecieved;
    }

    private void Update()
    {
        timeSinceLastRefresh+= Time.deltaTime;

        if (timeSinceLastRefresh >= timeBtwBallMovement)
        { timeSinceLastRefresh = 0;
            points++;
            visualController.UpdatePoints();
            AdvanceBall();
        }
        visualController.UpdateCrono();

        timeSinceLastRandom += Time.deltaTime;

        if (timeSinceLastRandom>= timeForNextRandom)
        {
            AddRandom();
        }

        
    }
    void AdvanceBall()
    {
        if (leftRightCount.x > leftRightCount.y)
        {
            currentPos--;
        }
        else if (leftRightCount.y > leftRightCount.x)
        { currentPos++; }
        visualController.UpdateNumbers();
        visualController.UpdateBall();
        
        CheckIfLost();
    }

    void OnMessageRecieved(Chatter chatter)
    {

        AddNumber(LeftRightCounter(chatter));
        

    }

    

    void AddRandom()
    {
        int amount = Random.Range(5, 41);
        if (Random.Range(0, 2) == 1)
        {
            AddNumber(new Vector2Int(amount, 0));
        }
        else
        {
            AddNumber(new Vector2Int(0, amount));
        }

        timeSinceLastRandom = 0;
        timeForNextRandom = Random.Range(20f, 50f);

        print("Random Added");
    }

    void CheckIfLost()
    {
        if (Mathf.Abs(currentPos) > 3)
        {
          

            if (!PlayerPrefs.HasKey("hs"))
            {
                PlayerPrefs.SetInt("hs", 0);
            }
            if (PlayerPrefs.GetInt("hs") < points)
            {
                PlayerPrefs.SetInt("hs", points);
            }
            points = 0;
            currentPos = 0;
            leftRightCount = Vector2Int.zero;


            visualController.UpdateHighscore();
            visualController.UpdateBalance();
            visualController.UpdateNumbers();
            visualController.UpdateBall();
            visualController.UpdatePoints();
        }
        
    }

    //LeftRight Counter
    private Vector2Int LeftRightCounter(string message)
    { 
        int left = Mathf.Min(message.Count(f => f=='l' || f == 'L'), maxChangePerMessage);
        int right = Mathf.Min(message.Count(f => f=='r' || f == 'R'), maxChangePerMessage);

        return new Vector2Int(left, right);
    }
    private Vector2Int LeftRightCounter(Chatter chatter)
    { return LeftRightCounter(chatter.message); }

    private void AddNumber(Vector2Int add)
    {
        leftRightCount += add;
        visualController.UpdateNumbers();
        visualController.UpdateBalance();

        Vector2 randOffset = new Vector2(Random.Range(-offset.x, offset.x), Random.Range(-offset.y, offset.y));

        if (add.x > 0 )
        {
            Vector3 pos = new Vector3(randOffset.x + leftPos.x, randOffset.y + leftPos.y, 0);
            GameObject a = Instantiate(prefabNumber, pos, Quaternion.identity);
            a.transform.SetParent(canvasForNumbers);
            a.GetComponent<TextMeshProUGUI>().text = "+" + add.x.ToString();
        }

        if (add.y > 0)
        {
            Vector3 pos = new Vector3(randOffset.x + rightPos.x, randOffset.y + rightPos.y, 0);
            GameObject a = Instantiate(prefabNumber, pos, Quaternion.identity);
            a.transform.SetParent(canvasForNumbers);
            a.GetComponent<TextMeshProUGUI>().text = "+" + add.y.ToString();
        }



    }

    public void ChangeTwitchName(string cName)
    { chanelName = cName;
        
    }
    public void Connect()
    { TwitchController.Login(chanelName); }
    public void ChangeTimeBtwActions(string time)
    { timeBtwBallMovement = int.Parse(time); }
    public void ChangeMaxPerMessage(string max)
    { maxChangePerMessage = int.Parse(max); }

    public void RestartHighscore()
    { PlayerPrefs.SetInt("hs", 0);
        visualController.UpdateHighscore();
    }

    public void CloseApp()
    { Application.Quit(); }
}
