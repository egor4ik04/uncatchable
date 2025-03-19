using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatRunParticles : MonoBehaviour
{
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private Transform particlesContainer;
    [SerializeField] private float spawnTick = 0.2f;
    [SerializeField] private float lifeTime = 1.2f;
    private CatController catControllerScript;

    private void Start()
    {
        particlesContainer ??= GameObject.Find("ParticlesContainer").transform;
        catControllerScript = GetComponent<CatController>();
        StartCoroutine(particleSpawn());
    }

    private IEnumerator particleSpawn()
    {
        yield return new WaitForSeconds(spawnTick);

        float spawnChance = Mathf.Clamp01(Mathf.Abs(catControllerScript.Velocity)) / 2;
        if (catControllerScript.IsDashing)
            spawnChance = 1;
        bool isSpawn = spawnChance >= Random.value;
        if (isSpawn)
        {
            GameObject newParticle = Instantiate(
                particlePrefab, 
                transform.position + (Random.value > 0.5f ? Vector3.left * 0.7f : Vector3.right * 0.5f) * Mathf.Sign(catControllerScript.IsDashing ? catControllerScript.LastClickPosition.x - 0.5f : catControllerScript.Velocity) + Vector3.down * 0.5f, 
                Quaternion.AngleAxis(Random.Range(-30f, 30f), Vector3.forward), 
                particlesContainer);
            StartCoroutine(animateParticle(newParticle));
        }

        StartCoroutine(particleSpawn());
    }
    private IEnumerator animateParticle(GameObject particle)
    {
        particle.GetComponent<Animator>()?.Play("meow");
        particle.GetComponent<SpriteRenderer>().flipX = catControllerScript.IsDashing ? Mathf.Sign(catControllerScript.LastClickPosition.x - 0.5f) > 0 : catControllerScript.Velocity > 0;
        yield return new WaitForSeconds(lifeTime);
        Destroy(particle);
    }
}
