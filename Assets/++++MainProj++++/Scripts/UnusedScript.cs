using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CST
{

    public class UnusedScript : MonoBehaviour
    {
        public enum CameraType { FpCamera, TpCamera };

        //------------------------------------ Definitions ----------------------------------------------
        #region
        [Serializable]
        public class Components
        {
            public Camera tpCamera;
            public Camera fpCamera;

            [HideInInspector] public Transform tpRig;
            [HideInInspector] public Transform fpRoot;
            [HideInInspector] public Transform fpRig;

            [HideInInspector] public GameObject tpCamObject;
            [HideInInspector] public GameObject fpCamObject;

            [HideInInspector] public Rigidbody rBody;
            [HideInInspector] public Animator anim;
        }

        [Serializable]
        public class KeyOption
        {
            public KeyCode moveForward = KeyCode.W;
            public KeyCode moveBackward = KeyCode.S;
            public KeyCode moveRight = KeyCode.A;
            public KeyCode moveLeft = KeyCode.D;
            public KeyCode run = KeyCode.LeftShift;
            public KeyCode jump = KeyCode.Space;
            public KeyCode switchCamera = KeyCode.Tab;
            public KeyCode showCursor = KeyCode.LeftAlt;
        }

        [Serializable]
        public class MovementOption
        {
            [Range(1f, 10f), Tooltip("�̵��ӵ�")]
            public float speed = 3f;
            [Range(1f, 3f), Tooltip("�޸��� �̵��ӵ� ���� ���")]
            public float runningCoef = 1.5f;
            [Range(1f, 10f), Tooltip("���� ����")]
            public float jumpForce = 5.5f;
        }

        [Serializable]
        public class CameraOption
        {
            [Tooltip("���� ���� �� ī�޶�")]
            public CameraType initialCamera;
            [Range(1f, 10f), Tooltip("ī�޶� �����¿� ȸ�� �ӵ�")]
            public float rotationSpeed = 2f;
            [Range(-90f, 0f), Tooltip("�÷��ٺ��� ���� ����")]
            public float lookUpDegree = -60f;
            [Range(0f, 75f), Tooltip("�����ٺ��� ���� ����")]
            public float lookDownDegree = 75f;
        }

        [Serializable]
        public class AnimatorOption
        {
            public string paraMoveX = "Move X";
            public string paraMoveY = "Move Y";
            public string paraMoveZ = "Move Z";
        }

        public class CharacterState
        {
            public bool isCurrentFp;
            public bool isMoving;
            public bool isRunning;
            public bool isGrounded;
        }
        #endregion

        //--------------------------------- Fields, Properties ------------------------------------------

        #region.
        public Components Com => _components;
        public KeyOption Key => _keyOption;
        public MovementOption MoveOption => _movementOption;
        public CameraOption CamOption => _cameraOption;
        public AnimatorOption AnimOption => _animatorOption;
        public CharacterState State => _state;

        [SerializeField] private Components _components = new Components();
        [Space]
        [SerializeField] private KeyOption _keyOption = new KeyOption();
        [Space]
        [SerializeField] private MovementOption _movementOption = new MovementOption();
        [Space]
        [SerializeField] private CameraOption _cameraOption = new CameraOption();
        [Space]
        [SerializeField] private AnimatorOption _animatorOption = new AnimatorOption();
        [Space]
        [SerializeField] private CharacterState _state = new CharacterState();

        public Vector3 _moveDir;
        private Vector3 _worldMove;
        public Vector2 _rotation;

        #endregion


        private void Awake()
        {
            InitComponents();
            InitSettings();
        }

        private void InitComponents()
        {
            LogNotInitializedComponentError(Com.tpCamera, "TP Camera");
            LogNotInitializedComponentError(Com.fpCamera, "FP Camera");
            TryGetComponent(out Com.rBody);
            Com.anim = GetComponentInChildren<Animator>();

            Com.tpCamObject = Com.tpCamera.gameObject;
            Com.tpRig = Com.tpCamera.transform.parent;
            Com.fpCamObject = Com.fpCamera.gameObject;
            Com.fpRig = Com.fpCamera.transform.parent;
            Com.fpRoot = Com.fpRig.parent;
        }

        private void InitSettings()
        {
            //Rigidbody
            if (Com.rBody)
            {
                // ȸ���� Ʈ�������� ���� ���� ������ ���̱� ������ ������ٵ� ȸ���� ����
                Com.rBody.constraints = RigidbodyConstraints.FreezeRotation;
            }

            //Camera
            var allCams = FindObjectsOfType<Camera>();
            foreach (var cam in allCams)
            {
                cam.gameObject.SetActive(false);
            }

            // ������ ī�޶� �ϳ��� Ȱ��ȭ
            State.isCurrentFp = (CamOption.initialCamera == CameraType.FpCamera);
            Com.fpCamObject.SetActive(State.isCurrentFp);
            Com.tpCamObject.SetActive(State.isCurrentFp);
        }

        private void LogNotInitializedComponentError<T>(T component, string componentName) where T : Component
        {
            if (component == null)
            {
                Debug.LogError($"{componentName} ������Ʈ�� �ν����Ϳ� �־��ּ���");
            }
        }

        private void SetValuesByKeyInput()
        {
            float h = 0f, v = 0f;

            if (Input.GetKey(Key.moveForward)) v += 1.0f;
            if (Input.GetKey(Key.moveBackward)) v -= 1.0f;
            if (Input.GetKey(Key.moveRight)) h += 1.0f;
            if (Input.GetKey(Key.moveLeft)) h -= 1.0f;

            Vector3 moveInput = new Vector3(h, 0f, v).normalized;
            _moveDir = Vector3.Lerp(_moveDir, moveInput, MoveOption.runningCoef);
            _rotation = new Vector2(Input.GetAxisRaw("Mous X"), -Input.GetAxisRaw("Mouse Y"));

            State.isMoving = _moveDir.sqrMagnitude > 0.01f;
            State.isRunning = Input.GetKey(Key.run);
        }

        // 1��Ī ȸ��
        public void Rotate()
        {
            float deltaCoef = Time.deltaTime * 50f;

            // ���� : Fp rig ȸ��
            float xRotPrev = Com.fpRig.localEulerAngles.x;
            float xRotNext = xRotPrev + _rotation.y * CamOption.rotationSpeed * deltaCoef;

            if (xRotNext > 180f)
                xRotNext -= 360f;

            //�¿� : Fp root ȸ��
            float yRotPrev = Com.fpRoot.localEulerAngles.y;
            float yRotNext = yRotPrev + _rotation.x * CamOption.rotationSpeed * deltaCoef;

            // ���� ȸ�� ���� ����
            bool xRotatable = CamOption.lookUpDegree < xRotNext &&
                              CamOption.lookDownDegree > xRotNext;

            // Fp rig ���� ȸ�� ���� 
            Com.fpRig.localEulerAngles = Vector3.right * (xRotatable ? xRotNext : xRotPrev);

            // Fp Root �¿� ȸ�� ����
            Com.fpRoot.localEulerAngles = Vector3.up * yRotNext;
        }

        private void Move()
        {
            // �̵����� �ʴ� ���, �̲��� ����
            if (State.isMoving == false)
            {
                Com.rBody.velocity = new Vector3(0f, Com.rBody.velocity.y, 0f);
                return;
            }

            // ���� �̵� ���� ���
            _worldMove = Com.fpRoot.TransformDirection(_moveDir);
            _worldMove *= (MoveOption.speed) * (State.isRunning ? MoveOption.runningCoef : 1f);

            // Y�� �ӵ��� �����ϸ鼭 XZ��� �̵�
            Com.rBody.velocity = new Vector3(_worldMove.x, Com.rBody.velocity.y, _worldMove.z);
        }

        private void Update()
        {
            SetValuesByKeyInput();
            Rotate();
            Move();
        }


    }
}
