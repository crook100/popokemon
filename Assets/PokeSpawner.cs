using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeSpawner : MonoBehaviour
{
    [SerializeField]
    public Region[] regions;

    private GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    public void TrySpawnPokemon()
    {
        string current_city = gameController.GetCurrentCity();
        foreach (Region region in regions) 
        {
            if (region.name == current_city) 
            {
                float spawnRate = region.spawnRate;
                if (Random.Range(0f, 100f) <= spawnRate) 
                {
                    //Spawn pokemon
                    Debug.Log("Spawning pokemon");
                }
                break;
            }
        }
    }
}

[System.Serializable]
public class Region {
    public string name = "Unknown";
    [Range(0.0f, 100.0f)]
    public float spawnRate = 0;
    public string[] pokemonList = new string[] { };
}