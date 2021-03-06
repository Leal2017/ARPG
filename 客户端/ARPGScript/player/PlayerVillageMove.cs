﻿using UnityEngine;
using System.Collections;

public class PlayerVillageMove : MonoBehaviour {

    public float velocity = 5;
    private UnityEngine.AI.NavMeshAgent agent;

    void Start() {
        agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

	
	// Update is called once per frame
	void Update () {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 vel = GetComponent<Rigidbody>().velocity;
        if (Mathf.Abs(h) > 0.05f || Mathf.Abs(v) > 0.05f) {
            GetComponent<Rigidbody>().velocity = new Vector3(-h * velocity, vel.y, -v * velocity);
            transform.rotation = Quaternion.LookRotation(new Vector3(-h, 0, -v));
        } else {
            if (agent.enabled == false) {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
        if (agent.enabled) {
            transform.rotation = Quaternion.LookRotation ( agent.velocity  );
        }
	}
}
