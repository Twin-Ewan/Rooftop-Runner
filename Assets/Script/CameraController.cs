using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject Player;
    Vector3 startPos;

    void Start()
    {
        Player = GameObject.Find("Player");
        startPos = this.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // When player dies they get deleted so this is to stop errors
        if(Player != null) this.transform.position = new Vector3(Player.transform.position.x + 10, startPos.y, startPos.z);
    }
}
