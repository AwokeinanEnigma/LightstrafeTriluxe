#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class Rifle : WeaponManager.Gun
{
    public AudioClip fireSound;

    public AudioClip boltUp;

    public AudioClip boltBack;

    public AudioClip boltForward;

    public AudioClip boltDown;

    public List<GameObject> parts;

    public Transform barrel;

    public Transform center;

    public Transform stock;

    private const float crouchPositionSpeed = 4f;

    private float _upChange;

    private float _upSoften;

    private float _rightChange;

    private float _rightSoften;

    private float _forward;

    private Vector3 _prevVelocity;

    private float _crouchFactor;

    private float _crouchReloadMod;

    private Player player;

    private PlayerInput input;

    private AnimatorStateInfo _layer0Info;

    private AnimatorStateInfo _layer1Info;

    protected float leftHandFactor;

    private bool fireInputConsumed;

    private bool shotAvailable;

    private float aimFactor;

    private Vector3 toTargetVector;

    public bool UseSideGun
    {
        get
        {
            if (!player.jumpKitEnabled)
            {
                return false;
            }

            if (_layer0Info.IsName("Unequip"))
            {
                return false;
            }

            if (player.IsSliding)
            {
                return true;
            }

            return false;
        }
    }

    public override WeaponManager.GunType GetGunType()
    {
        return WeaponManager.GunType.Rifle;
    }

    private void Start()
    {
        player = Game.OnStartResolve<Player>();
        input = Game.OnStartResolve<PlayerInput>();
    }

    public void ReloadComplete()
    {
        animator.SetBool("Reload", false);
    }

    public void BoltUp()
    {
        player.AudioManager.PlayAudio(boltUp);
    }

    public void BoltBack()
    {
        player.AudioManager.PlayAudio(boltBack);
    }

    public void BoltForward()
    {
        player.AudioManager.PlayAudio(boltForward);
    }

    public void BoltDown()
    {
        player.AudioManager.PlayAudio(boltDown);
    }

    private void FixedUpdate()
    {
        if (!(animator == null))
        {
            _layer0Info = animator.GetCurrentAnimatorStateInfo(0);
            _layer1Info = animator.GetCurrentAnimatorStateInfo(1);
        }
    }

    private void Update()
    {
        if ((_layer1Info.normalizedTime <= 1f || _layer1Info.IsTag("Hold")) && _layer1Info.speed > 0f)
        {
            leftHandFactor = Mathf.Lerp(leftHandFactor, 1f, Time.deltaTime / 0.05f);
            if (_layer1Info.IsTag("Instant"))
            {
                leftHandFactor = 1f;
            }
        }
        else
        {
            leftHandFactor = Mathf.Lerp(leftHandFactor, 0f, Time.deltaTime / 0.25f);
        }

        animator.SetLayerWeight(1, leftHandFactor);
        if (player.IsOnGround)
        {
            shotAvailable = true;
        }

        shotAvailable = true;
        if (fireInputConsumed && !input.IsKeyPressed(PlayerInput.PrimaryInteract))
        {
            fireInputConsumed = false;
        }

        if (input.IsKeyPressed(PlayerInput.PrimaryInteract) && Time.timeScale > 0f && !animator.GetBool("Unequip") && !animator.GetBool("Reload") && !fireInputConsumed && shotAvailable)
        {
            shotAvailable = false;
            fireInputConsumed = true;
            player.AudioManager.PlayOneShot(fireSound);
            if (animator != null)
            {
                animator.Play("Fire", -1, 0f);
                animator.SetBool("Reload", true);
            }
        }
    }

    private void LateUpdate()
    {
        float yawIncrease = player.YawIncrease;
        Vector3 vector = player.velocity - _prevVelocity;
        if (UseSideGun)
        {
            if (_crouchFactor < 1f)
            {
                _crouchFactor += Time.deltaTime * 4f;
            }
        }
        else if (_crouchFactor > 0f)
        {
            _crouchFactor -= Time.deltaTime * 4f;
        }

        _crouchFactor = Mathf.Max(0f, Mathf.Min(1f, _crouchFactor));
        float num = (0f - (Mathf.Cos((float)Math.PI * _crouchFactor) - 1f)) / 2f;
        _upChange -= vector.y / 15f;
        if (!player.IsOnGround && !player.IsOnWall)
        {
            _upChange += Time.deltaTime * Mathf.Lerp(2f, 1f, num);
        }
        else
        {
            _upChange -= vector.y / Mathf.Lerp(25f, 50f, num);
        }

        _rightChange -= yawIncrease / 3f;
        _rightChange = Mathf.Lerp(_rightChange, 0f, Time.deltaTime * 20f);
        _upChange = Mathf.Lerp(_upChange, 0f, Time.deltaTime * 8f);
        _rightSoften = Mathf.Lerp(_rightSoften, _rightChange, Time.deltaTime * 20f);
        if (_upSoften > _upChange)
        {
            _upSoften = Mathf.Lerp(_upSoften, _upChange, Time.deltaTime * 10f);
        }
        else
        {
            _upSoften = Mathf.Lerp(_upSoften, _upChange, Time.deltaTime * 5f);
        }

        _upSoften = Mathf.Clamp(_upSoften, -1.3f, 1.3f);
        _prevVelocity = player.velocity;
        _forward = Mathf.Lerp(_forward, 0f, Time.deltaTime * 8f);
        float forward = _forward;
        float num2 = -0.02f * num;
        float num3 = Mathf.Lerp(0f, 0.02f, num);
        float num4 = _upSoften / 15f;
        float num5 = Mathf.Lerp(_rightSoften, 60f, num);
        float num6 = _rightSoften / Mathf.Lerp(10f, 5f, num);
        float num7 = _upSoften < 0f ? _upSoften * 10f : _upSoften;
        Vector3 up = center.up;
        Vector3 right = center.right;
        Vector3 forward2 = center.forward;
        num7 += 5f * _crouchReloadMod;
        num3 -= 0.02f * _crouchReloadMod;
        num2 -= 0.005f * _crouchReloadMod;
        num5 -= 5f * _crouchReloadMod;
        num5 += player.CameraRoll / 2f;
        Vector3 position = barrel.position;
        Vector3 position2 = center.position;
        Vector3 position3 = stock.position;
        foreach (GameObject part in parts)
        {
            part.transform.localPosition += new Vector3(forward, num2, num3);
            part.gameObject.transform.RotateAround(position, up, num5);
            part.gameObject.transform.RotateAround(position2, right, num6);
            part.gameObject.transform.RotateAround(position3, forward2, num7);
            part.transform.position += Vector3.up * num4;
        }

        rightHand.transform.localPosition += new Vector3(forward, num2, num3);
        rightHand.gameObject.transform.RotateAround(position, up, num5);
        rightHand.gameObject.transform.RotateAround(position2, right, num6);
        rightHand.gameObject.transform.RotateAround(position3, forward2, num7);
        rightHand.transform.position += Vector3.up * num4;
        float num8 = 1f - leftHandFactor;
        leftHand.transform.localPosition += new Vector3(forward, num2, num3 * num8);
        leftHand.gameObject.transform.RotateAround(position, up, num5 * num8);
        leftHand.gameObject.transform.RotateAround(position2, right, num6 * num8);
        leftHand.gameObject.transform.RotateAround(position3, forward2, num7 * num8);
        leftHand.transform.position += Vector3.up * num4;
    }

    private static Vector3 Flatten(Vector3 vec)
    {
        return new Vector3(vec.x, 0f, vec.z);
    }
}