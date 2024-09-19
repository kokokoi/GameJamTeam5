using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMove : MonoBehaviour
{
    public Transform cameraPos;
    //public Particles particles;
    
    // Start is called before the first frame update
    void Start()
    {
        //particles = new ParticleSystem.Particle[particlesSys.main.maxParticles];


        //int nbParticles = particlesSys.GetParticles(particles);


        //for (int i = 0; i < nbParticles; i++)
        //{
        //    particles[i].position = cameraPos;
        //}

        //particlesSys.SetParticles(particles, nbParticles);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 PosX = new Vector3(cameraPos.position.x, this.transform.position.y, this.transform.position.z);
        this.transform.position = PosX;
    }
}
