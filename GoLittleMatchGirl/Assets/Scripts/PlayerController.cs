using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("플레이어 수치 설정")]
    [Range(0.5f, 1.5f)]
    public float attackTime;
    [Range(0.5f, 1.5f)]
    public float slideTime;
    [Range(0.5f, 1.5f)]
    public float jumpTime;
    [Range(1, 3)]
    public float invincibleTime;
    [Range(100, 1000)]
    public int jumpForce;
    [Range(1, 3)]
    public float damageEffectSpeed;
    [Range(0.25f, 0.5f)]
    public float speedUpValue;

    [Header("속도 테스트")]
    [Range(0.1f, 1)]
    public float moveSpeed;


    private Animator topAnim, bottomAnim;
    private SpriteRenderer topSR, bottomSR;
    private GameObject match;
    private Rigidbody2D rb;
    private Vector2 boxCastSize = new Vector2(0.4f, 0.05f);
    public int playerLife = 1;
    //private int speed = 1;
    private float boxCastMaxDistance = 0.75f;
    private bool isInvincibleMode, canJump, canAttack;

    private IEnumerator attackCrt, jumpCrt;

    private void Start()
    {
        match = transform.GetChild(0).gameObject;
        topAnim = transform.GetChild(1).GetComponent<Animator>();
        topSR = transform.GetChild(1).GetComponent<SpriteRenderer>();
        bottomAnim = transform.GetChild(2).GetComponent<Animator>();
        bottomSR = transform.GetChild(2).GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        canAttack = true;
        canJump = true;
        //canSlide = true;
    }

    private void FixedUpdate()
    {
        // 게임시작(UI 구현 후 교체 예정)
        if (Input.GetKey(KeyCode.S))
        {
            topAnim.SetTrigger("gameStart");
            bottomAnim.SetTrigger("gameStart");
            StartCoroutine(MoveCrt());
        }

        #region 속도 테스트용 코드
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position -= new Vector3(moveSpeed, 0f, 0f);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += new Vector3(moveSpeed, 0f, 0f);
        }

        if (Input.GetKey(KeyCode.U))
        {
            UpSpeed();
        }
        #endregion

        // 점프
        if (Input.GetKey(KeyCode.D) && canJump && IsOnGround())
        {
            jumpCrt = JumpCrt();
            StartCoroutine(jumpCrt);
        }

        // 성냥 던지기
        if (canAttack && Input.GetKey(KeyCode.K))
        {
            attackCrt = AttackCrt();
            StartCoroutine(attackCrt);
        }

        //// 슬라이드
        //if (canSlide && Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.Space))
        //{
        //    Debug.Log("슬라이드");
        //    slideCrt = SlideCrt();
        //    StartCoroutine(slideCrt);
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Potion":
                playerLife = Mathf.Clamp(playerLife+1, 0, 3);
                Debug.Log("life:"+playerLife);
                collision.gameObject.SetActive(false);
                break;
            case "Obstacle":
                if (!isInvincibleMode)
                {
                    collision.gameObject.SetActive(false);
                    playerLife--;
                    Debug.Log("life:" + playerLife);
                    if (playerLife == 0)
                    {
                        SceneManager.LoadScene("GameOverScene");
                        topAnim.SetTrigger("gameOver");
                        bottomAnim.SetTrigger("gameOver");
                    }
                    StartCoroutine(DamageCrt());
                }
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, LayerMask.GetMask("Ground"));

        Gizmos.color = Color.red;
        if (raycastHit.collider != null)
        {
            Gizmos.DrawRay(transform.position, Vector2.down * raycastHit.distance);
            Gizmos.DrawWireCube(transform.position + Vector3.down * raycastHit.distance, boxCastSize);
        }
        else
        {
            Gizmos.DrawRay(transform.position, Vector2.down * boxCastMaxDistance);
        }
    }

    private void UpSpeed()
    {
        topAnim.speed += speedUpValue;
        bottomAnim.speed += speedUpValue;
    }

    private bool IsOnGround()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, LayerMask.GetMask("Ground"));
        return (raycastHit.collider != null);
    }

    private void SetState(bool attack, bool jump)
    {
        canAttack = attack;
        canJump = jump;
        //canSlide = slide;
    }

    // 처음 시작 시 중앙으로 달림
    private IEnumerator MoveCrt()
    {
        while (transform.position.x < 0)
        {
            transform.position += new Vector3(moveSpeed, 0f, 0f);
            yield return new WaitForSeconds(0.05f);
        }
    }

    // 장애물에 부딪혔을 때
    private IEnumerator DamageCrt()
    {
        isInvincibleMode = true;
        float time = 0f;
        while (time<invincibleTime)
        {
            time += Time.deltaTime;
            //Debug.Log("(Mathf.Cos(time) + 1) * 0.5f:"+ Mathf.Abs(Mathf.Cos(damageEffectSpeed*time) + 1) * 0.5f);
            topSR.color = new Color(1f, 0.5f, 0.5f, Mathf.Abs(Mathf.Cos(damageEffectSpeed * time)) * 0.5f + 0.5f);
            bottomSR.color = new Color(1f, 0.5f, 0.5f, Mathf.Abs(Mathf.Cos(damageEffectSpeed * time)) * 0.5f + 0.5f);
            yield return null;
        }
        topSR.color = Color.white;
        bottomSR.color = Color.white;
        isInvincibleMode = false;
    }

    private IEnumerator JumpCrt()
    {
        Debug.Log("점프");
        SetState(true, false);
        rb.AddForce(Vector2.up * jumpForce);
        topAnim.SetBool("isJumping", true);
        bottomAnim.SetBool("isJumping", true);
        yield return new WaitForSeconds(0.01f);
        while (!IsOnGround()) yield return null; 
        bottomAnim.SetBool("isJumping", false);
        topAnim.SetBool("isJumping", false);
        SetState(true, true);
    }

    //private IEnumerator SlideCrt()
    //{
    //    SetState(false, false);
    //    transform.eulerAngles = new Vector3(0, 0, 90);
    //    yield return new WaitForSeconds(slideTime);
    //    transform.eulerAngles = Vector3.zero;
    //    SetState(true, true);
    //}

    private IEnumerator AttackCrt()
    {
        Debug.Log("성냥 던지기");
        SetState(false, true);
        topAnim.SetBool("isAttacking", true);
        match.SetActive(true);
        yield return new WaitForSeconds(attackTime);
        match.SetActive(false);
        topAnim.SetBool("isAttacking", false);
        topAnim.Play("Player_Run_Top", -1, 0f);
        bottomAnim.Play("Player_Run_Bottom", -1, 0f);
        SetState(true, true);
    }
}
