using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManger : MonoBehaviour
{
    [SerializeField] GameObject Buliding;
    [SerializeField] GameObject[] Addons;
    Color[] BuildingColours;

    float oldScaleModifierX = 0;
    float WaitTime = 1;

    [SerializeField] Text DisText;
    GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
        StartCoroutine(SpawnManager());
    }

    // Update is called once per frame
    void Update()
    {
        DisText.text = "Distance: " + (int)(Player.transform.position.x * 10);
    }

    Color CreateColour()
    {
        // #7C3E44 is "103, 42, 59" in RGB or ".404, .15, .231" as Percentages
        // #672A3B is " 49, 24, 27" in RGB or ".486, .243, .267" as Percentages
        // so to pick any colour between the two we just pick a random number between two percentages

        return new Vector4(Random.Range(.404f, .486f), Random.Range(.15f, .243f), Random.Range(.231f, .267f), 1);
    }

    IEnumerator SpawnManager()
    {
        yield return new WaitForSeconds(WaitTime);
        SpawnBuilding();
        StartCoroutine(SpawnManager());
    }

    void SpawnBuilding()
    {
        Color Colour = CreateColour();
        Vector2 BuilidngSpawnPos = new Vector2(Player.transform.position.x + 30, -13);

        GameObject BuildingPoint = new GameObject("Building");
        BuildingPoint.transform.position = BuilidngSpawnPos;

        GameObject BuildingGO = Instantiate(Buliding, BuildingPoint.transform);

        Vector3 ScaleModifier = new Vector3(Random.Range(5f, 15f), Random.Range(8f, 13f), Random.Range(2f, 6f));

        BuildingGO.GetComponentInChildren<Renderer>().material.color = Colour;
        BuildingGO.transform.localScale = ScaleModifier;

        WaitTime = Player.GetComponent<Rigidbody>().velocity.x / (ScaleModifier.x + oldScaleModifierX) + 2;

        // Addon (Obstacles) Spawns 1 in 3 times
        if (Random.Range(0, 3) == 0)
        {
            int index = Random.Range(0, Addons.Length);
            GameObject GOAddon = null;
            Vector3 AddonSpawnPos;

            // Roof Access Prefab
            if (index == 0)
            {
                // Chooses a side of the building to spawn Roof Access
                float AddonWidth = Addons[index].GetComponent<MeshRenderer>().bounds.size.x;
                AddonSpawnPos = new Vector2((Random.Range(0, 2) * 2 - 1) * (ScaleModifier.x/2 - AddonWidth), ScaleModifier.y);

                GOAddon = Instantiate(Addons[index], AddonSpawnPos + BuildingGO.transform.position, 
                    Quaternion.identity, BuildingPoint.transform);

                // Makes the Roof Access face towards (away from the closest edge) 
                if (AddonSpawnPos.x < 0) GOAddon.transform.localScale = new Vector3(1, 1, 1);
                else GOAddon.transform.localScale = new Vector3(-1, 1, 1);

            }

            // Fence Prefab
            if (index == 1)
            {
                float Seperation = .5f;
                AddonSpawnPos = new Vector2(0, BuilidngSpawnPos.y + ScaleModifier.y);

                GOAddon = Instantiate(Addons[index], AddonSpawnPos + BuildingGO.transform.position,
                    Quaternion.identity, BuildingPoint.transform);

                GOAddon.transform.localScale = new Vector3(ScaleModifier.x - Seperation, Random.Range(1f, 2.5f), 
                    ScaleModifier.z - Seperation);
            }

            GOAddon.GetComponentInChildren<Renderer>().material.color = Colour;
        }
        Destroy(BuildingGO.transform.parent, 15);
        oldScaleModifierX = ScaleModifier.x;
    }
}