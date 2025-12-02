using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
	[Header("Settings")]
	public Transform[] waypoints;      
	public float idleTime = 2f;        
	public float walkSpeed = 2f;       
	public float chaseSpeed = 4f;      
	public float sightDistance = 10f;  
	public float catchDistance = 1.5f; 

	[Header("Audio")]
	public AudioClip idleSound;
	public AudioClip walkingSound;
	public AudioClip chasingSound;

	// ตัวแปรภายใน
	private int currentWaypointIndex = 0;
	private NavMeshAgent agent;
	private Animator animator;
	private float idleTimer = 0f;
	private Transform player;
	private AudioSource audioSource;
	private PlayerInteract playerScript; 

	private enum EnemyState { Idle, Walk, Chase }
	private EnemyState currentState = EnemyState.Idle;

	private bool isChasingAnimation = false;
	private bool isGameOver = false; 

	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();

		
		GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
		if (playerObj != null)
		{
			player = playerObj.transform;
			playerScript = playerObj.GetComponent<PlayerInteract>();
		}

		
		if (waypoints.Length > 0)
		{
			SetDestinationToWaypoint();
		}
	}

	private void Update()
	{
		
		if (isGameOver || waypoints.Length == 0 || player == null) return;

		switch (currentState)
		{
			
			case EnemyState.Idle:
				idleTimer += Time.deltaTime;

				animator.SetBool("IsWalking", false);
				animator.SetBool("IsChasing", false);

				PlaySound(idleSound);

				if (idleTimer >= idleTime)
				{
					NextWaypoint();
				}

				CheckForPlayerDetection();
				break;

			
			case EnemyState.Walk:
				idleTimer = 0f;

				animator.SetBool("IsWalking", true);
				animator.SetBool("IsChasing", false);

				PlaySound(walkingSound);

				
				if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
				{
					currentState = EnemyState.Idle;
				}

				CheckForPlayerDetection();
				break;

			
			case EnemyState.Chase:
				idleTimer = 0f;

				
				if (playerScript != null && playerScript.isHidden)
				{
					
					currentState = EnemyState.Walk;
					agent.speed = walkSpeed;
					NextWaypoint();
					return;
				}

				
				agent.speed = chaseSpeed;
				agent.SetDestination(player.position);

				isChasingAnimation = true;
				animator.SetBool("IsChasing", true);
				animator.SetBool("IsWalking", false);

				PlaySound(chasingSound);

				
				float distanceToPlayer = Vector3.Distance(transform.position, player.position);
				if (distanceToPlayer <= catchDistance)
				{
					DoJumpscare(); 
				}

				
				if (Vector3.Distance(transform.position, player.position) > sightDistance)
				{
					currentState = EnemyState.Walk;
					agent.speed = walkSpeed;
					NextWaypoint();
				}
				break;
		}
	}

	
	void DoJumpscare()
	{
		isGameOver = true; 

		
		agent.isStopped = true;
		animator.enabled = false; 

		
		GameOverManager gm = FindObjectOfType<GameOverManager>();
		if (gm != null)
		{
			gm.CatchPlayer();
		}
		else
		{
			Debug.LogError("หา GameOverManager ไม่เจอ! ลากสคริปต์ GameOverManager ใส่ในฉากหรือยัง?");
		}
	}

	private void CheckForPlayerDetection()
	{
		if (playerScript != null && playerScript.isHidden) return;

		RaycastHit hit;
		Vector3 playerDirection = player.position - transform.position;

		if (Physics.Raycast(transform.position, playerDirection.normalized, out hit, sightDistance))
		{
			if (hit.collider.CompareTag("Player"))
			{
				currentState = EnemyState.Chase;
			}
		}
	}

	private void PlaySound(AudioClip soundClip)
	{
		if (audioSource.clip != soundClip)
		{
			audioSource.clip = soundClip;
			audioSource.Play();
		}
		else if (!audioSource.isPlaying)
		{
			audioSource.Play();
		}
	}

	private void NextWaypoint()
	{
		if (waypoints.Length == 0) return;
		currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
		SetDestinationToWaypoint();
	}

	private void SetDestinationToWaypoint()
	{
		if (waypoints.Length == 0) return;
		agent.SetDestination(waypoints[currentWaypointIndex].position);
		currentState = EnemyState.Walk;
		agent.speed = walkSpeed;
		if (animator != null) animator.enabled = true;
	}

	private void OnDrawGizmos()
	{
		if (player == null) return;
		Gizmos.color = currentState == EnemyState.Chase ? Color.red : Color.green;
		Gizmos.DrawLine(transform.position, transform.position + (player.position - transform.position).normalized * sightDistance);
	}
}