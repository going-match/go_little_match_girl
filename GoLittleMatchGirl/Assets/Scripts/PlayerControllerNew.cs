using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerNew : MonoBehaviour
{
    [Header("�÷��̾� ��ġ ����")]
    [Range(0.5f, 1.5f)]
    public float attackTime;
    [Range(0.5f, 1.5f)]
    public float jumpTime;
    [Range(1, 3)]
    public float invincibleTime;
    [Range(100, 1000)]
    public int jumpForce;
    [Range(1, 3)]
    public float damageEffectSpeed;
    [Range(0.01f, 0.1f)]
    public float initSpeed;

    [Header("�ӵ� �׽�Ʈ")]
    [Range(0.1f, 1)]
    public float moveSpeed;


    [SerializeField] private GameObject lifeGroup;
    private GameObject[] life;
    private AudioController audioController;

    private Animator topAnim, bottomAnim;
    private SpriteRenderer topSR, bottomSR;
    private GameObject match;
    private Rigidbody2D rb;
    private Vector2 boxCastSize = new Vector2(0.4f, 0.05f);
    private int playerLife = 3;
    //private int speed = 1;
    private float boxCastMaxDistance = 0.75f;
    private bool isInvincibleMode, canJump, canAttack;

    private int currentStage = 0;
    private float[] speed = { 0.15f, 0.2f, 0.25f};

    private IEnumerator attackCrt, jumpCrt;

    private void Start()
    {
        life = new GameObject[3];
        for(int i=0; i<3; i++)
        {
            life[i] = lifeGroup.transform.GetChild(i).gameObject;
        }

        audioController = gameObject.GetComponent<AudioController>();

        match = transform.GetChild(0).gameObject;
        topAnim = transform.GetChild(1).GetComponent<Animator>();
        topSR = transform.GetChild(1).GetComponent<SpriteRenderer>();
        bottomAnim = transform.GetChild(2).GetComponent<Animator>();
        bottomSR = transform.GetChild(2).GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        canAttack = true;
        canJump = true;
        //canSlide = true;

        topAnim.speed = initSpeed;
        bottomAnim.speed = initSpeed;
    }

    private void FixedUpdate()
    {
        // ���ӽ���(UI ���� �� ��ü ����)
        if (Input.GetKey(KeyCode.S))
        {
            topAnim.SetTrigger("gameStart");
            bottomAnim.SetTrigger("gameStart");
            StartCoroutine(MoveCrt());
        }

        #region �ӵ� �׽�Ʈ�� �ڵ�
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

        // ����
        if (Input.GetKey(KeyCode.D) && canJump && IsOnGround())
        {
            jumpCrt = JumpCrt();
            StartCoroutine(jumpCrt);
        }

        // ���� ������
        if (canAttack && Input.GetKey(KeyCode.K))
        {
            attackCrt = AttackCrt();
            StartCoroutine(attackCrt);
        }

        //// �����̵�
        //if (canSlide && Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.Space))
        //{
        //    Debug.Log("�����̵�");
        //    slideCrt = SlideCrt();
        //    StartCoroutine(slideCrt);
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Potion":
                playerLife = Mathf.Clamp(playerLife + 1, 0, 3);
                life[playerLife - 1].SetActive(true);
                Debug.Log("life:" + playerLife);
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
                        Debug.Log("GameOver");
                    }
                    else
                    {
                        life[playerLife].SetActive(false);
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
        currentStage++;
        topAnim.speed = speed[currentStage];
        bottomAnim.speed = speed[currentStage];
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

    // ó�� ���� �� �߾����� �޸�
    private IEnumerator MoveCrt()
    {
        while (transform.position.x < 0)
        {
            transform.position += new Vector3(moveSpeed, 0f, 0f);
            yield return new WaitForSeconds(0.05f);
        }
    }

    // ��ֹ��� �ε����� ��
    private IEnumerator DamageCrt()
    {
        topAnim.SetBool("isDamaged",true);
        bottomAnim.SetBool("isDamaged", true);
        yield return new WaitForSeconds(0.05f);
        topAnim.SetBool("isDamaged", false);
        bottomAnim.SetBool("isDamaged", false);
        isInvincibleMode = true;
        float time = 0f;
        while (time < invincibleTime)
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
        topAnim.speed = speed[currentStage]*2;
        bottomAnim.speed = speed[currentStage]*2;
        Debug.Log("����");
        SetState(true, false);
        rb.AddForce(Vector2.up * jumpForce);
        topAnim.SetBool("isJumping", true);
        bottomAnim.SetBool("isJumping", true);
        yield return new WaitForSeconds(0.01f);
        while (!IsOnGround()) yield return null;
        bottomAnim.SetBool("isJumping", false);
        topAnim.SetBool("isJumping", false);
        SetState(true, true);
        topAnim.speed = speed[currentStage];
        bottomAnim.speed = speed[currentStage];
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
        topAnim.gameObject.transform.position += new Vector3(0.15f,0f);
        topAnim.speed = speed[currentStage]*2;
        bottomAnim.speed = speed[currentStage]*2;
        Debug.Log("���� ������");
        SetState(false, true);
        topAnim.SetBool("isAttacking", true);
        match.SetActive(true);
        yield return new WaitForSeconds(attackTime);
        match.SetActive(false);
        topAnim.SetBool("isAttacking", false);
        topAnim.Play("Player_Run_Top", -1, 0f);
        bottomAnim.Play("Player_Run_Bottom", -1, 0f);
        SetState(true, true);
        topAnim.speed = speed[currentStage];
        bottomAnim.speed = speed[currentStage];
        topAnim.gameObject.transform.position -= new Vector3(0.15f, 0f);
    }
}
