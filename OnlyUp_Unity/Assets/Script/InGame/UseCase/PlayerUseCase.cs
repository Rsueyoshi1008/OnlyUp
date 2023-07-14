using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using InGame.Model.Player;
using Datas.DataRepository;
public class PlayerUseCase : MonoBehaviour
{
    [SerializeField] private Transform _groundCheck;// 接触判定を取るオブジェクト
    [SerializeField] private Animator _animator; // キャラにアタッチされるアニメーターへの参照
    [SerializeField] private Transform _cameraTransform; // カメラのTransformを参照するための変数
    
    private DataRepository _repository;
    private PlayerModel _model;
    public UnityAction<PlayerModel> ChangeModel;
    private Rigidbody rb;

    

    private float rotationSpeed = 0.1f;
    private float _groundDistance = 0.1f;// 地面との判定距離

    private Vector3 _velocity;

    private bool _isSprint = false;
    
    private bool _isGrounded, _isJumpedHit, _isJumping;//   地面接触判定、ジャンプ中の判定、ジャンプ判定
    private Vector3 spawnPosition;//    スポーンする初期位置
    private float spawnCount = 0;//     オブジェクトに引っかかった時のリスポーンカウント
    public void Initialize(DataRepository repository)
    {
        _repository = repository;
        _model = new PlayerModel();
        rb = GetComponent<Rigidbody>();
        spawnPosition = transform.position;
    }
    public void SyncModel()
    {
        var player = _repository.player;

        _model.Speed = player.Speed;
        _model.SprintSpeed = player.SprintSpeed;
        _model.Attack = player.Attack;
        _model.Defense = player.Defense;
        _model.HP = player.HP;
        _model.JumpPower = player.JumpPower;

        ChangeModel?.Invoke(_model);
    }

    void Update()
    {
        bool isKeyDown = false;
        float horizontalInput = 0.0f, verticalInput = 0.0f;
        // キャラクターのレイヤーマスクを作成
        int layerMask = ~LayerMask.GetMask("Player");
        // レイキャストを使って地面との接触を検出
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, layerMask);

        _animator.SetBool("Grounded", _isGrounded);
        
        if(_isGrounded == false)//   地面についたらジャンプフラグをオフ
        {
            //_isJumping = true;
            
        }
        else
        {
            _isSprint = Input.GetKey(KeyCode.LeftShift);
        }
        
        
        //移動量の取得
        if (Input.GetKey(KeyCode.W))
        {
            verticalInput = 1.0f;
            isKeyDown = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            verticalInput = -1.0f;
            isKeyDown = true;
        }

        if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1.0f;
            isKeyDown = true;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1.0f;
            isKeyDown = true;
        }

        // 移動量を保存
        Vector3 forward = Vector3.Scale(_cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 right = Vector3.Scale(_cameraTransform.right, new Vector3(1, 0, 1)).normalized;
        _velocity = (forward * verticalInput + right * horizontalInput).normalized;
        
        if(Input.GetKeyDown(KeyCode.Space) && _isGrounded)//ジャンプ処理
        {
            Jump();
            _animator.SetBool("Jump", true);
        }
        else
        {
            _animator.SetBool("Jump", false);
        }
        
        _animator.SetFloat("V", isKeyDown ? 1.0f : 0.0f, dampTime: 0.1f, Time.deltaTime);
        
        
        //Debug.Log(verticalInput + horizontalInput);
        if(verticalInput != 0.0f || horizontalInput != 0.0f)//呼び続けるとLookRotation(movement)がゼロのログが出続けるから押したときに呼び出す
        {
            if(_isGrounded)//   地面についたときにリスポーンカウントリセット
            {
                spawnCount = 0.0f;
            }
            if(_isJumpedHit && !_isGrounded)//   空中でオブジェクトに当たったら移動不可
            {
                _animator.SetFloat("Speed", 0.0f, dampTime: 0.1f, Time.deltaTime);
                spawnCount += Time.deltaTime;//  時間測定
                Debug.Log(spawnCount);
                if(spawnCount >= 5f)
                {
                    ReSpawn();
                }

            }
            else
            {
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    ShiftAccelerate(verticalInput,horizontalInput);//   ダッシュ
                }
                else
                Move(verticalInput,horizontalInput);//  歩き
            }
            
            
        }
        else if(_isGrounded)//  入力がない時すぐに止まる
        {
            rb.velocity = new Vector3 (0f,0f,0f);
        }
        // Debug.Log(_isJumpedHit);
        // Debug.Log(_isGrounded);
        
    }
    public void Move(float verticalInput, float horizontalInput)
    {
        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        
        // 方向キーの入力値とカメラの向きから、移動方向を決定
        Vector3 movement = cameraForward * verticalInput + Camera.main.transform.right * horizontalInput;

        //移動ベクトルを正規化する
        movement = movement.normalized;

        // 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
        rb.velocity = movement * _model.Speed + new Vector3(0, rb.velocity.y, 0);

        //アニメーション
        _animator.SetFloat("Speed", _isSprint ? 2.0f : 0.7f, dampTime: 0.1f, Time.deltaTime);

        // 入力方向に回転する
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), rotationSpeed);
    }

    public void ShiftAccelerate(float verticalInput, float horizontalInput)
    {
        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        
        // 方向キーの入力値とカメラの向きから、移動方向を決定
        Vector3 movement = cameraForward * verticalInput + Camera.main.transform.right * horizontalInput;

        //移動ベクトルを正規化する
        movement = movement.normalized;

        // 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
        rb.velocity = movement * _model.SprintSpeed + new Vector3(0, rb.velocity.y, 0);

        //アニメーション
        _animator.SetFloat("Speed", _isSprint ? 2.0f : 0.7f, dampTime: 0.1f, Time.deltaTime);
        
        // 入力方向に回転する
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), rotationSpeed);
    }
    public void Jump()
    {
        rb.AddForce(Vector3.up * _model.JumpPower, ForceMode.Impulse);
    }

    public void ReSpawn()
    {
        transform.position = spawnPosition;
    }

    private void OnCollisionEnter(Collision collision)//当たった瞬間
    {
        if (collision.gameObject.CompareTag("Stairs"))
        {
            _isJumpedHit = true;
        }
    }
    private void OnCollisionExit(Collision collision)//離れた瞬間
    {
        if (collision.gameObject.CompareTag("Stairs"))
        {
            _isJumpedHit = false;
        }
    }
}
