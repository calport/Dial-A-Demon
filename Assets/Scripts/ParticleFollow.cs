using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class ParticleFollow : MonoBehaviour
{
    public GameObject FollowObj;
    public GameObject ParticleTrail;
    private Vector2 newLocation;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        newLocation = new Vector2(FollowObj.transform.position.x, FollowObj.transform.position.y);
        //var worldLoc = Camera.main.ScreenToWorldPoint(newLocation);
        gameObject.transform.position = newLocation; 
    }
}
