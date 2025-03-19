using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class CatController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float currentVelocity;
    public float Velocity { get => currentVelocity; }
    [SerializeField] private CatOptions catOptions;
    [SerializeField] public bool IsControllable;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private bool isDashing = false;
    public bool IsDashing { get => isDashing; }
    [SerializeField] public bool IsLaying = false;
    [SerializeField] private bool canDash = false;
    [SerializeField] private bool canMove = false;
    private float lastClickTime = 0f;
    [SerializeField] private float doubleClickTime = 0.3f;
    [SerializeField] private Vector2 lastClickPosition;
    public Vector2 LastClickPosition { get => lastClickPosition; }
    [SerializeField] private float dashCD = 1f;
    private int layCDCounter = 0;
    private readonly int isLayingAnimatorProperty = Animator.StringToHash("isLaying");
    private readonly int velocityAnimatorProperty = Animator.StringToHash("velocity");

    SpriteRenderer spriteRenderer;
    Animator animator;
    GameManager gameManager;
    IAnimType currentAnim;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        currentAnim = IAnimType.idle;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        catOptions.OnIndexChanged -= CatChanged;
        catOptions.OnIndexChanged += CatChanged;
        canDash = true;
        canMove = true;
    }

    private void Update()
    {
        if (IsControllable)
        {
            Vector2 currentClickPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            if (!isDashing)
            {
                bool isMouse1Click = Input.GetMouseButton(0);
                if (currentClickPosition.y > 0.6f)
                {
                    if (!IsLaying)
                    {
                        canMove = false;
                        StopAllCoroutines();
                        //StopCoroutine(layToMoveCD());
                        StartCoroutine(layToMoveCD());
                    }
                    IsLaying = isMouse1Click; 
                    currentVelocity = 0;
                }
                else
                {
                    IsLaying = false;
                    if (canMove && Input.GetMouseButtonUp(0))
                    {
                        if (Time.time - lastClickTime < doubleClickTime && Mathf.Abs(currentClickPosition.x - 0.5f) > 0.1f && Mathf.Abs(lastClickPosition.x - 0.5f) > 0.1f)
                            if (canDash && (currentClickPosition.x - 0.5f < 0) == (lastClickPosition.x - 0.5f < 0))
                                StartCoroutine(Dash(currentClickPosition.x - 0.5f > 0 ? 1 : -1));
                        lastClickTime = Time.time;
                        lastClickPosition = currentClickPosition;
                    }
                    if (canMove && isMouse1Click)
                    {
                        Vector2 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                        currentVelocity = Mathf.Clamp01(pos.x) * 2 - 1;
                        Move(currentVelocity);
                    }
                    else
                        currentVelocity = 0;
                }                
            }
        }
        AnimCalculate();
    }

    private IEnumerator Dash(float direction)
    {
        canDash = false;
        isDashing = true;
        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector2(transform.position.x + direction * dashSpeed, transform.position.y),
                dashSpeed * Time.deltaTime);
            yield return null;
        }
        isDashing = false;

        float dashCDCurent = 0;
        float d = 0; 
        while (dashCDCurent < dashCD)
        {
            d = Mathf.Abs(dashCDCurent / dashCD - 0.5f) + 0.5f;
            Color color = new Color(1, d, d);
            spriteRenderer.color = color;
            dashCDCurent += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = Color.white;
        canDash = true;
    }
    private void CatChanged()
    {
        animator.runtimeAnimatorController = catOptions.CatsControllers[catOptions.CurrentControllerIndex];
        currentAnim = IAnimType.idle;
        AnimCalculate();
    }
    public void Move(float x)
    {
        if (Mathf.Abs(x) > 1)
            x = x < 0 ? -1 : 1;
        currentVelocity = x;
        if (Mathf.Abs(currentVelocity) < 0.05f)
            currentVelocity = 0;
        else
        {
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector2(transform.position.x + x, transform.position.y),
                Mathf.Abs(currentVelocity) * speed * Time.deltaTime);
        }
    }
    private void AnimCalculate()
    {
        IAnimType calculated;
        if (IsLaying)
        {
            calculated = IAnimType.lay;
            animator.speed = 1;
        }
        else
        {
            if (Mathf.Abs(currentVelocity) >= 0.05f)
            {
                if (Mathf.Abs(currentVelocity) < 0.5f)
                    calculated = IAnimType.walk;
                else
                    calculated = IAnimType.run;
                animator.speed = Mathf.Abs(currentVelocity) * 2;
                spriteRenderer.flipX = currentVelocity < 0;
            }
            else
            {
                calculated = IAnimType.idle;
                animator.speed = 1;
            }
        }
        animator.SetBool(isLayingAnimatorProperty, IsLaying);
        animator.SetFloat(velocityAnimatorProperty, Mathf.Abs(currentVelocity));
        if (calculated != currentAnim)
        {
            currentAnim = calculated;
            //animator.Play(AnimationManager.GetAnimName(currentAnim));
        }
    }
    private IEnumerator layToMoveCD()
    {
        yield return null;
        while (IsLaying)
            yield return null;
        yield return new WaitForSeconds(1f);
        if (!IsLaying)
        {
            canMove = true; 
            canDash = true;
            spriteRenderer.color = Color.white;
        }
    }
}
