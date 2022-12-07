using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Interactable")]
    public float InteractionRadius = 1.0f;

    private Transform _focusedTransform;
    private bool _hasInteracted = false;
    private bool _isFocus = false;

    public bool HasInteracted
    {
        get
        {
            return _hasInteracted;
        }

        set
        {
            _hasInteracted = value;
        }
    }

    private void Update()
    {
        if (!_hasInteracted && _isFocus)
        {
            float distance = Vector2.Distance(transform.position, _focusedTransform.position);
            if (distance <= InteractionRadius)
            {
                //Interaction
                Interact();
                _hasInteracted = true;
            }
        }
    }

    //virtual method which can be modified (overriden) for each class/script which inherits Interactable class!
    public virtual void Interact()
    {

    }

    public void OnFocus(Transform playerTransform)
    {
        _isFocus = true;
        _focusedTransform = playerTransform;
    }

    public void OnDefocus()
    {
        _isFocus = false;
        _focusedTransform = null;
    }

    public void ResetInteraction()
    {
        _hasInteracted = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, InteractionRadius);
    }
}
