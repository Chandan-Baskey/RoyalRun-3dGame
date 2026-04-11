using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManagement : MonoBehaviour
{
    [SerializeField] TMP_Text scoreUI;
    [SerializeField] TMP_Text CoinUI;
    [SerializeField] TMP_Text lifeUI;
    [SerializeField] Image red;
    [SerializeField] GameObject GameOverUI;

    [SerializeField] float scoreFactor;
    float score= 0;
    int coin = 0;
    int life = 1;

    private void Update()
    {
        score = scoreFactor * Time.time;
        scoreUI.text =((int)score).ToString();

        if(life <=0)
        {
            GameOverUI.SetActive(true);
        }
    }

    public void ChangeCoin(int amount)
    {
        coin += amount;
        CoinUI.text = coin.ToString();
    }

    public void ChangeLife(int amount)
    {
        life += amount;
        lifeUI.text = life.ToString();
    }

}
