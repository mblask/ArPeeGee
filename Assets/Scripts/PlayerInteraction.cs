using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private static PlayerInteraction _instance;

    public static PlayerInteraction Instance
    {
        get
        {
            return _instance;
        }
    }

    private float _rightClickRaycastDistance = 0.01f;
    private Vector3 _destination;
    private bool _shouldMove;

    [SerializeField] private Interactable _focus;
    private PlayerController _playerController;
    private WeaponSlot _weaponSlot;
    private Camera _camera;

    private void Awake()
    {
        _instance = this;

        _camera = Camera.main;
    }

    private void Start()
    {
        _playerController = PlayerController.Instance;
        _weaponSlot = GetComponentInChildren<WeaponSlot>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 mouseInputPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseInputPosition, Vector2.up, _rightClickRaycastDistance);

            if (hit)
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    //interactable not null
                    //Debug.Log("Interactable is not null");

                    //clicked interactable not current interactable
                    //Debug.Log("Clicked interactable not current interactable");
                    if (interactable.transform.IsChildOf(_weaponSlot.transform))
                        return;

                    RemoveFocus();
                    SetFocus(interactable);

                    float distance = Vector2.Distance(transform.position, interactable.transform.position);
                    if (distance >= interactable.InteractionRadius)
                    {
                        //player is too far from interactable to interact
                        //Debug.Log("Too far from interactable, go to it");
                        _playerController.ShouldMove(true);
                        _playerController.SetDestination(interactable.transform.position);
                    }
                    else
                    {
                        //player is near enought to interact
                        //Debug.Log("Close enough to interactable, interact with it");
                        _playerController.StopMovement();
                        interactable.Interact();
                        RemoveFocus();
                    }
                }
            }
            else
            {
                //hit is false
                RemoveFocus();
            }
        }
    }

    public void SetFocus(Interactable newFocus)
    {
        if (newFocus != _focus)
        {
            if (_focus != null)
                _focus.OnDefocus();

            _focus = newFocus;
            _focus.OnFocus(transform);
        }
    }

    public void RemoveFocus()
    {
        if (_focus != null)
            _focus.OnDefocus();

        _focus = null;
    }
}
