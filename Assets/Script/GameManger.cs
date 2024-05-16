using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManger : MonoBehaviour
{
    [SerializeField] GameObject Buliding;
    [SerializeField] GameObject RoofPrefab;
    [SerializeField] GameObject[] Addons;

    float oldScaleModifierX = 0;
    float WaitTime = 1;

    [SerializeField] Text DisText;
    float playerDist;

    [SerializeField] GameObject gameplayCanvas, gameOverCanvas;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI[] highscoreText;

    GameObject Player;
    Rigidbody PlayerRB;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
        PlayerRB = Player.GetComponent<Rigidbody>();
        StartCoroutine(SpawnManager());
    }

    // Update is called once per frame
    void Update()
    {
        if (Player != null) playerDist = Player.transform.position.x * 10;
        if (playerDist > 0) DisText.text = "Distance: " + (int)playerDist;
    }

    public void BTN_Exit()
    {
        Application.Quit();
    }

    public void BTN_Retry()
    {
        SceneManager.LoadScene("Gameplay");
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        gameplayCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);

        Time.timeScale = 0;

        // Creates a list of the highscores
        List<int> Highscores = new List<int>();
        for (int i = 0; i < 5; i++) Highscores.Add(PlayerPrefs.GetInt("Highscore" + i));

        // Adds it to the lists and sorts it from lowest to Highest
        Highscores.Add((int)playerDist);
        Highscores.Sort();

        // Then displays them whilst writing them to player's prefs
        // Starts at 1 to skip the lowest score, which is not a highscore
        for (int i = 1; i < Highscores.Count; i++)
        {
            highscoreText[i - 1].text = (-i + 6) + ": " + Highscores[i].ToString();
            PlayerPrefs.SetInt("Highscore" + (i - 1), Highscores[i]);
        }

        scoreText.text = "Your Distance: " + ((int)playerDist).ToString();
    }

    Color CreateColour()
    {
        // #7C3E44 is "103, 42, 59" in RGB or ".404, .15, .231" as Percentages
        // #672A3B is " 49, 24, 27" in RGB or ".486, .243, .267" as Percentages
        // so to pick any colour between the two we just pick a random number between two percentages

        return new Vector4(Random.Range(.404f, .486f), Random.Range(.15f, .243f), Random.Range(.231f, .267f), 1);
    }

    // Spawns a building X amount of seconds, decided by WaitTime
    IEnumerator SpawnManager()
    {
        yield return new WaitForSeconds(WaitTime);
        SpawnBuilding();
        StartCoroutine(SpawnManager());
    }

    void SpawnBuilding()
    {
        Color Colour = CreateColour();
        Vector2 BuilidngSpawnPos = new Vector2(Player.transform.position.x + 50, -13);

        GameObject BuildingPoint = new GameObject("Building");
        BuildingPoint.transform.position = BuilidngSpawnPos;

        GameObject BuildingGO = Instantiate(Buliding, BuildingPoint.transform);

        Vector3 ScaleModifier = new Vector3(Random.Range(5f, 15f), Random.Range(8f, 13f), Random.Range(2f, 6f));

        BuildingGO.GetComponentInChildren<Renderer>().material.color = Colour;
        BuildingGO.transform.localScale = ScaleModifier;

        WaitTime = (ScaleModifier.x + oldScaleModifierX) / PlayerRB.velocity.x/2;

        // Adds a roof onto the building 75% of the time
        if (Random.Range(0, 4) != 0)
        {
            float Seperation = .2f; // half the width of the wall
            Vector3 AddonSpawnPos = new Vector2(0, ScaleModifier.y);

            GameObject GORoof = Instantiate(RoofPrefab, AddonSpawnPos + BuildingGO.transform.position,
                Quaternion.identity, BuildingPoint.transform);

            // Grabs all the transforms of the walls so I can position them and scale them to fill the edges
            Transform[] GORoofChildTrans = new Transform[4];
            for (int i = 0; i < GORoof.transform.childCount; i++)
            {
                GORoofChildTrans[i] = GORoof.transform.GetChild(i).GetComponent<Transform>();
            }

            BuilidngSpawnPos = new Vector3(BuilidngSpawnPos.x, 0, 0);

            // Back wall
            GORoofChildTrans[0].position = new Vector3(BuilidngSpawnPos.x, ScaleModifier.y - 13f,
                ScaleModifier.z / 2 - Seperation);
            GORoofChildTrans[0].localScale = new Vector3(ScaleModifier.x, .5f, 0.4f);

            // Front wall
            GORoofChildTrans[1].position = new Vector3(BuilidngSpawnPos.x, ScaleModifier.y - 13f,
                -(ScaleModifier.z / 2 - Seperation));
            GORoofChildTrans[1].localScale = new Vector3(ScaleModifier.x, .5f, 0.4f);

            // Right wall
            GORoofChildTrans[2].position = new Vector3(ScaleModifier.x / 2 - Seperation + BuilidngSpawnPos.x,
                ScaleModifier.y - 13f, 0);
            GORoofChildTrans[2].localScale = new Vector3(0.4f, .5f, ScaleModifier.z);

            // Left wall
            GORoofChildTrans[3].position = new Vector3(-(ScaleModifier.x / 2 - Seperation) + BuilidngSpawnPos.x,
                ScaleModifier.y - 13f, 0);
            GORoofChildTrans[3].localScale = new Vector3(0.4f, .5f, ScaleModifier.z);

            // Sets all the colours for the walls
            for (int i = 0; i < GORoof.transform.childCount; i++)
            {
                GORoof.transform.GetChild(i).GetComponent<Renderer>().material.color = Colour;
            }
        }

        // Addon (Obstacles) Spawns 33% of the time
        if (Random.Range(0, 3) == 0)
        {
            int index = Random.Range(0, Addons.Length);
            GameObject GOAddon = null;
            Vector3 AddonSpawnPos;

            // Roof Access Prefab
            if (index == 0)
            {
                if(ScaleModifier.x > 8)
                {
                    // Chooses a side of the building to spawn Roof Access
                    float AddonWidth = Addons[index].GetComponent<MeshRenderer>().bounds.size.x;
                    AddonSpawnPos = new Vector2((Random.Range(0, 2) * 2 - 1) * (ScaleModifier.x / 2 - AddonWidth), ScaleModifier.y);

                    GOAddon = Instantiate(Addons[index], AddonSpawnPos + BuildingGO.transform.position,
                        Quaternion.identity, BuildingPoint.transform);

                    // Makes the Roof Access face away from the closest edge
                    if (AddonSpawnPos.x < 0) GOAddon.transform.localScale = new Vector3(1, 1, 1);
                    else GOAddon.transform.localScale = new Vector3(-1, 1, 1);

                    GOAddon.GetComponent<Renderer>().material.color = Colour;
                }
            }

            // Fence Prefab
            if (index == 1)
            {
                if(ScaleModifier.x > 8) 
                {
                    float Seperation = .5f;
                    AddonSpawnPos = new Vector2(0, ScaleModifier.y);

                    GOAddon = Instantiate(Addons[index], AddonSpawnPos + BuildingGO.transform.position,
                        Quaternion.identity, BuildingPoint.transform);

                    // Will make fence stretch across the whole building and have variable height
                    GOAddon.transform.localScale = new Vector3(ScaleModifier.x - Seperation, Random.Range(1f, 2.5f),
                        ScaleModifier.z - Seperation);

                    GOAddon.GetComponent<Renderer>().material.color = Colour;
                }
            }

        }
        Destroy(BuildingGO.transform.parent.gameObject, 15);
        oldScaleModifierX = ScaleModifier.x;
    }
}