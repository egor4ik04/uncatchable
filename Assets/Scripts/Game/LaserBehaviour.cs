using System.Collections;
using UnityEngine;
using static RandomEventsManager;

public class LaserBehaviour : MonoBehaviour
{
    [SerializeField] private int healthByItem = -2;
    [SerializeField] private Material laserMaterial;
    [SerializeField] private Color offColor;
    [SerializeField] private Color preonColor;
    [SerializeField] private Color onColor;
    [SerializeField] private float colorSwitchDuration = 0.3f;
    [SerializeField] private float laserDuration = 1f;
    [SerializeField] private float laserPeriods = 17f;

    private bool canDamage = false;
    private readonly int ShaderColorId = Shader.PropertyToID("_Color");

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.OnGameEnd -= laserReset;
        gameManager.OnGameEnd += laserReset;
        gameManager.OnGameStart -= laserStart;
        gameManager.OnGameStart += laserStart;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (canDamage && !collision.GetComponent<CatController>().IsLaying)
            {
                canDamage = false;
                gameManager.Lifes += healthByItem;
            }
        }
    }
    private void laserStart()
    {
        laserReset();
        StartCoroutine(period());
    }
    private void laserReset()
    {
        StopAllCoroutines();
        laserMaterial.SetColor(ShaderColorId, offColor);
        canDamage = false;
    }
    private IEnumerator period()
    {
        if (gameManager.IsGameStarted)
        {
            StartCoroutine(switchColor(offColor, preonColor, colorSwitchDuration));
            yield return new WaitForSeconds(colorSwitchDuration);
            StartCoroutine(switchColor(preonColor, offColor, colorSwitchDuration));
            yield return new WaitForSeconds(colorSwitchDuration);
            StartCoroutine(switchColor(offColor, preonColor, colorSwitchDuration));
            yield return new WaitForSeconds(colorSwitchDuration);
            StartCoroutine(switchColor(preonColor, offColor, colorSwitchDuration));
            yield return new WaitForSeconds(colorSwitchDuration);
            StartCoroutine(switchColor(offColor, preonColor, colorSwitchDuration));
            yield return new WaitForSeconds(colorSwitchDuration);
            StartCoroutine(switchColor(preonColor, onColor, colorSwitchDuration));
            yield return new WaitForSeconds(colorSwitchDuration);

            canDamage = true;
            yield return new WaitForSeconds(laserDuration);
            canDamage = false;
            StartCoroutine(switchColor(onColor, offColor, colorSwitchDuration * 2));
            yield return new WaitForSeconds(colorSwitchDuration * 2);
        }
        yield return new WaitForSeconds(laserPeriods);
        StartCoroutine(period());
    }
    private IEnumerator switchColor(Color start, Color end, float duration)
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            Color currentColor = Color.Lerp(start, end, t);
            timeElapsed += Time.deltaTime;

            laserMaterial.SetColor(ShaderColorId, currentColor);

            yield return null;
        }

        laserMaterial.SetColor(ShaderColorId, end);
    }
}
