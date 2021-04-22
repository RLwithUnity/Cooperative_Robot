using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class box_on_target : MonoBehaviour
{
    public Agent_Robot agent_script;
    // Start is called before the first frame update

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "target")
        {
            agent_script.target_ground = true;
        }
        else
        {
            agent_script.target_ground = false;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
