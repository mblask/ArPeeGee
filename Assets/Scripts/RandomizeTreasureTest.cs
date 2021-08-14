using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeTreasureTest : MonoBehaviour
{
    public Vector2Int MinMaxDroppable;

    private float _seedSize = 100000.0f;

    [Header("Primary Drop Thresholds")]
    private float _commonDropFrequency = 5000.0f;
    private float _rareDropFrequency = 200.0f;

    [Header("Secondary Drop Chances")]
    private float _rareCommonDropFrequency = 25000.0f;
    private float _rareRareDropFrequency = 1250.0f;
    private float _normalUniqueDropFrequency = 10.0f;

    private float _dropFrequencyModifier = 1.0f;
    private List<Item> _treasure = new List<Item>();
    private List<Item> _droppableItemsPool = new List<Item>();

    private void Start()
    {
        _treasure = GameManager.Instance.Treasure;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            for (int i = 0; i < 1000; i++)
            {
                RandomizeItems();
            }
        }
    }

    public void RandomizeItems()
    {
        _droppableItemsPool.Clear();

        float randomDropChance = Random.Range(0.0f, _seedSize);

        //Debug.Log(randomDropChance);

        do
        {        
            if (randomDropChance <= _rareDropFrequency)
            {
                //Debug.Log("Unique drop");

                float uniqueDropChance = Random.Range(0.0f, _rareRareDropFrequency);

                if (uniqueDropChance <= _normalUniqueDropFrequency)
                {
                    //Debug.Log("Rare unique drop");

                    for (int i = 0; i < _treasure.Count; i++)
                    {
                        //Rare unique drop
                        if (_treasure[i].DropFrequency <= _normalUniqueDropFrequency * _dropFrequencyModifier)
                        {
                            //Debug.Log(_treasure[i].ItemName);
                            _droppableItemsPool.Add(_treasure[i]);
                        }
                    }
                }
                else if (uniqueDropChance > _normalUniqueDropFrequency && uniqueDropChance <= _rareDropFrequency)
                {
                    //Debug.Log("Normal unique drop");

                    for (int i = 0; i < _treasure.Count; i++)
                    {
                        //Normal unique drop
                        if (_treasure[i].DropFrequency > _normalUniqueDropFrequency * _dropFrequencyModifier && _treasure[i].DropFrequency <= _rareDropFrequency)
                        {
                            //Debug.Log(_treasure[i].ItemName);
                            _droppableItemsPool.Add(_treasure[i]);
                        }
                    }
                }
            }
            else if (randomDropChance > _rareDropFrequency && randomDropChance <= _commonDropFrequency)
            {
                //Debug.Log("Rare drop");

                float rareDropChance = Random.Range(_rareDropFrequency, _commonDropFrequency);

                //Debug.Log(rareDropChance);

                if (rareDropChance > _rareDropFrequency && rareDropChance <= _rareRareDropFrequency)
                {
                    //Debug.Log("Rare rare drop");

                    for (int i = 0; i < _treasure.Count; i++)
                    {
                        //Rare rare drop
                        if ((_treasure[i].DropFrequency > _rareDropFrequency * _dropFrequencyModifier && _treasure[i].DropFrequency <= _rareRareDropFrequency * _dropFrequencyModifier))
                        {
                            //Debug.Log(_treasure[i].ItemName);
                            _droppableItemsPool.Add(_treasure[i]);
                        }
                    }
                }
                else if (rareDropChance > _rareRareDropFrequency && rareDropChance <= _commonDropFrequency)
                {
                    //Debug.Log("Normal rare drop");

                    for (int i = 0; i < _treasure.Count; i++)
                    {
                        //Normal rare drop
                        if ((_treasure[i].DropFrequency > _rareRareDropFrequency * _dropFrequencyModifier && _treasure[i].DropFrequency <= _commonDropFrequency * _dropFrequencyModifier))
                        {
                            //Debug.Log(_treasure[i].ItemName);
                            _droppableItemsPool.Add(_treasure[i]);
                        }
                    }
                }
            }
            else
            {
                //Debug.Log("Common drop");

                float commonDropChance = Random.Range(_commonDropFrequency, _seedSize);

                //Debug.Log(commonDropChance);

                if (commonDropChance > _rareCommonDropFrequency)
                {
                    //Debug.Log("Normal common drop");

                    for (int i = 0; i < _treasure.Count; i++)
                    {
                        //Normal rare drop
                        if ((_treasure[i].DropFrequency > _rareCommonDropFrequency * _dropFrequencyModifier))
                        {
                            //Debug.Log(_treasure[i].ItemName);
                            _droppableItemsPool.Add(_treasure[i]);
                        }
                    }
                }
                else if (commonDropChance > _rareDropFrequency && commonDropChance <= _rareCommonDropFrequency)
                {
                    //Debug.Log("Rare common drop");

                    for (int i = 0; i < _treasure.Count; i++)
                    {
                        //Normal rare drop
                        if ((_treasure[i].DropFrequency > _rareDropFrequency * _dropFrequencyModifier && _treasure[i].DropFrequency <= _rareCommonDropFrequency * _dropFrequencyModifier))
                        {
                            //Debug.Log(_treasure[i].ItemName);
                            _droppableItemsPool.Add(_treasure[i]);
                        }
                    }
                }
            }
        } while (_droppableItemsPool.Count == 0);

        int index = Random.Range(0, _droppableItemsPool.Count);

        Debug.Log(_droppableItemsPool.Count);
    }
}
