using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject planetPrefab;
    [SerializeField] private int numberOfPlanets = 10;
    [SerializeField] private float minPlanetSize = 0.5f;
    [SerializeField] private float maxPlanetSize = 1.5f;
    [SerializeField] private float minDistanceBetweenPlanets = 1.0f;

    private Camera mainCamera;
    private Planet selectedPlanet;

    private void Start()
    {
        mainCamera = Camera.main;
        GenerateMap();
    }

    private void GenerateMap()
    {
        for (int i = 0; i < numberOfPlanets; i++)
        {
            Vector2 position;
            float planetSize;
            int attempts = 0;

            do
            {
                planetSize = UnityEngine.Random.Range(minPlanetSize, maxPlanetSize);
                position = GetRandomPositionWithinCameraBounds(planetSize);
                attempts++;
            } while (!Planet.TryAddPlanetPosition(position, planetSize, minDistanceBetweenPlanets) && attempts < 100);

            if (attempts >= 100)
            {
                continue;
            }

            var planetObject = Instantiate(planetPrefab, position, Quaternion.identity);
            var planet = planetObject.GetComponent<Planet>();
            bool isPlayerPlanet = i == 0;
            int initialShips = isPlayerPlanet ? 50 : UnityEngine.Random.Range(10, 30);
            planet.Initialize(initialShips, planetSize, isPlayerPlanet);

            if (isPlayerPlanet)
            {
                SelectPlanet(planet);
            }
        }
    }

    private Vector2 GetRandomPositionWithinCameraBounds(float size)
    {
        Vector3 cameraMin = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 cameraMax = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));
        float x = UnityEngine.Random.Range(cameraMin.x + size, cameraMax.x - size);
        float y = UnityEngine.Random.Range(cameraMin.y + size, cameraMax.y - size);
        return new Vector2(x, y);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                Planet clickedPlanet = hit.collider.GetComponent<Planet>();

                if (clickedPlanet != null)
                {
                    if (clickedPlanet.IsPlayerControlled)
                    {
                        SelectPlanet(clickedPlanet);
                    }
                    else if (selectedPlanet != null && selectedPlanet.IsPlayerControlled)
                    {
                        selectedPlanet.SendShips(clickedPlanet);
                    }
                }
            }
        }
    }

    private void SelectPlanet(Planet planet)
    {
        if (selectedPlanet != null)
        {
            selectedPlanet.Deselect();
        }

        selectedPlanet = planet;
        selectedPlanet.Select();
    }
}
