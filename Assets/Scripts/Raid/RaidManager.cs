using System;
using System.Collections.Generic;
using Damage;
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
        public float SpaceBetweenAvatars = 10;

        private GameObject folder;
        private int count;

        private void Awake() {
            folder = new GameObject {name = "PCs"};
            SpawnHero(Heroes[0], PcCount);
        }

        private void SpawnHero(Hero hero, int amount) {
            var prefab = hero.Prefab;
            var outfits = hero.OutfitPrefabs;

            for (var i = 0; i < amount; i++) {
                Slider hp;
                InitializeAvater(hero.AvatarPrefab, out hp);
                var spawn = SpawnPoint.transform.position + GetModification();
                var heroInstance = Instantiate(prefab, spawn, Quaternion.identity);
                heroInstance.transform.SetParent(folder.transform);
                heroInstance.GetComponent<PlayerHealth>().AddHealthbar(hp);
                var outfit = Instantiate(outfits[Random.Range(0, outfits.Count)], heroInstance.transform);
                outfit.transform.SetParent(heroInstance.transform);
                count++;
            }
        }

        private void InitializeAvater(GameObject prefab, out Slider healthbar) {
            var canvas = GameObject.FindWithTag("Canvas");
            var avatar = Instantiate(prefab);
            avatar.transform.SetParent(canvas.transform);
            var rectTransform = avatar.GetComponent<RectTransform>();
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(0, 1);
            var pos = rectTransform.position;
            var offset = SpaceBetweenAvatars + (rectTransform.rect.height + SpaceBetweenAvatars) * count;
            pos.y = pos.y - offset;
            pos.x = SpaceBetweenAvatars;
            rectTransform.position = pos;
            healthbar = avatar.GetComponentInChildren<Slider>();
            avatar.GetComponentInChildren<Text>().text = Names[Random.Range(0, Names.Count)];
        }

        private Vector3 GetModification() {
            return new Vector3(count * 2, 0, 0);
        }
    }
}