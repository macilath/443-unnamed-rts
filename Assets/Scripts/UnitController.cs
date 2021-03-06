using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;

public abstract class UnitController : MonoBehaviour {

    protected GameObject thisShip;
    protected GameObject Electric;
    protected GameObject ElectricEffect;
    protected GameObject Afterburn;
    protected GameObject AfterburnEffect;
    protected string burnFull;
    protected string burnHalf;
    protected bool isSelected;
    public static GameManager manager; 
    protected int shipSpeed;
    protected int shipAccel;
    protected float shipSizeH;
    protected float shipSizeW;
    protected float shipRotSpeed;
    public int shipHealth;
    protected int maxHealth;
    protected Vector3 targetDest;
    protected bool hasTarget;
    protected bool facingTarget;
    protected bool targetIsEnemy;
    protected int fireInterval = 1000;
    protected Stopwatch stopwatch = new Stopwatch();
    protected Stopwatch stunTimer = new Stopwatch();
    protected int stunDuration;
    protected bool isActive = true;
    protected float angleToTarget = 0;
    protected Vector3 prevTravel = Vector3.zero;

    protected bool shipCanFire()
    {
        if(stopwatch.ElapsedMilliseconds == 0 || stopwatch.ElapsedMilliseconds >= fireInterval)
        {
            stopwatch.Reset();
            return true;
        }
        return false;
    }

    protected bool shipIsActive()
    {
        return isActive;
    }

    public void deactivate(int time)
    {
        stunTimer.Reset();
        stunTimer.Start();
        stunDuration = time;
        Vector3 effectPos = thisShip.transform.position;
        effectPos.z -= 0.5f;
        if( isActive )
        {
            ElectricEffect = Instantiate(Electric, effectPos, thisShip.transform.rotation) as GameObject;
            ElectricEffect.transform.parent = thisShip.transform;
        }
        isActive = false;
    }

    public virtual void checkStun()
    {
        if(stunTimer.ElapsedMilliseconds >= stunDuration)
        {
            stunTimer.Stop();
            isActive = true;
            if(ElectricEffect.activeInHierarchy)
            {
                Destroy(ElectricEffect);
            }
        }
    }

    public abstract void getShipSelected(bool selected);

    public abstract void setTarget();

    public abstract void takeDamage(int damage);

    public void Awake()
    {
        GameObject camera = GameObject.Find("Main Camera");
        manager = camera.GetComponent<GameManager>();
    }
    
    protected float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);
        
        if (dir > 0f) {
            return 1f;
        } else if (dir < 0f) {
            return -1f;
        } else {
            return 0f;
        }
    }

    protected void rotate(Vector3 shipPosition, Vector3 rotateTo)
    {
        Vector3 toTarget = rotateTo - shipPosition;
        Vector3 normalizedTarget = toTarget;
        toTarget.Normalize();
        Vector3 shipDir = this.transform.up;
        shipDir.Normalize();
        float shipAngle = this.transform.rotation.eulerAngles.z;
        float angleDir = AngleDir(this.transform.up, toTarget, Vector3.forward);
        float targetAngle = Vector3.Angle(Vector3.up, toTarget) * angleDir;
        if (targetAngle < 0)
        {
            targetAngle += 360;
        }
        angleToTarget = targetAngle - shipAngle;
        //Debug.Log("Rotate from " + shipAngle + " to " + targetAngle);
        //Debug.Log(rotationAngle + " degrees");

        if( Math.Abs(angleToTarget) < 5 )
        {
            this.rigidbody.angularVelocity = Vector3.zero;
            this.transform.rotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
            facingTarget = true;
        }
        else if(this.rigidbody.angularVelocity.z < shipRotSpeed)
        {
            Vector3 rotate = new Vector3(0, 0, 5 * angleDir);
            this.rigidbody.AddTorque(rotate);
        }
        this.rigidbody.AddForce( -1 * (prevTravel / shipAccel) );
    }

    protected void move(Vector3 shipPosition)
    {
        // Move ship
        Vector3 forceVector = (targetDest - shipPosition);
        forceVector.Normalize();
        Vector3 shipVelocity = this.rigidbody.velocity;
        Vector3 normalizedVelocity = shipVelocity;
        normalizedVelocity.Normalize();

        Rect boundingRect = new Rect(shipPosition.x - (shipSizeW/2), shipPosition.y - (shipSizeH/2), shipSizeW, shipSizeH);
        //Debug.Log(shipPosition - targetDest);
        if (hasTarget && boundingRect.Contains(targetDest))
        {
            //thisShip.rigidbody.AddRelativeForce(-shipVelocity * thisShip.rigidbody.mass);
            this.rigidbody.velocity = Vector3.zero;
            this.rigidbody.angularVelocity = Vector3.zero;
            //UnityEngine.Debug.Log("Destination Reached.");
            hasTarget = false;
            if(AfterburnEffect != null)
            {
                Destroy(AfterburnEffect);
            }
            return;
        }

        //TODO: change this to compare vectors using cosine to ensure ship is always trying to move to targetDest
        if (hasTarget)
        {
            if (shipVelocity.sqrMagnitude < shipSpeed && Vector3.Dot(normalizedVelocity, forceVector) != 1)
            {
                forceVector = (forceVector * shipAccel);
                //UnityEngine.Debug.Log("Accelerating");
                if( AfterburnEffect == null)
                {
                    //UnityEngine.Debug.Log("Engaging Afterburner");
                    Vector3 enginePos = thisShip.transform.position;
                    enginePos.z += 0.05f;
                    AfterburnEffect = Instantiate(Afterburn, enginePos, thisShip.transform.rotation) as GameObject;
                    tk2dSprite burner = AfterburnEffect.GetComponent<tk2dSprite>();
                    burner.SetSprite(burnFull);
                    AfterburnEffect.transform.parent = thisShip.transform;
                    AfterburnEffect.GetComponent<MeshCollider>().enabled = false;
                }
                else
                {
                    tk2dSprite burner = AfterburnEffect.GetComponent<tk2dSprite>();
                    //UnityEngine.Debug.Log("Constant Speed");
                    //UnityEngine.Debug.Log("Current Sprite name: " + burner.CurrentSprite.name);
                    if( burner.CurrentSprite.name == burnHalf )
                    {
                        //UnityEngine.Debug.Log("Engine full speed");
                        burner.SetSprite(burnFull);
                    }
                }
            }
            else
            {
                tk2dSprite burner = AfterburnEffect.GetComponent<tk2dSprite>();
                //UnityEngine.Debug.Log("Constant Speed");
                //UnityEngine.Debug.Log("Current Sprite name: " + burner.CurrentSprite.name);
                if( burner.CurrentSprite.name == burnFull )
                {
                    //UnityEngine.Debug.Log("Engine half speed");
                    burner.SetSprite(burnHalf);
                }
                forceVector = new Vector3(0, 0, 0);
            }
        }
        /*else //TODO: use this idea to have ship slow down at destination instead of just stop instantly
        {
            if (shipVelocity.sqrMagnitude > 0)
            {
                forceVector = new Vector3(0, -1, 0);
                forceVector *= shipAccel;
                thisShip.rigidbody.AddRelativeForce(forceVector);
                return;
            }
        }*/

        this.rigidbody.AddForce(forceVector);
    }

    protected virtual void checkHealth()
    {
        if(shipHealth <= 0)
        {
            GameObject Explosion = (GameObject)Resources.Load("ShipExplode1");
            Instantiate(Explosion, thisShip.transform.position, Quaternion.identity);

            Destroy(thisShip);
        }
    }

    protected abstract void fireWeapons();
}
