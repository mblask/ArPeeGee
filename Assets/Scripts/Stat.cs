using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private float _baseValue = 0.0f;
    [SerializeField] private List<float> _modifiers = new List<float>();

    private bool _isTemporaryModified = false;
    public bool IsTemporaryModified => _isTemporaryModified;

    public float GetValue()
    {
        float finalValue = _baseValue;
        foreach (float modifier in _modifiers)
        {
            finalValue += modifier;
        }
        return finalValue;
    }

    public void AddModifier(float modifier)
    {
        if (modifier != 0.0f)
            _modifiers.Add(modifier);
    }

    public void RemoveModifier(float modifier)
    {
        if (modifier != 0.0f)
            _modifiers.Remove(modifier);
    }

    public void SetTemporaryModified(bool value)
    {
        _isTemporaryModified = value;
    }
}
