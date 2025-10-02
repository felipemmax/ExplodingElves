using UnityEngine;
using UnityEngine.InputSystem;

namespace ExplodingElves.Core
{
    public class CameraControl : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float dragSpeed = 2f;
        [SerializeField] private float keyboardSpeed = 10f;
        [SerializeField] private float edgePanSpeed = 10f;
        [SerializeField] private float edgePanBorder = 50f; // Distance from screen edge to trigger panning

        [Header("Bounds Settings")]
        [SerializeField] private Vector2 minBounds = new Vector2(-50f, -50f);
        [SerializeField] private Vector2 maxBounds = new Vector2(50f, 50f);

        [Header("Smoothing")]
        [SerializeField] private bool useSmoothing = true;
        [SerializeField] private float smoothTime = 0.1f;

        [Header("Input Actions")]
        [SerializeField] private InputActionAsset inputActions;

        private Camera _camera;
        private Vector3 _dragOrigin;
        private Vector3 _velocity = Vector3.zero;
        private bool _isDragging = false;

        private InputAction _moveAction;
        private InputAction _pointerPositionAction;
        private InputAction _pointerDragAction;
        private InputAction _touchPositionAction;
        private InputAction _touchPressAction;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            SetupInputActions();
        }

        private void SetupInputActions()
        {
            if (inputActions == null)
            {
                // Create input actions programmatically if not assigned
                CreateInputActionsProgrammatically();
            }
            else
            {
                // Get actions from the assigned asset
                var cameraMap = inputActions.FindActionMap("Camera");
                _moveAction = cameraMap.FindAction("Move");
                _pointerPositionAction = cameraMap.FindAction("PointerPosition");
                _pointerDragAction = cameraMap.FindAction("PointerDrag");
                _touchPositionAction = cameraMap.FindAction("TouchPosition");
                _touchPressAction = cameraMap.FindAction("TouchPress");
            }

            // Subscribe to drag events
            _pointerDragAction.started += OnPointerDragStarted;
            _pointerDragAction.canceled += OnPointerDragCanceled;
            
            _touchPressAction.started += OnTouchPressStarted;
            _touchPressAction.canceled += OnTouchPressCanceled;
        }

        private void CreateInputActionsProgrammatically()
        {
            var cameraMap = new InputActionMap("Camera");

            // WASD Movement
            _moveAction = cameraMap.AddAction("Move", InputActionType.Value, "<Keyboard>/w");
            _moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/s")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/a")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/d")
                .With("Right", "<Keyboard>/rightArrow");

            // Mouse/Pointer Position
            _pointerPositionAction = cameraMap.AddAction("PointerPosition", InputActionType.Value, "<Mouse>/position");

            // Mouse/Pointer Drag (left button)
            _pointerDragAction = cameraMap.AddAction("PointerDrag", InputActionType.Button, "<Mouse>/leftButton");

            // Touch Position
            _touchPositionAction = cameraMap.AddAction("TouchPosition", InputActionType.Value, "<Touchscreen>/primaryTouch/position");

            // Touch Press
            _touchPressAction = cameraMap.AddAction("TouchPress", InputActionType.Button, "<Touchscreen>/primaryTouch/press");

            cameraMap.Enable();
        }

        private void OnEnable()
        {
            _moveAction?.Enable();
            _pointerPositionAction?.Enable();
            _pointerDragAction?.Enable();
            _touchPositionAction?.Enable();
            _touchPressAction?.Enable();
        }

        private void OnDisable()
        {
            _moveAction?.Disable();
            _pointerPositionAction?.Disable();
            _pointerDragAction?.Disable();
            _touchPositionAction?.Disable();
            _touchPressAction?.Disable();
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (_pointerDragAction != null)
            {
                _pointerDragAction.started -= OnPointerDragStarted;
                _pointerDragAction.canceled -= OnPointerDragCanceled;
            }

            if (_touchPressAction != null)
            {
                _touchPressAction.started -= OnTouchPressStarted;
                _touchPressAction.canceled -= OnTouchPressCanceled;
            }
        }

        private void Update()
        {
            HandleDrag();
            HandleKeyboardMovement();
            HandleEdgePanning();
        }

        private void OnPointerDragStarted(InputAction.CallbackContext context)
        {
            Vector2 screenPos = _pointerPositionAction.ReadValue<Vector2>();
            _dragOrigin = _camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Mathf.Abs(_camera.transform.position.z)));
            _isDragging = true;
        }

        private void OnPointerDragCanceled(InputAction.CallbackContext context)
        {
            _isDragging = false;
        }

        private void OnTouchPressStarted(InputAction.CallbackContext context)
        {
            Vector2 screenPos = _touchPositionAction.ReadValue<Vector2>();
            _dragOrigin = _camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Mathf.Abs(_camera.transform.position.z)));
            _isDragging = true;
        }

        private void OnTouchPressCanceled(InputAction.CallbackContext context)
        {
            _isDragging = false;
        }

        private void HandleDrag()
        {
            if (!_isDragging) return;

            // Determine which position to use (touch takes priority)
            Vector2 currentScreenPos;
            if (_touchPressAction.IsPressed())
            {
                currentScreenPos = _touchPositionAction.ReadValue<Vector2>();
            }
            else if (_pointerDragAction.IsPressed())
            {
                currentScreenPos = _pointerPositionAction.ReadValue<Vector2>();
            }
            else
            {
                return;
            }

            Vector3 currentWorldPos = _camera.ScreenToWorldPoint(new Vector3(currentScreenPos.x, currentScreenPos.y, Mathf.Abs(_camera.transform.position.z)));
            Vector3 difference = _dragOrigin - currentWorldPos;

            Vector3 targetPosition = transform.position + difference * dragSpeed * Time.deltaTime;
            targetPosition = ClampPosition(targetPosition);

            if (useSmoothing)
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime);
            }
            else
            {
                transform.position = targetPosition;
            }

            _dragOrigin = _camera.ScreenToWorldPoint(new Vector3(currentScreenPos.x, currentScreenPos.y, Mathf.Abs(_camera.transform.position.z)));
        }

        private void HandleKeyboardMovement()
        {
            Vector2 moveInput = _moveAction.ReadValue<Vector2>();

            if (moveInput != Vector2.zero)
            {
                Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);
                movement.Normalize();
                
                Vector3 targetPosition = transform.position + movement * keyboardSpeed * Time.deltaTime;
                targetPosition = ClampPosition(targetPosition);
                transform.position = targetPosition;
            }
        }

        private void HandleEdgePanning()
        {
            // Don't pan with edge if dragging
            if (_isDragging) return;

            Vector2 mousePos = _pointerPositionAction.ReadValue<Vector2>();
            Vector3 movement = Vector3.zero;

            // Check if mouse is near screen edges
            if (mousePos.x < edgePanBorder)
            {
                movement += Vector3.left;
            }
            else if (mousePos.x > Screen.width - edgePanBorder)
            {
                movement += Vector3.right;
            }

            if (mousePos.y < edgePanBorder)
            {
                movement += Vector3.back;
            }
            else if (mousePos.y > Screen.height - edgePanBorder)
            {
                movement += Vector3.forward;
            }

            if (movement != Vector3.zero)
            {
                movement.Normalize();
                Vector3 targetPosition = transform.position + movement * edgePanSpeed * Time.deltaTime;
                targetPosition = ClampPosition(targetPosition);
                transform.position = targetPosition;
            }
        }

        private Vector3 ClampPosition(Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
            position.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
            return position;
        }

        // Visualize bounds in the editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 bottomLeft = new Vector3(minBounds.x, minBounds.y, 0);
            Vector3 bottomRight = new Vector3(maxBounds.x, minBounds.y, 0);
            Vector3 topLeft = new Vector3(minBounds.x, maxBounds.y, 0);
            Vector3 topRight = new Vector3(maxBounds.x, maxBounds.y, 0);

            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);
        }
    }
}