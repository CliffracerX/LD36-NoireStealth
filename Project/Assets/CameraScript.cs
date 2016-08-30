using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

public class CameraScript : MonoBehaviour
{
	public FirstPersonController fpc;
	public int corePapersLeft = 1;
	public int bonusPapersLeft = 1;
	public int guardsKilled = 0;
	public int guardsTranquilized = 0;
	public GUIStyle style;
	public static CameraScript instance;
	public AudioSource normalMusSrc,hackMusSrc;
	public GameObject exit,bulletPrefab;
	public int currentAmmo,maxAmmo;
	public Animation wepAnim;
	public AnimationClip idleClip,walkClip,baseClip,sprintClip,shootClip,reloadClip;
	public Transform firePos;
	public AudioSource gunSound;
	public bool resetRotYet = false;
	public Transform deathCamBase;
	public MouseLook mLook;
	public RenderTexture rt;

	void Start()
	{
		rt=RenderTexture.active;
		CameraScript.instance=this;
		exit=GameObject.Find("Exit");
		wepAnim[shootClip.name].speed=0.75f;
		wepAnim[shootClip.name].layer=4;
		wepAnim[reloadClip.name].speed=0.5f;
		wepAnim[reloadClip.name].layer=4;
		wepAnim[walkClip.name].layer=2;
		wepAnim[walkClip.name].speed=0.5f;
		wepAnim[walkClip.name].blendMode=AnimationBlendMode.Additive;
		wepAnim[idleClip.name].layer=2;
		wepAnim[idleClip.name].speed=0.25f;
		wepAnim[idleClip.name].blendMode=AnimationBlendMode.Additive;
		wepAnim[sprintClip.name].layer=1;
		wepAnim[sprintClip.name].speed=0.0f;
		wepAnim[baseClip.name].layer=1;
		wepAnim[baseClip.name].speed=0.0f;
		//wepAnim[sprintClip.name].blendMode=AnimationBlendMode.Additive;
		if(exit)
		{
			exit.SetActive(false);
		}
	}

	void FireWeapon()
	{
		GameObject bullet = (GameObject)Instantiate(bulletPrefab, firePos.position, firePos.rotation);
		Vector3 randSpread = new Vector3(Vector3.forward.x+Random.Range(0, 0.025f), Vector3.forward.y+Random.Range(0, 0.025f), Vector3.forward.z+Random.Range(0, 0.025f));
		Vector3 overallVect = transform.TransformDirection(randSpread*50);
		bullet.GetComponent<Rigidbody>().velocity=overallVect;
		wepAnim.CrossFade(shootClip.name);
		currentAmmo-=1;
		gunSound.Play();
	}

	void OnGUI()
	{
		RenderTexture tempRT = RenderTexture.active;
		RenderTexture.active=rt;
		GUI.Label(new Rect(15, 15+24, 240, 24), "Core Data Left: "+corePapersLeft, style);
		GUI.Label(new Rect(15, 15+48, 240, 24), "Bonus Data Remaining: "+bonusPapersLeft, style);
		GUI.Label(new Rect(15, 15+62, 240, 24), "Weapon Ammo: "+currentAmmo+"/"+maxAmmo, style);
		RenderTexture.active=tempRT;
	}

	public Quaternion targetRot,targetRot2;

	Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;
		
		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);
		
		angleX = Mathf.Clamp (angleX, -90, 90);
		
		q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);
		
		return q;
	}

	void Update()
	{
		if(fpc.enabled)
		{
			normalMusSrc.volume=SaveManager.instance.musVol;
			hackMusSrc.volume=0;
		}
		else
		{
			normalMusSrc.volume=0;
			hackMusSrc.volume=SaveManager.instance.musVol;
		}
		if(fpc.dead)
		{
			if(!resetRotYet)
			{
				transform.rotation=Quaternion.identity;
				deathCamBase.rotation=Quaternion.identity;
				targetRot=transform.rotation;
				targetRot2=deathCamBase.rotation;
				resetRotYet=true;
				mLook.Init(deathCamBase, transform);
			}
			if(Input.GetButtonUp("Reload"))
			{
				Application.LoadLevel(Application.loadedLevel);
			}
			deathCamBase.position+=(transform.forward*Input.GetAxis("Vertical"));
			deathCamBase.position+=(transform.right*Input.GetAxis("Horizontal"));
			deathCamBase.parent=null;
			transform.parent=deathCamBase;
			transform.localPosition=Vector3.zero;
			mLook.LookRotation(deathCamBase, transform);

		}
		if(fpc.enabled)
		{
			if(Input.GetAxis("Horizontal")!=0 || Input.GetAxis("Vertical")!=0)
			{
				wepAnim.CrossFade(walkClip.name);
				wepAnim.Stop(idleClip.name);
			}
			else
			{
				wepAnim.CrossFade(idleClip.name);
				wepAnim.Stop(walkClip.name);
			}
			if(Input.GetButton("Sprint"))
			{
				wepAnim.CrossFade(sprintClip.name);
			}
			else
			{
				wepAnim.CrossFade(baseClip.name);
			}
		}
		if(Input.GetButtonUp("Fire1") && !fpc.dead)
		{
			if(Cursor.lockState==CursorLockMode.None)
			{
				Cursor.lockState=CursorLockMode.Locked;
			}
			else
			{
				if(wepAnim.isPlaying && wepAnim.clip!=reloadClip && wepAnim.clip!=shootClip && currentAmmo>0)
					FireWeapon();
			}
		}
		if(Input.GetButtonUp("Pause"))
		{
			if(fpc.enabled)
			{
				Cursor.lockState=CursorLockMode.None;
			}
		}
		if(Input.GetButtonUp("Reload") && fpc.enabled)
		{
			wepAnim.CrossFade(reloadClip.name);
			currentAmmo=maxAmmo;
		}
		/*if(wepAnim.isPlaying==false)
		{
			if(wepAnim.clip==reloadClip)
			{
				currentAmmo=maxAmmo;
			}
			wepAnim.clip=idleClip;
			wepAnim.Play(idleClip.name);
		}*/
		if(Input.GetButtonUp("Interact") && !fpc.dead)
		{
			Ray r = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
			RaycastHit rh;
			if(Physics.Raycast(r, out rh))
			{
				if(rh.collider.GetComponent<ConsoleScript>()!=null)
				{
					fpc.enabled=false;
					rh.collider.GetComponent<ConsoleScript>().fpc=fpc;
					rh.collider.GetComponent<ConsoleScript>().currentlyFocused=true;
				}
				if(rh.collider.gameObject.name=="CorePapers")
				{
					corePapersLeft-=1;
					Destroy(rh.collider.gameObject);
					if(corePapersLeft<=0)
					{
						exit.SetActive(true);
					}
				}
				if(rh.collider.gameObject.name=="BonusPapers")
				{
					bonusPapersLeft-=1;
					Destroy(rh.collider.gameObject);
				}
			}
		}
	}
}