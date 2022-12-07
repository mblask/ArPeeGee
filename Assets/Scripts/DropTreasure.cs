using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTreasure : MonoBehaviour
{
    private static DropTreasure _instance;
    public static DropTreasure Instance
    {
        get
        {
            return _instance;
        }
    }

    public Vector2Int MinMaxDroppable;

    public Item TestItem;

    private float _dropFrequencyModifier = 1.0f;

    private float _seedSize = 100000.0f;

    [Header("Primary Drop Thresholds")]
    private float _commonDropFrequency = 5000.0f;
    private float _rareDropFrequency = 200.0f;

    [Header("Secondary Drop Chances")]
    private float _rareCommonDropFrequency = 25000.0f;
    private float _rareRareDropFrequency = 1250.0f;
    private float _normalUniqueDropFrequency = 10.0f;

    private Vector2 _dropOffset = Vector2.down;
    private List<Item> _treasure = new List<Item>();
    private List<Item> _droppableItemsPool = new List<Item>();

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _treasure = GameManager.Instance.Treasure;

        if (_treasure == null)
            Debug.LogWarning("There is no Treasure!!");
    }

    public void SetMinMaxDroppable(Vector2Int minmax)
    {
        if (minmax == Vector2Int.zero)
            return;

        MinMaxDroppable = minmax;
    }

    public void SetDropFrequencyModifier(float value)
    {
        if (value < 0.0f || value > 10.0f)
            return;

        _dropFrequencyModifier = value;
    }

    public void ResetDropFrequencyModifier()
    {
        _dropFrequencyModifier = 1.0f;
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

        //int index = Random.Range(0, _droppableItemsPool.Count);

        //Debug.Log(_droppableItemsPool.Count);
    }

    public void DropOneItem(Item item, Transform targetPosition)
    {
        if (item == null)
            return;

        float randomAngle = Random.Range(0.0f, 360.0f);

        GameObject itemClone = Instantiate(item.ItemPrefab, (Vector2)targetPosition.position + Vector2.down / 2.0f, Quaternion.AngleAxis(randomAngle, Vector3.forward));
        /**/
        itemClone.GetComponent<ItemPickup>().Item = item;
        itemClone.GetComponent<SpriteRenderer>().sprite = item.ItemImage;
        /**/
        itemClone.transform.SetParent(null);
    }

    public void DropItems(Transform targetPosition)
    {
        int itemsToDrop = 0;

        if (MinMaxDroppable.x > MinMaxDroppable.y || MinMaxDroppable.x < 0 || MinMaxDroppable.y < 0)
        {
            Debug.LogWarning("Droppable items Min and Max values are illogical");
            return;
        }

        if (MinMaxDroppable.x == MinMaxDroppable.y)
        {
            itemsToDrop = MinMaxDroppable.x;
        }
        else
        {
            itemsToDrop = Random.Range(MinMaxDroppable.x, MinMaxDroppable.y + 1);
        }

        if (itemsToDrop == 0)
        {
            //Drops nothing
            return;
        }

        for (int i = 0; i < itemsToDrop; i++)
        {
            RandomizeItems();

            if (_droppableItemsPool.Count == 0)
            {
                return;
            }

            int randomItemIndex = Random.Range(0, _droppableItemsPool.Count);

            Vector2 itemDisplacement = Mathf.Pow(-1.0f, i) * Vector2.right / 3.0f;

            float randomAngle = Random.Range(0.0f, 360.0f);

            GameObject itemClone = Instantiate(_droppableItemsPool[randomItemIndex].ItemPrefab, (Vector2)targetPosition.position + _dropOffset + itemDisplacement, 
                Quaternion.AngleAxis(randomAngle, Vector3.forward));
            /**/
            itemClone.GetComponent<ItemPickup>().Item = _droppableItemsPool[randomItemIndex];
            itemClone.GetComponent<SpriteRenderer>().sprite = _droppableItemsPool[randomItemIndex].ItemImage;
            /**/
            itemClone.transform.SetParent(null);
        }

        ResetDropFrequencyModifier();
    }

    public void SetDropOffset(Vector2 vector)
    {
        if (vector != Vector2.zero)
            _dropOffset = vector;
    }
}
