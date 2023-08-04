using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VisualController : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] GameObject balance,ball;
    [SerializeField] TextMeshProUGUI leftCounter, rigthCounter, highscore,points;
    [SerializeField] List<float> places;

    bool panelShow = true;
    [SerializeField] Animator panelAnim;


    GameManager gm;
    private void Start()
    {
        gm = GetComponent<GameManager>();
    }
    public void UpdateCrono()
    { slider.value = Mathf.Clamp(gm.timeSinceLastRefresh,0,gm.timeBtwBallMovement);
        slider.maxValue = gm.timeBtwBallMovement;
    }
    public void UpdateBalance()
    {
        int difference = gm.leftRightCount.x - gm.leftRightCount.y;
        if (difference > 0)
        { RotateBalance(15); }
        else if (difference < 0)
        { RotateBalance(-15); }
        else
        { RotateBalance(0); }
    }
    void RotateBalance(float rot)
    { balance.transform.rotation = Quaternion.Euler(0, 0, rot);}

    public void UpdateNumbers()
    {
        leftCounter.text = gm.leftRightCount.x.ToString();
        rigthCounter.text = gm.leftRightCount.y.ToString();
        
    }
    public void UpdatePoints()
    { points.text = gm.points.ToString(); }
    public void UpdateBall()
    { ball.transform.localPosition = new Vector3(places[gm.currentPos+4], ball.transform.localPosition.y, 0); }
    public void UpdateHighscore()
    {
        if (!PlayerPrefs.HasKey("hs"))
        {
            PlayerPrefs.SetInt("hs", 0);
        }
        print( "record" + PlayerPrefs.GetInt("hs"));
        highscore.text = PlayerPrefs.GetInt("hs").ToString();
    }

    public void ShowHidePanel()
    {
        panelShow = !panelShow;
        panelAnim.SetBool("Show", panelShow);
    }
}
