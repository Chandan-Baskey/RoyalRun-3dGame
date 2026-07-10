using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManagement : MonoBehaviour
{
    [SerializeField] TMP_Text scoreUI;
    [SerializeField] TMP_Text scoreShow;
    [SerializeField] TMP_Text CoinUI;
    [SerializeField] TMP_Text lifeUI;
    [SerializeField] Image red;
    [SerializeField] GameObject GameOverUI;
   
    [SerializeField] GameObject Show;
    [SerializeField] GameObject retry;
    [SerializeField] GameObject player;
    [SerializeField] GameObject level;
    



    void Start()
    {
        
    }

    [SerializeField] float scoreFactor;
    float score= 0;
    int coin = 0;
    int life = 3;

    private void Update()
    {
        
        ScoreT();




        if (life <= 0)
        {
           GameOver();
           


        }


    }


    void GameOver()
    {
        
        Show.SetActive(false);
        GameOverUI.SetActive(true);
        scoreShow.text = coin.ToString();
        retry.SetActive(true);
        player.SetActive(false);
        level.SetActive(false);
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

    void ScoreT()
    {
        score = scoreFactor * Time.time;
        scoreUI.text = ((int)score).ToString();
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }
    
    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}
