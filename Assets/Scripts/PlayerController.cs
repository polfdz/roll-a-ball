using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GameObject ramp1, ramp2, ramp3, ramp4, ramp5, ramp6, ramp7, ramp8;
    private int level = 1; //level of the current platform

    //Objects
    private Rigidbody rb; //the ball -- private not shown in Unity editor
    public Text countText, winText, timerText, highscoreText; //if public can be access in Unity editor

    //Other classes for attributes access 
    public RandomObjects randomObjects;

    //Timers
    private float colorTimer = 50.0f;

    //Counters
    private int count;

    //Ball speed
    public float speed, windSpeed; //public will be shown in Unity editor
    public float boxSpeed;


    Vector3 movement;
    // Use this for initialization
    void Start()
    {
        //ramp1.SetActive(false);

        //start game
        Time.timeScale = 1;
        rb = GetComponent<Rigidbody>(); //gets ball components
        count = 0;
        SetCountText();
        winText.text = "";
        timerText.text = "";
        highscoreText.text = "Highscore: " + PlayerPrefs.GetInt("highscore", 0);
    }

    // Update is called once per frame
    void Update()
    {
        //check if ball falled
        if (rb.position.y < -10) {
            GameOver(0);
        }

        //color timer behavior
        updateColorTimer();
       
        //Wind force if is active
        applyWind(2);

        //enable other platforms and ramps
        checkPlatform();

    }

    //Before performing any physics calculation
    void FixedUpdate()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVeritcal = Input.GetAxis("Vertical");

            float x = moveHorizontal;
            float y = 0.0f;
            float z = moveVeritcal;

            movement = new Vector3(x, y, z);
            rb.AddForce(movement * speed);

        }
        else {

            float moveHorizontal = Input.acceleration.x;
            float moveVertical = Input.acceleration.y;
            movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

            rb.AddForce(movement * speed * 3);
        }
    }

    // Collisions
    void OnTriggerEnter(Collider collider) {
        //Destroy(other.gameObject);

        switch (collider.gameObject.tag) {
            case "Pick Up":
                randomObjects.currentPickUp--;
                collider.gameObject.SetActive(false);
                count++;
                SetCountText();
                break;
            case "Wall":
                //Move wall
                collider.transform.Translate(0.0f, -0.3f, 0.0f);
                break;
            case "Box Time":
                randomObjects.currentTimeBoxes--;
                collider.gameObject.SetActive(false);
                colorTimer += 4;
                break;
            case "Box Wind":
                collider.gameObject.SetActive(false);
                randomObjects.desactivateWind();
                break;
            case "Box Ground":
                collider.gameObject.SetActive(false);
                randomObjects.resizeGround(0);
                break;
            case "Ramp":
                Debug.Log("RAMP");
                rb.AddForce(movement * speed * 50);
                break;


        }
    }
	 
    /*
     * Coins counting text
     * 
     */
    void SetCountText() {
        countText.text = "Score: " + count.ToString();
       /* if (count >= 10) {
            winText.text = "You Win!";
        }*/
    }

    /*
     * Color Timer behavior
     * 
     */

    void updateColorTimer() {
        colorTimer -= Time.deltaTime;
        timerText.text = "Time:\n" + colorTimer.ToString("0.#");
        if (colorTimer < 11.0f)
        {
            timerText.color = Color.red;
        }else {
            timerText.color = Color.green;
        }
        if (colorTimer < 0)
        {
            GameOver(1);
        }
    }
    //Platforms and Ramps behavior
    void checkPlatform()
    {

        level = checkLevel();

        switch (level) {
            case 2:
                ramp1.SetActive(true);
                ramp2.SetActive(true);
                break;
            case 3:
                ramp3.SetActive(true);
                ramp4.SetActive(true);
                break;
            case 4:
                ramp5.SetActive(true);
                ramp6.SetActive(true);
                ramp7.SetActive(true);
                ramp8.SetActive(true);
                break;
        }

    }
    int checkLevel() {
        if (count > 15)
        {
            return level = 4;
        }
        else if (count > 5)
        {
            return level = 3;

        }
        else if (count > 1) {
            return level = 2;
        }
        return level;
    }
    /*
     * Adds force to the ball 
     * factor - determines strength of the force
     * 
     */
    void applyWind(int factor) {
        GameObject wind = randomObjects.getActiveWind();
        float windForce = 0.2f;
        Vector3 movement;
        if (wind != null) {
            switch (wind.name) {
                case "WindZoneW":
                    movement = new Vector3(windForce, 0.0f, 0.0f);
                    rb.AddForce(movement * windSpeed * factor);
                    break;
                case "WindZoneE":
                    movement = new Vector3(-windForce, 0.0f, 0.0f);
                    rb.AddForce(movement * windSpeed * factor);
                    break;
                case "WindZoneN":
                    movement = new Vector3(0.0f, 0.0f, -windForce);
                    rb.AddForce(movement * windSpeed * factor);
                    break;
                case "WindZoneS":
                    movement = new Vector3(0.0f, 0.0f, windForce);
                    rb.AddForce(movement * windSpeed * factor);
                    break;

            }
        }

    }



    /*
     * Ends the game depending on type of ending
     * 0 ball out of the terrain
     * 1 Timer expired
     * 2
     */
    void GameOver(int type)
    {
        switch (type)
        {
            case 0:
                //Ball out the floor
                winText.text = "You lost...";
                break;
            case 1:
                //ColorTime expired
                winText.text = "Time is over...";
                timerText.text = "--";
                break;
                
        }

        StoreHighscore(count);
        //Stop app
        Time.timeScale = 0;
    }

    void StoreHighscore(int newHighscore) {

        int oldHighscore = PlayerPrefs.GetInt("highscore", 0);
        if (newHighscore > oldHighscore) {
            PlayerPrefs.SetInt("highscore", newHighscore);
        }
    }
}
