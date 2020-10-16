using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private CharacterController _controller;
    private Vector3 _direction;
    [SerializeField]
    private float _speed = 9.0f;
    [SerializeField]
    private float _jumpHeight = 12.0f;
    [SerializeField]
    private float _gravity = 30.0f;
    private Animator _anim;
    private bool _jumping = false;
    private bool _onLedge;
    private Ledge _activeLedge;

    private float _rollCompletePos = 14.5f;


    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (_onLedge == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                _anim.SetTrigger("ClimbUp");
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _anim.SetTrigger("Roll");
        }
    }

    void CalculateMovement()
    {
        // if grounded
        if (_controller.isGrounded == true)
        {
            if (_jumping == true)
            {
                _jumping = false;
                _anim.SetBool("Jumping", _jumping);
            }

            float horizontal = Input.GetAxisRaw("Horizontal");


            _direction = new Vector3(0, 0, horizontal) * _speed;
            _anim.SetFloat("Speed", Mathf.Abs(horizontal));

            if (horizontal != 0)
            {
                Vector3 facing = transform.localEulerAngles;
                facing.y = _direction.z > 0 ? 0 : 180;
                transform.localEulerAngles = facing;
            }

            if (horizontal > 0)
            {
                _rollCompletePos = 14.5f;
            }
            else if (horizontal < 0)
            {
                _rollCompletePos = -14.5f;
            }


            if (Input.GetKeyDown(KeyCode.Space))
            {
                _jumping = true;
                _direction.y += _jumpHeight;
                _anim.SetBool("Jumping", _jumping);
            }
        }

        _direction.y -= _gravity * Time.deltaTime;

        _controller.Move(_direction * Time.deltaTime);
    }

    public void GrabLedge(Vector3 handpos, Ledge currentLedge)
    {
        _controller.enabled = false;
        _anim.SetBool("GrabLedge", true);
        _anim.SetFloat("Speed", 0.0f);
        _anim.SetBool("Jumping", false);
        _onLedge = true;

        transform.position = handpos;
        _activeLedge = currentLedge;
    }

    public void ClimbUpComplete()
    {
        transform.position = _activeLedge.GetStandPos();
        _anim.SetBool("GrabLedge", false);
        _controller.enabled = true;

    }

    public void RollComplete()
    {
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + _rollCompletePos);
        this.transform.position = newPos;
    }
}
