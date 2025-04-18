using UnityEngine;

public class Spawnin : MonoBehaviour
{
    public GameObject[] fruitPrefabs; 
    public Transform spawnArea; 
    public float spawnInterval = 2f; 
    public float fallSpeed = 0.1f; 
    public int maxMissedFruits = 10; 
    public ParticleSystem particleEffect;

    private int missedFruits = 0; // Count of missed fruits
    private int score = 0; // Player score

    void Start()
    {
        // Repeatedly spawn fruits
        InvokeRepeating("SpawnFruit", 0f, spawnInterval);
    }

    void Update()
    {
        // Check if game over condition is met
        if (missedFruits >= maxMissedFruits)
        {
            GameOver();
        }
    }

    void SpawnFruit()
    {
        // Randomly select a fruit prefab
        int randomIndex = Random.Range(0, fruitPrefabs.Length);
        GameObject fruit = Instantiate(fruitPrefabs[randomIndex]);

        // Set the fruit's position in the spawn area
        fruit.transform.position = new Vector3(
            Random.Range(spawnArea.position.x - 2.5f, spawnArea.position.x + 2.5f),
            spawnArea.position.y,
            spawnArea.position.z
        );

        // Add the falling force (gravity-like) to the fruit
        Rigidbody rb = fruit.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = 0.1f; 
        rb.drag = 0.5f; 
        rb.AddForce(Vector3.down * fallSpeed, ForceMode.Acceleration);

        // Add collider to detect grabs
        Collider col = fruit.AddComponent<SphereCollider>();
        col.isTrigger = true;

        // Attach script to handle grabbing
        FruitScript fruitScript = fruit.AddComponent<FruitScript>();
        fruitScript.parentGame = this; 
    }

    public void GrabbedFruit(GameObject fruit)
    {
        // Increase score
        score += 10;
        Debug.Log("Score: " + score);

        // Play particle effect at fruit's position
        ParticleSystem effect = Instantiate(particleEffect, fruit.transform.position, Quaternion.identity);
        effect.Play();

        // Destroy the fruit object after the particle effect plays
        Destroy(fruit);
    }

    public void MissedFruit()
    {
        // Increment missed fruits count
        missedFruits++;

        // Log missed fruits
        Debug.Log("Missed Fruits: " + missedFruits);
    }

    void GameOver()
    {
        // Display game over message
        Debug.Log("Game Over! You missed " + missedFruits + " fruits.");
        // Stop spawning fruits
        CancelInvoke("SpawnFruit");
    }
}

public class FruitScript : MonoBehaviour
{
    public Spawnin parentGame; // Reference to the parent game script

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            parentGame.GrabbedFruit(gameObject); 
        }
        else if (transform.position.y < -5f) 
        {
            parentGame.MissedFruit(); 
            Destroy(gameObject); 
        }
    }
}
