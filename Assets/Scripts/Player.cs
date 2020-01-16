using System;
using UnityEngine;

namespace Entities
{
    public class Player : LiveEntity
    {
        //DECLARING VARIABLES
        private PlayerObject objectScript;
        private float hookDamage = 5;
        private float hookKnockback = 80;

        private float shootSpeed = 0;
        public Vector2 hookVelocity;
        private float initialAngle = 0;
        private float targetAngle = 0;
        private float rotateTarget = 0;
        private float rotationPercentage;
        private float hookSize;
        private float moveAngle = 0;
        private float hookAngle = 0;
        private float lastHookAngle = 0;
        private float lastMoveAngle;
        private float loadTimer = 0;
        public hState hookState { get; private set; }

        private int moveX = 0;
        private int moveY = 0;
        private bool movingLastFrame = false;
        private bool hookBackLastFrame = false;

        public enum hState
        {
            fired, loading, loaded
        }
        private GameObject anchor;
        private GameObject hook;

        public Player(Vector2 position)
        {
            attachedObject = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/PlayerObject"), new Vector2(position.x, position.y), Quaternion.identity);
            objectScript = attachedObject.GetComponent<PlayerObject>();
            objectScript.linkedScript = this;

            anchor = attachedObject.transform.GetChild(0).gameObject;
            hook = anchor.transform.GetChild(0).gameObject;
            hookSize = hook.GetComponent<CircleCollider2D>().radius;
            moveSpeed = 20;
        }

        private void processHit(LiveEntity target, RaycastHit2D collision)
        {
            if (!target.invincible)
            {
                target.TakeDamage(hookDamage);
                Transform t = target.attachedObject.transform;
                Vector2 knockback = (-collision.normal + Mathf.Sign(shootSpeed)*hookVelocity.normalized).normalized/2 * hookKnockback;
                //Vector2 knockback = collision.normal * -hookKnockback;
                target.ApplyKnockback(knockback, 0.2f, 0.2f, 0.05f);
            }
        }

        public void parseHookCollisionData(RaycastHit2D[] col)
        {
            foreach (RaycastHit2D i in col)
            {
                if (i.transform.gameObject.tag == "Enemy")
                {
                    processHit(i.transform.gameObject.GetComponent<EnemyObject>().linkedScript, i);
                }
            }
        }

        public override void Update()
        {
            base.Update();
            //INPUTS
            moveX = (Input.GetKey("right") ? 1 : 0) - (Input.GetKey("left") ? 1 : 0);
            moveY = (Input.GetKey("up") ? 1 : 0) - (Input.GetKey("down") ? 1 : 0);
            bool directHookBack = Input.GetKey("q");
            bool lockHookRotation = Input.GetKey("w");
            float speedMod = (moveX != 0 && moveY != 0) ? 1 / Mathf.Sqrt(2) : 1;
            float appliedSpeed = speedMod * moveSpeed;

            //MOVEMENT
            Vector2 v = velocity;
            v.x = (moveX != 0) ? v.x + moveX * 1.5f : Mathf.Sign(v.x) * Mathf.Max(0, Mathf.Abs(v.x) - 60 * Time.deltaTime);
            v.x = Mathf.Clamp(v.x, -appliedSpeed, appliedSpeed);
            v.y = (moveY != 0) ? v.y + moveY * 1.5f : Mathf.Sign(v.y) * Mathf.Max(0, Mathf.Abs(v.y) - 60 * Time.deltaTime);
            v.y = Mathf.Clamp(v.y, -appliedSpeed, appliedSpeed);

            velocity = v;

            Vector2 currentPos = hook.transform.position;
            attachedObject.transform.Translate(velocity * Time.deltaTime);

            if (moveState != LiveEntity.moveStates.stunned)
            {
                if (moveX != 0 || moveY != 0)
                {
                    moveAngle = Vector2.SignedAngle(Vector2.right, new Vector2(10 * moveX, 10 * moveY));
                    moveState = LiveEntity.moveStates.active;
                }
                else
                {
                    moveState = LiveEntity.moveStates.stationary;
                }
            }

            lastHookAngle = hookAngle;
            hookAngle = moveAngle - (directHookBack ? Mathf.Sign(moveAngle) * 180 : 0);

            //HOOK ROTATION
            float eulersZ = anchor.transform.localRotation.eulerAngles.z;
            //TODO: understand this better
            float diff = (hookAngle - eulersZ + 180) % 360 - 180;
            diff = Mathf.Abs(diff < -180 ? diff + 360 : diff);

            if (hookAngle != lastHookAngle)
            {
                if (!movingLastFrame && hookBackLastFrame == directHookBack)
                {
                    moveAngle = lastMoveAngle;
                    hookAngle = lastHookAngle;
                }
                else
                {
                    initialAngle = eulersZ;
                    targetAngle = hookAngle;
                    rotationPercentage = 0;
                }
            }
            if (!lockHookRotation)
            {
                rotationPercentage = Mathf.Min(1, rotationPercentage + Time.deltaTime / 0.4f);
                rotateTarget = Mathf.LerpAngle(initialAngle, targetAngle, 1 - Mathf.Pow((rotationPercentage - 1), 2));
                anchor.transform.localRotation = Quaternion.Euler(0, 0, rotateTarget);
            }

            //HOOK PROPULSION
            if (hook.transform.localPosition.x == 0.2f && hookState == hState.fired)
            {
                hookState = hState.loading;
                loadTimer = 0.1f;
            }

            if (hookState == hState.loaded)
            {
                shootSpeed = 0;
                if (Input.GetKey("space"))
                {
                    shootSpeed = 9;
                    hookState = hState.fired;
                }
            }
            else
            {
                shootSpeed -= 60 * Time.deltaTime * (!Input.GetKey("space") ? 1 : 0.35f);
            }

            if (hookState == hState.loading && (loadTimer <= 0 || !Input.GetKey("space")))
            {
                hookState = hState.loaded;
            }
            else
            {
                loadTimer -= Time.deltaTime;
            }

            Transform ht = hook.transform;
            Vector2 nextPos = ht.TransformPoint(new Vector2(Mathf.Max(ht.localPosition.x + shootSpeed * Time.deltaTime, 0.2f), 0));
            hookVelocity = nextPos - currentPos;
            if (hookState == hState.fired)
            {
                parseHookCollisionData(Physics2D.CircleCastAll(currentPos, hookSize, hookVelocity, shootSpeed * Time.deltaTime));
            }
            ht.localPosition = new Vector2(Mathf.Max(ht.localPosition.x + shootSpeed * Time.deltaTime, 0.2f), 0);

            //TRACKING
            movingLastFrame = (moveState == LiveEntity.moveStates.active);
            hookBackLastFrame = directHookBack;
            lastMoveAngle = moveAngle;

            //GRAPHICS
            if (lockHookRotation)
            {
                hook.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                hook.transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        protected override void Death()
        {
            Debug.Log("Player died!");
        }
    }
}
