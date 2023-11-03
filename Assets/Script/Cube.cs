using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class Cube : MonoBehaviour
{
    [Range(0.1f, 5f)]
    [SerializeField] 
    private float _Speed = 1f;

    private float _ElapsedTime = 0f;

    private float _DurationBetweenTicks = 1f;
    private float _Ratio;

    private Vector3 _FromPosition;
    private Vector3 _ToPosition;

    private Quaternion _FromRotation;
    private Quaternion _ToRotation;

    private Vector3 _MovementDirection;
    private Quaternion _MovementRotation;

    private float _RotationOffsetY;
    private float _CubeSide = 1;
    private float _CubeDiagonal;

    [SerializeField] private float _RayOffSetOutsideCube = .4f;
    private float _RaycastDistance = 0f;
    private Vector3 _RaycastDirection;
    private RaycastHit _Hit;

    private Vector3 _Down;

    private Action DoAction;
    // Start is called before the first frame update
    // Start is called before the first frame update
    void Start()
    {
        _CubeDiagonal = Mathf.Sqrt(2) * _CubeSide;
        _RotationOffsetY = _CubeDiagonal / 2 - _CubeSide / 2;

        _MovementDirection = transform.forward;
        _MovementRotation = Quaternion.AngleAxis(90f, transform.right);

        _RaycastDistance = _CubeSide / 2 + _RayOffSetOutsideCube;

        SetModeVoid();
    }

    // Update is called once per frame
    void Update()
    {
        Tick();
        DoAction();
    }

    private void Tick()
    {
        if (_ElapsedTime > _DurationBetweenTicks)
        {
            Debug.Log("Tick");
            SetModeMove();
            CheckCollision();
            _ElapsedTime = 0f;
        }
        _ElapsedTime += Time.deltaTime * _Speed;
        _Ratio = _ElapsedTime / _DurationBetweenTicks;
    }

    private void SetModeVoid()
    {
        DoAction = DoActionVoid;
    }

    private void DoActionVoid()
    {

    }

    private void InitNextMovement()
    {
        _FromPosition = transform.position;
        _FromRotation = transform.rotation;

        _ToPosition = _FromPosition + _MovementDirection;
        _ToRotation = _MovementRotation * _FromRotation;
    }

    private void SetModeMove()
    {
        InitNextMovement();
        DoAction = DoActionMove;
    }

    private void DoActionMove()
    {
        //3 Paramètre: StartValue, EndValue, ratio to interpolate between a & b
        transform.rotation = Quaternion.Lerp(_FromRotation, _ToRotation, _Ratio);
        transform.position = Vector3.Lerp(_FromPosition, _ToPosition, _Ratio) + (Vector3.up * (_RotationOffsetY * Mathf.Sin(Mathf.PI * _Ratio)));
    }

    private void CheckCollision()
    {
        _Down = Vector3.down;
        if (Physics.Raycast(transform.position, _Down, out _Hit, _RaycastDistance))
        {
            Debug.DrawRay(transform.position, _Down * _RaycastDistance, Color.red);

            GameObject lCollided = _Hit.collider.gameObject;

            Debug.Log(lCollided);

            if(lCollided.CompareTag("Ground"))
            {
                SetModeFall();
            }
        }
        else
        {
            SetModeFall();
        }
    }


    private void InitNextFallingMovement()
    {
        _FromPosition = transform.position;
        _ToPosition = _FromPosition + Vector3.down;
    }

    private void SetModeFall()
    {
        Debug.Log("SetModeFall");
        InitNextFallingMovement();
        DoAction = DoActionFall;
    }
    private void DoActionFall()
    {
        transform.position = Vector3.Lerp(_FromPosition, _ToPosition, _Ratio);
    }
}
