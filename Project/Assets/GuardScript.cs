using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class GuardScript : MonoBehaviour
{
	public enum EnemyState {Idle=0, Wandering=1, Alerted=2, PlayerInSight=3}
	public EnemyState es;
	public float lineOfSight,baseRange;
	public GameObject player;
	public NavMeshAgent nma;
	public GameObject revolver,hat,bulletPrefab;
	public Animation revolverAnim;
	public int currentAmmo,maxAmmo;
	public AnimationClip idleClip,walkClip,shootClip,reloadClip;
	public AudioSource gunSound;
	public Transform firePos;

	void Start()
	{
		player=GameObject.Find("FPSController");
	}

	public void Die()
	{
		revolver.GetComponent<Rigidbody>().isKinematic=false;
		revolver.GetComponent<Rigidbody>().useGravity=true;
		revolver.transform.parent=null;
		revolverAnim.enabled=false;
		hat.GetComponent<Rigidbody>().isKinematic=false;
		hat.GetComponent<Rigidbody>().useGravity=true;
		hat.transform.parent=null;
		this.GetComponent<Rigidbody>().isKinematic=false;
		this.GetComponent<Rigidbody>().useGravity=true;
		this.transform.parent=null;
		nma.enabled=false;
		this.enabled=false;
	}

	public int shootChance = 60;

	void FireWeapon()
	{
		GameObject bullet = (GameObject)Instantiate(bulletPrefab, firePos.position, firePos.rotation);
		Vector3 randSpread = new Vector3(Vector3.forward.x+Random.Range(0, 0.025f), Vector3.forward.y+Random.Range(0, 0.025f), Vector3.forward.z+Random.Range(0, 0.025f));
		firePos.LookAt(player.transform.position);
		Vector3 overallVect = firePos.TransformDirection(randSpread*50);
		bullet.GetComponent<Rigidbody>().velocity=overallVect;
		revolverAnim.clip=shootClip;
		revolverAnim.Play(shootClip.name);
		currentAmmo-=1;
		gunSound.Play();
	}

	void FixedUpdate()
	{
		if(es==EnemyState.PlayerInSight)
		{
			if(Random.Range(0, shootChance)==0 && currentAmmo>0 && (revolverAnim.clip==idleClip || revolverAnim.clip==walkClip))
			{
				FireWeapon();
			}
		}
		else
		{
			if(currentAmmo==0)
			{
				currentAmmo=maxAmmo;
				revolverAnim.clip=reloadClip;
				revolverAnim.Play(reloadClip.name);
			}
		}
	}

	void Update()
	{
		float rayRange = baseRange;
		float visib = player.GetComponent<FirstPersonController>().visibility;;
		if(visib>=0.8f)
			rayRange=baseRange*4;
		else if(visib>=0.6f && visib<=0.8f)
			rayRange=baseRange*3;
		else if(visib>=0.4f && visib<=0.6f)
			rayRange=baseRange*2;
		else if(visib>=0.2f && visib<=0.4f)
			rayRange=baseRange*1;
		else if(visib>=0.0f && visib<=0.2f)
			rayRange=baseRange*0;
		Vector3 rayDir = player.transform.position - transform.position;
		RaycastHit rh;
		if(!revolverAnim.isPlaying)
		{
			revolverAnim.clip=idleClip;
			revolverAnim.Play(idleClip.name);
		}
		if(nma.pathStatus==NavMeshPathStatus.PathPartial)
		{
			if(revolverAnim.clip==idleClip)
			{
				revolverAnim.clip=walkClip;
				revolverAnim.Play(walkClip.name);
			}
		}
		if(Physics.Raycast(transform.position, rayDir, out rh, rayRange))
		{
			float angle = Vector3.Angle(rayDir, transform.forward);
			if(angle<lineOfSight/2 && rh.collider.gameObject==player)
			{
				es=EnemyState.PlayerInSight;
				nma.SetDestination(player.transform.position);
			}
			else
			{
				es=EnemyState.Alerted;
				nma.SetDestination(transform.position);
			}
		}
	}
}