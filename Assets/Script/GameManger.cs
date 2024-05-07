using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManger : MonoBehaviour
{
    [SerializeField] GameObject Buliding;
    [SerializeField] GameObject[] Addons;
    Color[] BuildingColours;

    [SerializeField] Text DisText;
    GameObject Player;

    float timeSincelastSpawn = 0;
    float timeWait = 0;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        DisText.text = "Distance: " + (int)(Player.transform.position.x * 10);

        timeSincelastSpawn += Time.deltaTime;
        while (timeSincelastSpawn >= timeWait)
        {
            SpawnBuilding();
            timeSincelastSpawn -= timeWait;
            timeWait = 20/Player.GetComponent<Rigidbody>().velocity.x;
        }
    }

    Color CreateColour()
    {
        // #7C3E44 is "103, 42, 59" in RGB or ".404, .15, .231" as Percentages
        // #672A3B is " 49, 24, 27" in RGB or ".486, .243, .267" as Percentages
        // so to pick any colour between the two we just pick a random number between two percentages

        return new Vector4(Random.Range(.404f, .486f), Random.Range(.15f, .243f), Random.Range(.231f, .267f), 1);
    }    

    void SpawnBuilding()
    {
        Color Colour = CreateColour();

        Vector2 BuilidngSpawnPos = new Vector2(Player.transform.position.x + 30, -13);
        GameObject BuildingGO = Instantiate(Buliding, BuilidngSpawnPos, Quaternion.identity);

        Vector3 ScaleModifier = new Vector3(Random.Range(5f, 15f), Random.Range(8f, 13f), Random.Range(2f, 6f));

        BuildingGO.GetComponentInChildren<Renderer>().material.color = Colour;
        BuildingGO.transform.localScale = ScaleModifier;

        if (Random.Range(0, 3) == 0 && ScaleModifier.x > 2 && ScaleModifier.y < 11)
        {
            Vector3 AddonSpawnPos = new Vector2 (Random.Range(1.5f, ScaleModifier.x - 1.5f) - ScaleModifier.x/2, 1);
            GameObject AddonGO = Instantiate(Addons[Random.Range(0, Addons.Length)], AddonSpawnPos, Quaternion.identity);
            AddonGO.transform.parent = BuildingGO.transform;
            AddonGO.GetComponent<Renderer>().material.color = Colour;
            
            Destroy(AddonGO, 15);
        }
        Destroy(BuildingGO, 15);
    }
}