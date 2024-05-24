using System.Collections;
using UnityEngine;

// This is a copy and paste job from GameManager
// I changed the the script to work with diffrent Z positions (as normally you're going in a straight line),
// randomised spawning and gave the buildings rigidbodies so they can collide all silly like

// Beyond that though and some clean up of some unnecessary variables it's the same code, at least of today's
// GameManger, who knows if I'll update this if I add more

public class MenuBackground: MonoBehaviour
{
    [SerializeField] GameObject Buliding;
    [SerializeField] GameObject RoofPrefab;
    [SerializeField] GameObject[] Addons;

    float WaitTime = .5f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnManager());

        // Spawns 100 of buildings to make the background fill up at start
        for (int i = 0; i < 100; i++) SpawnBuilding();
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
        Vector3 BuilidngSpawnPos = new Vector3(Random.Range(-8, 8f), -13f, Random.Range(5, 20f));

        GameObject BuildingPoint = new GameObject("Building");
        BuildingPoint.AddComponent<Rigidbody>();
        BuildingPoint.GetComponent<Rigidbody>().useGravity = false;

        BuildingPoint.transform.position = BuilidngSpawnPos;

        GameObject BuildingGO = Instantiate(Buliding, BuildingPoint.transform);

        Vector3 ScaleModifier = new Vector3(Random.Range(5f, 15f), Random.Range(8f, 13f), Random.Range(2f, 6f));

        BuildingGO.GetComponentInChildren<Renderer>().material.color = Colour;
        BuildingGO.transform.localScale = ScaleModifier;

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

            BuilidngSpawnPos = new Vector3(BuilidngSpawnPos.x, 0, BuilidngSpawnPos.z);

            // Back wall
            GORoofChildTrans[0].position = new Vector3(BuilidngSpawnPos.x, ScaleModifier.y - 13f,
                (ScaleModifier.z / 2 - Seperation) + BuilidngSpawnPos.z);
            GORoofChildTrans[0].localScale = new Vector3(ScaleModifier.x, .5f, 0.4f);

            // Front wall
            GORoofChildTrans[1].position = new Vector3(BuilidngSpawnPos.x, ScaleModifier.y - 13f,
                -(ScaleModifier.z / 2 - Seperation) + BuilidngSpawnPos.z);
            GORoofChildTrans[1].localScale = new Vector3(ScaleModifier.x, .5f, 0.4f);

            // Right wall
            GORoofChildTrans[2].position = new Vector3(ScaleModifier.x / 2 - Seperation + BuilidngSpawnPos.x,
                ScaleModifier.y - 13f, BuilidngSpawnPos.z);
            GORoofChildTrans[2].localScale = new Vector3(0.4f, .5f, ScaleModifier.z);

            // Left wall
            GORoofChildTrans[3].position = new Vector3(-(ScaleModifier.x / 2 - Seperation) + BuilidngSpawnPos.x,
                ScaleModifier.y - 13f, BuilidngSpawnPos.z);
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
                if (ScaleModifier.x > 8)
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
                if (ScaleModifier.x > 8)
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

            // Top Half
            if (index == 2) { }
        }
        Destroy(BuildingGO.transform.parent.gameObject, 300);
    }
}