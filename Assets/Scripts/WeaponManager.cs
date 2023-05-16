#region

using System.Collections.Generic;
using UnityEngine;

#endregion

public class WeaponManager : MonoBehaviour
{
    public enum GunType
    {
        Rifle = 0,
        Pistol = 1,
        Cannon = 2,
        None = 3
    }

    public abstract class Gun : MonoBehaviour
    {
        public Camera viewModel;

        public Transform cameraBone;

        public GameObject rightHand;

        public GameObject leftHand;

        public Transform tracerStart;

        public Transform leftHandCenter;

        public Animator animator;

        private Player player;

        public WeaponManager WeaponManager { get; set; }

        private void Start()
        {
            player = Game.OnStartResolve<Player>();
        }

        public abstract GunType GetGunType();

        public Vector3 GetTracerStartWorldPosition()
        {
            Vector3 position = viewModel.WorldToViewportPoint(tracerStart.position);
            position.z = 1.2f;
            return player.camera.ViewportToWorldPoint(position);
        }

        public void LeftWallStart()
        {
            if (animator.layerCount > 1 && animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 1f)
            {
                animator.Play("LeftWallTouch", 1, 0f);
            }
        }

        public void RightWallStart()
        {
            if (animator.layerCount > 1 && animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 1f)
            {
                animator.Play("RightWallTouch", 1, 0f);
            }
        }

        public void WallStop()
        {
            if (animator.layerCount > 1)
            {
                AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(1);
                if (currentAnimatorStateInfo.IsName("RightWallTouch"))
                {
                    animator.Play("RightWallTouchReverse", 1, 1f - Mathf.Clamp01(currentAnimatorStateInfo.normalizedTime));
                }

                if (currentAnimatorStateInfo.IsName("LeftWallTouch"))
                {
                    animator.Play("LeftWallTouchReverse", 1, 1f - Mathf.Clamp01(currentAnimatorStateInfo.normalizedTime));
                }
            }
        }

        public void Unequip()
        {
            animator.SetBool("Unequip", true);
        }

        public void UnequipFinished()
        {
            WeaponManager.cycleOnNextTimestep = true;
            animator.SetBool("Unequip", false);
        }
    }

    public List<Gun> guns;

    public Material tracerMaterial;

    private Dictionary<GunType, Dictionary<string, object>> parameters = new Dictionary<GunType, Dictionary<string, object>>();

    public GunType startGun;

    private Quaternion startRotation;

    private GunType? gunToEquip;

    private Player player;

    private bool cycleOnNextTimestep;

    public Gun EquippedGun { get; private set; }

    public void EquipGun(GunType? type)
    {
        if (EquippedGun != null)
        {
            if (type == EquippedGun.GetGunType())
            {
                return;
            }

            EquippedGun.Unequip();
        }

        gunToEquip = type;
    }

    public void WallStop()
    {
        if (EquippedGun != null)
        {
            EquippedGun.WallStop();
        }
    }

    public void LeftWallStart()
    {
        if (EquippedGun != null)
        {
            EquippedGun.LeftWallStart();
        }
    }

    public void RightWallStart()
    {
        if (EquippedGun != null)
        {
            EquippedGun.RightWallStart();
        }
    }

    private void Start()
    {
        player = Game.OnStartResolve<Player>();
        EquipGun(startGun);
    }

    private void FixedUpdate()
    {
        if ((!(EquippedGun == null) || !gunToEquip.HasValue) && !cycleOnNextTimestep)
        {
            return;
        }

        cycleOnNextTimestep = false;
        if (EquippedGun != null)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            AnimatorControllerParameter[] array = EquippedGun.animator.parameters;
            foreach (AnimatorControllerParameter animatorControllerParameter in array)
            {
                if (animatorControllerParameter.type == AnimatorControllerParameterType.Bool)
                {
                    dictionary[animatorControllerParameter.name] = EquippedGun.animator.GetBool(animatorControllerParameter.name);
                }

                if (animatorControllerParameter.type == AnimatorControllerParameterType.Float)
                {
                    dictionary[animatorControllerParameter.name] = EquippedGun.animator.GetFloat(animatorControllerParameter.name);
                }

                if (animatorControllerParameter.type == AnimatorControllerParameterType.Int)
                {
                    dictionary[animatorControllerParameter.name] = EquippedGun.animator.GetInteger(animatorControllerParameter.name);
                }
            }

            parameters[EquippedGun.GetGunType()] = dictionary;
            Destroy(EquippedGun.gameObject);
        }

        foreach (Gun gun in guns)
        {
            if (gun.GetGunType() != gunToEquip)
            {
                continue;
            }

            EquippedGun = Instantiate(gun, transform);
            if (parameters.ContainsKey(EquippedGun.GetGunType()))
            {
                foreach (KeyValuePair<string, object> item in parameters[EquippedGun.GetGunType()])
                {
                    if (item.Value is bool)
                    {
                        EquippedGun.animator.SetBool(item.Key, (bool)item.Value);
                    }

                    if (item.Value is float)
                    {
                        EquippedGun.animator.SetFloat(item.Key, (float)item.Value);
                    }

                    if (item.Value is int)
                    {
                        EquippedGun.animator.SetInteger(item.Key, (int)item.Value);
                    }
                }
            }

            EquippedGun.animator.Update(0f);
            EquippedGun.WeaponManager = this;
            startRotation = EquippedGun.cameraBone.localRotation;
            break;
        }
    }

    private void LateUpdate()
    {
        if (EquippedGun != null)
        {
            Vector3 eulerAngles = (EquippedGun.cameraBone.localRotation * Quaternion.Inverse(startRotation)).eulerAngles;
            player.cameraParent.localRotation = Quaternion.Euler(eulerAngles.y, eulerAngles.z, eulerAngles.x);
        }
    }
}