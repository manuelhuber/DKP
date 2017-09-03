using System.Collections.Generic;
using Movement;
using UnityEngine;
using UnityEngine.AI;

public class RaidManager : MonoBehaviour {
    public GameObject Rogue;
    public List<GameObject> RogueOutfits;
    public Transform SpawnPoint;
    public int PcCount = 5;

    private GameObject folder;
    private int count = 0;

    private void Awake() {
        folder = new GameObject {name = "PCs"};
        SpawnHero(Rogue, RogueOutfits, PcCount);
    }

    private void SpawnHero(GameObject prefab, List<GameObject> outfits, int amount) {
        for (var i = 0; i < amount; i++) {
            var spawn = SpawnPoint.transform.position + GetModification();
            var hero = Instantiate(prefab, spawn, Quaternion.identity);
            hero.transform.SetParent(folder.transform);
            var outfit = Instantiate(outfits[Random.Range(0, outfits.Count)], hero.transform);
            outfit.transform.SetParent(hero.transform);
            outfit.GetComponent<AnimationOnSpeed>().Agent = hero.GetComponent<NavMeshAgent>();
            count++;
        }
    }

    private Vector3 GetModification() {
        return new Vector3(count * 2, 0, 0);
    }
}