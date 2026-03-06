using UnityEngine;

public class ObjectFallScript : MonoBehaviour
{
    public float deathY = 1f;
    //[HideInInspector] public bool fall = false;
    public bool fall = false;

    void Update()
    {
        if (transform.position.y < deathY)
        {
            fall = true;
        }
    }
}