using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MovementScript : MonoBehaviour
{

    public Slider healthBarSlider;  //reference for slider
    public Text gameOverText;   //reference for text
    private bool isGameOver = false; //flag to see if game is over


    public float speed;
    public float tilt;

    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;
    private float nextFire = 0.0f;
    public Rigidbody rb;

    void Start()
    {
        gameOverText.enabled = false; //disable GameOver text on start
        rb = GetComponent<Rigidbody>();
    }

    //Check if player enters/stays on the fire
    void OnTriggerStay(Collider other)
    {
        //if player triggers fire object and health is greater than 0
        if (other.gameObject.name == "Fire" && healthBarSlider.value > 0)
        {
            healthBarSlider.value -= .011f;  //reduce health
        }
        else
        {
            isGameOver = true;    //set game over to true
            gameOverText.enabled = true; //enable GameOver text
        }
    }

        void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        
        rb.velocity = movement*speed;


        rb.position = new Vector3
        (
            rb.position.x,
            0.0f,
            rb.position.z
        );

        rb.rotation = Quaternion.Euler(0.0f, 0.0f, rb.velocity.x * -tilt);
    }

}