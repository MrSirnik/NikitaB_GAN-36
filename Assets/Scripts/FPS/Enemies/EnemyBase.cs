using UnityEngine;
using UnityEngine.AI;

namespace FPS
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyBase : MonoBehaviour, IDamageable
    {
        private enum State { Idle, Patrol, Chase, Attack }

        [Header("Здоровье")]
        [SerializeField] private int _maxHealth = 50;

        [Header("Обзор (конус)")]
        [SerializeField] private float _viewRadius = 12f;
        [SerializeField, Range(0f, 360f)] private float _viewAngle = 110f;

        [Header("Патрулирование")]
        [SerializeField] private float _patrolRadius = 8f;
        [SerializeField] private float _idleDuration = 3f;

        [Header("Сближение")]
        [SerializeField] private float _approachDistance = 8f;

        private NavMeshAgent _agent;
        private IEnemyAttack _attack;
        private State _state;
        private float _stateTimer;
        private Vector3 _homePosition;
        private Transform _player;

        public int CurrentHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _attack = GetComponent<IEnemyAttack>();
            CurrentHealth = _maxHealth;
            _homePosition = transform.position;
        }

        private void OnEnable() => EnemyRegistry.Register(this);
        private void OnDisable() => EnemyRegistry.Unregister(this);

        private void Start()
        {
            var playerHealth = FindFirstObjectByType<PlayerHealth>();
            _player = playerHealth != null ? playerHealth.transform : null;
            EnterState(State.Idle);
        }

        private void Update()
        {
            if (IsDead) return;

            switch (_state)
            {
                case State.Idle: TickIdle(); break;
                case State.Patrol: TickPatrol(); break;
                case State.Chase: TickChase(); break;
                case State.Attack: TickAttack(); break;
            }

            Vector3 corrected = NoEnemyZone.PushOutside(transform.position);
            if (corrected != transform.position)
            {
                _agent.Warp(corrected);
            }
        }

        public void TakeDamage(int amount)
        {
            if (IsDead || amount <= 0) return;

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            if (IsDead) Die();
        }

        private void Die()
        {
            _agent.isStopped = true;
            enabled = false;
            Destroy(gameObject, 3f);
        }

        private void EnterState(State next)
        {
            _state = next;
            _stateTimer = 0f;

            switch (next)
            {
                case State.Idle:
                    _agent.isStopped = true;
                    break;
                case State.Patrol:
                    _agent.isStopped = false;
                    SetPatrolDestination();
                    break;
                case State.Chase:
                    _agent.isStopped = false;
                    break;
                case State.Attack:
                    _agent.isStopped = true;
                    break;
            }
        }

        private void TickIdle()
        {
            _stateTimer += Time.deltaTime;

            if (CanSeePlayer())
            {
                EnterState(State.Chase);
                return;
            }

            if (_stateTimer >= _idleDuration)
            {
                EnterState(State.Patrol);
            }
        }

        private void TickPatrol()
        {
            if (CanSeePlayer())
            {
                EnterState(State.Chase);
                return;
            }

            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                EnterState(State.Idle);
            }
        }

        private void TickChase()
        {
            if (!CanSeePlayer())
            {
                EnterState(State.Idle);
                return;
            }

            Vector3 destination = NoEnemyZone.AdjustDestination(transform.position, _player.position);
            _agent.SetDestination(destination);

            float distance = Vector3.Distance(transform.position, _player.position);
            float attackRange = _attack != null ? _attack.AttackRange : _approachDistance;
            if (distance <= attackRange)
            {
                EnterState(State.Attack);
            }
        }

        private void TickAttack()
        {
            if (_player == null)
            {
                EnterState(State.Idle);
                return;
            }

            transform.LookAt(new Vector3(_player.position.x, transform.position.y, _player.position.z));

            float attackRange = _attack != null ? _attack.AttackRange : _approachDistance;
            float distance = Vector3.Distance(transform.position, _player.position);
            if (distance > attackRange)
            {
                EnterState(State.Chase);
                return;
            }

            _attack?.TryAttack(_player);
        }

        private void SetPatrolDestination()
        {
            Vector2 offset = Random.insideUnitCircle * _patrolRadius;
            Vector3 candidate = _homePosition + new Vector3(offset.x, 0f, offset.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, _patrolRadius, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
            }
        }

        private bool CanSeePlayer()
        {
            if (_player == null) return false;

            Vector3 toPlayer = _player.position - transform.position;
            float distance = toPlayer.magnitude;
            if (distance > _viewRadius) return false;

            float angle = Vector3.Angle(transform.forward, toPlayer);
            return angle <= _viewAngle * 0.5f;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _viewRadius);
        }
    }
}
