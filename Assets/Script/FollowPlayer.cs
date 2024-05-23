using System;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    GameObject Player;
    Vector3 startPos;


    [Flags] enum axises { X = 1, Y = 2, Z = 4};
    [SerializeField] axises EnabledAxies;

    [SerializeField] Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        Player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = startPos;
        if (EnabledAxies.HasFlag(axises.X)) newPos = new Vector3(Player.transform.position.x + offset.x + newPos.x, newPos.y, newPos.z);
        if (EnabledAxies.HasFlag(axises.Y)) newPos = new Vector3(newPos.x , Player.transform.position.y + offset.y + newPos.y, newPos.z);
        if (EnabledAxies.HasFlag(axises.Z)) newPos = new Vector3(newPos.x, newPos.y, Player.transform.position.z + offset.z + newPos.z);


        this.transform.position = newPos;
    }
}
