using Essentials.Saving;
using MessagePack;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// List of objects that will be saved and loaded, but can't individually do so.
/// </summary>
public class ObjectSaveManager : Singleton<ObjectSaveManager>, ISaveable
{
    [SerializeField]
    private GameObject healthPotionPrefab = default;
    public Dictionary<Vector3Int, Vector3> SavedHPPotionPositions { get; } = new Dictionary<Vector3Int, Vector3>();

    protected override void Awake()
    {
        SaveSystem.Register(this);

        //RemoveExistingPotions();
    }

    //private static void RemoveExistingPotions()
    //{
    //    SAMI_HealthDrop[] existingHealthPotions = FindObjectsOfType<SAMI_HealthDrop>();
    //    for (int i = 0; i < existingHealthPotions.Length; i++)
    //    {
    //        Destroy(existingHealthPotions[i].gameObject);
    //    }
    //}

    public void AddHealthPotionPosition(Vector3Int key, Vector3 pos)
    {
        if (!SavedHPPotionPositions.ContainsKey(key))
        {
            SavedHPPotionPositions.Add(key, pos);
        }
    }

    public void RemoveHealthPotionPosition(Vector3Int key)
    {
        SavedHPPotionPositions.Remove(key);
    }

    #region Saving
    public SaveData GetSave()
    {
        Vector3[] healthPotionPositions = new Vector3[SavedHPPotionPositions.Count];
        int index = 0;
        foreach (Vector3 pos in SavedHPPotionPositions.Values)
        {
            healthPotionPositions[index] = pos;
            index++;
        }

        return new ObjectSaveManagerData(gameObject.name)
        {
            healthPotionPositions = healthPotionPositions
        };
    }

    public void Load(SaveData saveData)
    {
        if (saveData is ObjectSaveManagerData save)
        {
            for (int i = 0; i < save.healthPotionPositions.Length; i++)
            {
                CheckPosForExistingObjects<SAMI_HealthDrop>(save, i);
                GameObject spawnedHealthPotion = Instantiate(healthPotionPrefab);
                spawnedHealthPotion.transform.position = save.healthPotionPositions[i];
            }
        }
    }

    private static void CheckPosForExistingObjects<T>(ObjectSaveManagerData save, int i)
    {
        Collider[] objsInPos = Physics.OverlapSphere(save.healthPotionPositions[i], 0.25f);
        if (objsInPos.Length > 0)
        {
            for (int j = 0; j < objsInPos.Length; j++)
            {
                Collider obj = objsInPos[j];
                if (obj.TryGetComponent(out T _))
                {
                    obj.gameObject.SetActive(false);
                }
            }
        }
    }

    [Serializable, MessagePackObject]
    public class ObjectSaveManagerData : SaveData
    {
        public Vector3[] healthPotionPositions;

        public ObjectSaveManagerData() { }

        public ObjectSaveManagerData(string objName)
        {
            this.objName = objName;
        }
    }
    #endregion
}