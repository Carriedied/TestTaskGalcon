using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public int Ships { get; private set; }
    public float Radius { get; private set; }
    public bool IsPlayerControlled { get; private set; }

    private TMP_Text shipsText;
    [SerializeField] private float productionRate = 5f;
    [SerializeField] private GameObject selectionSprite;

    private static List<Vector2> planetPositions = new List<Vector2>();

    private void Start()
    {
        shipsText = GetComponentInChildren<TMP_Text>();
        if (IsPlayerControlled)
            InvokeRepeating(nameof(ProduceShips), 1f, 1f / productionRate);
    }

    private void Update()
    {
        shipsText.text = Ships.ToString();
    }

    public void Initialize(int initialShips, float size, bool isPlayerControlled)
    {
        Ships = initialShips;
        IsPlayerControlled = isPlayerControlled;
        SetSize(size);
        if (isPlayerControlled)
        {
            GetComponent<SpriteRenderer>().color = UnityEngine.Color.blue;
        }
    }

    private void ProduceShips()
    {
        Ships += 1;
    }

    public void SendShips(Planet targetPlanet)
    {
        if (!IsPlayerControlled || targetPlanet == this) return;

        int shipsToSend = Ships / 2;
        Ships -= shipsToSend;

        GameObject fleetObject = new GameObject("Fleet");
        Fleet fleet = fleetObject.AddComponent<Fleet>();
        fleet.Initialize(this, targetPlanet, shipsToSend);
    }

    public void ReceiveShips(int incomingShips)
    {
        if (incomingShips > Ships)
        {
            IsPlayerControlled = true;
            Ships = incomingShips - Ships;
            GetComponent<SpriteRenderer>().color = UnityEngine.Color.blue;
            CancelInvoke(nameof(ProduceShips));
            InvokeRepeating(nameof(ProduceShips), 1f, 1f / productionRate);
        }
        else
        {
            Ships -= incomingShips;
        }
    }

    private void SetSize(float size)
    {
        transform.localScale = Vector3.one * size;
        Radius = GetComponent<SpriteRenderer>().bounds.extents.x;
    }

    public static bool TryAddPlanetPosition(Vector2 position, float size, float minDistanceBetweenPlanets)
    {
        foreach (var existingPosition in planetPositions)
        {
            float distance = Vector2.Distance(existingPosition, position);
            if (distance < size * 2 + minDistanceBetweenPlanets)
            {
                return false;
            }
        }
        planetPositions.Add(position);
        return true;
    }

    public void Select()
    {
        if (selectionSprite != null)
        {
            selectionSprite.SetActive(true);
        }
    }

    public void Deselect()
    {
        if (selectionSprite != null)
        {
            selectionSprite.SetActive(false);
        }
    }
}
