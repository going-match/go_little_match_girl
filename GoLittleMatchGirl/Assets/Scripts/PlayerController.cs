using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("플레이어 수치 설정")]
    [Range(0.5f, 1.5f)]
    public float attackTime;
    [Range(0.5f, 1.5f)]
    public float jumpTime;
    [Range(100, 1000)]
    public int jumpForce;
    [Range(1, 3)]
    public float damageEffectSpeed;

    private int invincibleTime = 1;

    private Animator topAnim, bottomAnim;
    private SpriteRenderer topSR, bottomSR;
    private GameObject match;
    private Rigidbody2D rb;
    private Vector2 boxCastSize = new Vector2(0.4f, 0.05f);
    private float speed;
    private float boxCastMaxDistance = 0.75f;
    private bool isInvincibleMode, canJump, canAttack;
    private bool isDKeyPressed;

    private IEnumerator jumpCrt;

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
        isDKeyPressed = false;
        //canSlide = true;

        speed = GameManager.Instance.GetStageSpeed();
        topAnim.speed = speed;
        bottomAnim.speed = speed;
    }

    private void Update()
    {
        // 게임 시작 전
        if (GameManager.Instance.isReady() && Input.GetKeyDown(KeyCode.Return))
        {
            GameManager.Instance.StartPlay();
            topAnim.SetTrigger("gameStart");
            bottomAnim.SetTrigger("gameStart");
        }

        // 게임 실행 중
        if (GameManager.Instance.IsPlaying())
        {
            #region 속도 테스트용 코드
            //if (Input.GetKey(KeyCode.LeftArrow))
            //{
            //    transform.position -= new Vector3(moveSpeed, 0f, 0f);
            //}

            //if (Input.GetKey(KeyCode.RightArrow))
            //{
            //    transform.position += new Vector3(moveSpeed, 0f, 0f);
            //}

            //if (Input.GetKey(KeyCode.U))
            //{
            //    UpSpeed();
            //}
            #endregion

            speed = GameManager.Instance.GetStageSpeed();

            if(Mathf.Abs(transform.position.x - 0f)>0.1f)
            {
                int direction = ((transform.position.x < 0f) ? 1 : -1);
                transform.position += new Vector3(direction * speed * 0.1f, 0f, 0f);
            }
            else GameManager.Instance.ChangePlayerCenterFlag(true);

            // ESC 키 누름; 게임 일시정지
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance.Pause();
            }

            // 점프
            if (Input.GetKeyDown(KeyCode.D) && canJump && IsOnGround())
            {
                isDKeyPressed = true;
            }

            // 성냥 던지기
            if (canAttack && Input.GetKeyDown(KeyCode.K))
            {
                StartCoroutine(AttackCrt());
            }

            //// 슬라이드
            //if (canSlide && Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.Space))
            //{
            //    Debug.Log("슬라이드");
            //    slideCrt = SlideCrt();
            //    StartCoroutine(slideCrt);
            //}
        }
        else if (GameManager.Instance.IsPaused())
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance.ChangeScene(GameManager.SCENE.MAIN);
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                topAnim.speed = speed;
                bottomAnim.speed = speed;
                GameManager.Instance.Resume();
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDKeyPressed)
        {
            isDKeyPressed = false;
            rb.AddForce(Vector2.up * jumpForce);
            jumpCrt = JumpCrt();
            StartCoroutine(jumpCrt);
        }

        if (transform.position.y < -5.0f) GameManager.Instance.GameOver();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance.IsPlaying())
        {
            if (collision.tag.Contains("Obstacle"))
            {
                if (!isInvincibleMode)
                {
                    // 생명 차감 및 무적모드
                    GameManager.Instance.AddLifeNum(-1);
                    isInvincibleMode = true;
                    if (collision.tag.Equals("Obstacle")) collision.gameObject.SetActive(false);
                    if(collision.tag.Contains("Stove")) GameManager.Instance.AddScore(-1);
                    StartCoroutine(DamageCrt());
                }
            }
            if (collision.tag.Equals("Potion"))
            {
                GameManager.Instance.AddLifeNum(1);
                collision.gameObject.SetActive(false);
            }
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
        speed = GameManager.Instance.GetStageSpeed();
        topAnim.speed = speed;
        bottomAnim.speed = speed;
    }

    public bool IsOnGround()
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
    //private IEnumerator MoveCenterCrt()
    //{
    //    while (transform.position.x < 0)
    //    {
    //        transform.position += new Vector3(speed*0.1f, 0f, 0f);
    //        yield return new WaitForSeconds(0.01f);
    //    }
    //    GameManager.Instance.ChangePlayerCenterFlag(true);
    //}

    // 성냥 던질 때; 플레이어 애니메이션 변경, 1초 후 리셋
    private IEnumerator AttackCrt()
    {
        SetState(false, true);
        topAnim.SetBool("isAttacking", true);

        yield return new WaitForSeconds(0.10f * (speed - 0.5f));
        match.SetActive(true);

        yield return new WaitForSeconds(0.14f * (speed - 0.5f));
        match.SetActive(false);
        topAnim.SetBool("isAttacking", false);

        // 상,하체 싱크 맞춤
        topAnim.Play("Player_Run_Top", -1, 0f);
        bottomAnim.Play("Player_Run_Bottom", -1, 0f);

        SetState(true, true);
    }

    // 장애물에 부딪혔을 때; 플레이어 애니메이션 변경, 깜빡임 효과 재생, 무적모드 적용
    private IEnumerator DamageCrt()
    {
        topAnim.SetBool("isDamaged",true);
        bottomAnim.SetBool("isDamaged", true);
        yield return new WaitForSeconds(0.05f);
        topAnim.SetBool("isDamaged", false);
        bottomAnim.SetBool("isDamaged", false);
        float time = 0f;
        while (time < invincibleTime)
        {
            time += Time.deltaTime;
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
        GameManager.Instance.audioController.PlayAnother(AudioController.AUDIO.JUMP);
        SetState(true, false);
        topAnim.gameObject.transform.position += new Vector3(0.15f, 0f, 0f);
        topAnim.SetBool("isJumping", true);
        bottomAnim.SetBool("isJumping", true);

        yield return new WaitForSeconds(0.01f);
        while (!IsOnGround()) yield return null;

        bottomAnim.SetBool("isJumping", false);
        topAnim.SetBool("isJumping", false);
        topAnim.gameObject.transform.position -= new Vector3(0.15f, 0f, 0f);
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
}
