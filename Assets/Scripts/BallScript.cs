using UnityEngine;

public class BallScript : MonoBehaviour
{
    public float speed = 5f;
    public float powerThrow = 5f;
    [SerializeField] private Transform _point;

    private CharacterController _controller;
    private Rigidbody _rb;
    private SphereCollider _collider;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();

        // Начинаем с CharacterController
        _controller.enabled = true;
        _collider.enabled = false;
        _rb.isKinematic = true;
    }

    void Update()
    {
        // Движение влево-вправо
        if (_controller.enabled)
        {
            float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            _controller.Move(Vector3.right * moveX);
        }

        // Бросок по пробелу
        if (Input.GetKeyDown(KeyCode.Space) && _controller.enabled)
        {
            Throw();
        }
    }

    void Throw()
    {
        // Переключаем на физику
        _controller.enabled = false;
        _collider.enabled = true;
        _rb.isKinematic = false;

        // Бросаем
        _rb.AddForce(Vector3.forward * powerThrow, ForceMode.Impulse);
    }

    public void ComeBack()
    {
        // Останавливаем
        _rb.velocity = Vector3.zero;

        // Возвращаем
        transform.position = _point.position;

        // Переключаем обратно
        _controller.enabled = true;
        _collider.enabled = false;
        _rb.isKinematic = true;
    }
}