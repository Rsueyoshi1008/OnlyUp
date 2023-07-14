using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnitController : MonoBehaviour
{
    // ---------- 定数宣言 ----------
    // ---------- プレハブ ----------
    // ---------- ゲームオブジェクト参照変数宣言 ----------
    [SerializeField] private Animator _animator; // キャラにアタッチされるアニメーターへの参照
    [SerializeField] private Transform _cameraTransform; // カメラのTransformを参照するための変数
    [SerializeField] private Rigidbody _rigidBody; // 剛体
    // ---------- プロパティ ----------
    [SerializeField] private float _rotateSpeed = 400f; // キャラクターの回転速度
    [SerializeField] private float _moveSpeed = 3.0f; // 移動速度
    [SerializeField] private float _sprintSpeed = 5.0f; // 移動速度
    [SerializeField] private float _jumpPower = 6.0f; // ジャンプ威力
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundDistance = 0.1f;
    private bool _isSprint = false;
    private bool _isJumping;
    private bool _isGrounded;
    // ---------- クラス変数宣言 ----------
    // ---------- インスタンス変数宣言 ----------
    private Vector3 _velocity;
    // ---------- コンストラクタ・デストラクタ ----------
    // ---------- Unity組込関数 ----------
    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        // Inputから移動量を取得
        float vertical = 0.0f;
        float horizontal = 0.0f;
        bool isKeyDown = false;

        if (Input.GetKey(KeyCode.W))
        {
            vertical = 1.0f;
            isKeyDown = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            vertical = -1.0f;
            isKeyDown = true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            horizontal = -1.0f;
            isKeyDown = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            horizontal = 1.0f;
            isKeyDown = true;
        }

        // 移動量を保存
        Vector3 forward = Vector3.Scale(_cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 right = Vector3.Scale(_cameraTransform.right, new Vector3(1, 0, 1)).normalized;

        _velocity = (forward * vertical + right * horizontal).normalized;

        _animator.SetFloat("V", isKeyDown ? 1.0f : 0.0f, dampTime: 0.1f, Time.deltaTime);

        // ジャンプ
        if (Input.GetButtonDown("Jump"))
        {
            _isJumping = true;
        }
        else
        {
            _isSprint = Input.GetKey(KeyCode.LeftShift);
        }
    }

    private void FixedUpdate()
    {
        // 重力下での移動処理
        Move();

        // キャラクターのレイヤーマスクを作成
        int layerMask = ~LayerMask.GetMask("Player");

        // レイキャストを使って地面との接触を検出
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, layerMask);
        _animator.SetBool("Grounded", _isGrounded);

        // ジャンプ処理
        if (_isGrounded && _isJumping)
        {
            Jump();
            _animator.SetBool("Jump", true);
            _isJumping = false;
        }

        // ジャンプ停止処理
        if (!_isGrounded && _animator.GetBool("Jump"))
        {
            _animator.SetBool("Jump", false);
        }
    }
    // ---------- Public関数 ----------
    // ---------- Protected関数 ----------
    // ---------- Private関数 ----------

    // 初期化
    protected virtual void Initialize()
    {
        return;
    }

    // 移動
    protected virtual void Move()
    {
        if (_velocity.magnitude <= 0.0f)
        {
            _rigidBody.velocity = new Vector3(0, _rigidBody.velocity.y, 0);
            _animator.SetFloat("Speed", 0.0f, dampTime: 0.1f, Time.deltaTime);
            return;
        }

        if (_velocity.magnitude >= 0.15f)
        {
            // キャラクターの向きを移動方向に即座に変更
            Quaternion lookRotation = Quaternion.LookRotation(_velocity);
            // Y軸の回転のみを抽出
            Quaternion targetRotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);

            // 目的の向きへ徐々に近づく
            Quaternion smoothedRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);

            transform.rotation = smoothedRotation;
        }

        // 移動量に速度を掛ける
        float speed = _isSprint ? _sprintSpeed : _moveSpeed;
        Vector3 moveVelocity = _velocity.normalized * speed;
        _animator.SetFloat("Speed", _isSprint ? 2.0f : 0.7f, dampTime: 0.1f, Time.deltaTime);

        // y軸の速度はそのままにする（自由落下等の影響を受けるため）
        moveVelocity.y = _rigidBody.velocity.y;

        // Rigidbodyの速度に直接代入することで移動
        _rigidBody.velocity = moveVelocity;
    }

    // ジャンプ処理
    protected virtual void Jump()
    {
        // Rigidbodyに上方向への力を追加
        _rigidBody.AddForce(new Vector3(0, _jumpPower, 0), ForceMode.VelocityChange);
    }
}