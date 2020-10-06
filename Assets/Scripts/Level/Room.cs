﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public enum RoomType
{
    Default,
    Room,
    Corridor,
    Special,
    Boss,
    Treasure
}

[Serializable]
public class Room : MonoBehaviour
{
    public GameObject[] exits;
    public GameObject[] spawnZones;
    public bool isActive = false;
    public bool isCleared = false;
    private bool isPlayerInside = false;
    private List<GameObject> enemies = new List<GameObject>();
    public RoomType roomType;
    public int depth;
    public int minDepth;
    public GameObject[] lightingObjects;

    public GameObject minimapPrefab;

    private void Start()
    {
        foreach (var lightingObject in lightingObjects)
        {
            lightingObject.GetComponent<Light>().enabled = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateRoom();
        }
    }

    private void ActivateRoom()
    {
        var currActiveRoom = GameManager.Instance.activeRoom;
        if (currActiveRoom && !currActiveRoom.isCleared) return;
        GameManager.Instance.activeRoom = this;
        isActive = true;
        isPlayerInside = true;

        foreach (var o in spawnZones)
        {
            o.GetComponent<SpawnZone>().SetActive();
        }

        foreach (var lightingObject in lightingObjects)
        {
            lightingObject.GetComponent<Light>().enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateRoom();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }

    private void OnDrawGizmos()
    {
        var bounds = GetComponent<Collider>().bounds;
        var color = isCleared ? Color.cyan : (isActive ? Color.green : Color.red);
        color.a = 0.2f;
        Gizmos.color = color;
        Gizmos.DrawCube(bounds.center, bounds.size);
    }

    private void Update()
    {
        if (isActive && isCleared && !isPlayerInside)
            isActive = false;
        CheckCleared();
    }

    private void CheckCleared()
    {
        if (isCleared || !isActive) return;

        var spawnersDone = spawnZones.Length == 0 || spawnZones.All((o =>
        {
            var spawnZone = o.GetComponent<SpawnZone>();
            return !(spawnZone is null) && spawnZone.IsDone();
        }));

        var enemiesDead = enemies.Count(o => o != null) == 0;
        isCleared = spawnersDone && enemiesDead;

        if (isCleared)
            OpenAllDoors();
    }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    public void OpenAllDoors()
    {
        foreach (var o in exits)
        {
            var exit = o.GetComponent<RoomExit>();
            if (exit == null || !exit.isConnected)
            {
                // print("null exit or not connected");
            }
            else
            {
                // print("opening exit");
                exit.Open();
            }
        }
    }

    public void GenerateMinimapSprite()
    {
        var minimaps = FindObjectsOfType<SpriteRenderer>();
        
        var bounds = GetComponent<Collider>().bounds;
        print(bounds);
        var minimapIcon = Instantiate(minimapPrefab, transform);
        var spriteRenderer = minimapIcon.GetComponent<SpriteRenderer>();
        var spriteWidth = spriteRenderer.bounds.size.x;
        var spriteHeight = spriteRenderer.bounds.size.z;

        var roomWidth = bounds.size.x;
        var roomHeight = bounds.size.z;

        minimapIcon.transform.localScale = new Vector3(roomWidth / spriteWidth, roomHeight / spriteHeight, 1);
        minimapIcon.transform.position = bounds.center + Vector3.up;
    }
}