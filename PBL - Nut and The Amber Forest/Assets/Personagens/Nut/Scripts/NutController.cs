using UnityEngine;

/// Controla o movimento do NUT e alterna entre os visuais filhos
/// NUT_Lateral (em movimento) e NUT_Perfil (parado/idle).
/// Anexar este script no GameObject PAI ("NUT"), que deve ter
/// Rigidbody2D + Collider2D. Os filhos NUT_Lateral e NUT_Perfil
/// não precisam de Rigidbody/Collider próprios — só SpriteRenderer + Animator.
[RequireComponent(typeof(Rigidbody2D))]
public class NutController : MonoBehaviour
{
    [Header("Referências dos visuais")]
    [SerializeField] private GameObject nutLateral;
    [SerializeField] private GameObject nutPerfil;

    [Header("Movimento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.15f;

    private Rigidbody2D rb;
    private Animator animLateral;
    private Animator animPerfil;

    private float moveInput;
    private bool isMoving;
    private bool isGrounded;
    private bool facingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animLateral = nutLateral.GetComponent<Animator>();
        animPerfil = nutPerfil.GetComponent<Animator>();

        // Garante um estado visual inicial coerente
        nutLateral.SetActive(false);
        nutPerfil.SetActive(true);
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        isMoving = Mathf.Abs(moveInput) > 0.05f;
        isGrounded = groundCheck != null &&
                     Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        AtualizarVisual();
        AtualizarFlip();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }

    /// <summary>
    /// Decide qual prefab visual fica ativo e alimenta os parâmetros
    /// do Animator correspondente.
    /// </summary>
    void AtualizarVisual()
    {
        bool devePassarParaLateral = isMoving;

        // Só troca o SetActive quando realmente muda de estado,
        // pra não resetar o Animator a cada frame
        if (devePassarParaLateral && !nutLateral.activeSelf)
        {
            nutLateral.SetActive(true);
            nutPerfil.SetActive(false);
        }
        else if (!devePassarParaLateral && !nutPerfil.activeSelf)
        {
            nutPerfil.SetActive(true);
            nutLateral.SetActive(false);
        }

        if (nutLateral.activeSelf)
        {
            animLateral.SetFloat("Speed", Mathf.Abs(moveInput));
            animLateral.SetBool("Grounded", isGrounded);
        }
        else
        {
            animPerfil.SetBool("Grounded", isGrounded);
        }
    }

    void AtualizarFlip()
    {
        if (moveInput > 0 && !facingRight) Flip();
        else if (moveInput < 0 && facingRight) Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = nutLateral.transform.localScale;
        scale.x *= -1f;
        nutLateral.transform.localScale = scale;

        //Se quiser aplicar o flip também no Perfil (ex: pulo virado):
        Vector3 scaleP = nutPerfil.transform.localScale;
        scaleP.x *= -1f;
        nutPerfil.transform.localScale = scaleP;
    }
}