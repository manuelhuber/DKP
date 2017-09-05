using System;
using System.Collections.Generic;
using Damage;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class Hero {
    public GameObject Prefab;
    public List<GameObject> OutfitPrefabs;
    public GameObject AvatarPrefab;
}

namespace Raid {
    public class RaidManager : MonoBehaviour {
        public List<Hero> Heroes;
        public Transform SpawnPoint;
        public int PcCount = 5;
        public List<string> Names;

        private GameObject folder;
        private int count;

        private void Awake() {
            folder = new GameObject {name = "PCs"};
            SpawnHero(Heroes[0], PcCount);
        }

        private void SpawnHero(Hero hero, int amount) {
            var prefab = hero.Prefab;
            var outfits = hero.OutfitPrefabs;
            var avatarRenderer = RaidUi.GetAvatarRenderer();

            for (var i = 0; i < amount; i++) {
                Slider hp;
                avatarRenderer.InitializeAvater(hero.AvatarPrefab, out hp, Names[Random.Range(0, Names.Count)]);
                var spawn = SpawnPoint.transform.position + GetModification();
                var heroInstance = Instantiate(prefab, spawn, Quaternion.identity);
                heroInstance.transform.SetParent(folder.transform);
                heroInstance.GetComponent<PlayerHealth>().AddHealthbar(hp);
                var outfit = Instantiate(outfits[Random.Range(0, outfits.Count)], heroInstance.transform);
                outfit.transform.SetParent(heroInstance.transform);
                count++;
            }
        }


        private Vector3 GetModification() {
            return new Vector3(count * 2, 0, 0);
        }
    }
}