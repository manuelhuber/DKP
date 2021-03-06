﻿using System;
using System.Collections.Generic;
using System.Linq;
using Control;
using Damage.Common;
using PCs.Scripts;
using UI.Scripts;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class Hero {
    public CharacterBuilder Builder;
    public List<GameObject> OutfitPrefabs;
    public GameObject AvatarPrefab;
}

namespace Raid {
    public class RaidManager : MonoBehaviour {
        public GameObject HeroPrefab;
        public List<Hero> Heroes;
        public Transform SpawnPoint;
        public int PcCount = 5;
        public List<string> Names;

        private GameObject folder;
        private int count;

        private void Awake() {
            folder = new GameObject {name = "PCs"};
            Heroes.Take(PcCount).ToList().ForEach(h => SpawnHero(h, 1));
        }

        private void SpawnHero(Hero hero, int amount) {
            var outfits = hero.OutfitPrefabs;
            var avatarRenderer = RaidUi.GetAvatarRenderer();
            var mainControl = FindObjectOfType<MainControl>();

            for (var i = 0; i < amount; i++) {
                Slider hp;
                avatarRenderer.InitializeAvater(hero.AvatarPrefab, out hp, Names[Random.Range(0, Names.Count)]);
                var spawn = SpawnPoint.transform.position + GetModification();
                var heroInstance = Instantiate(HeroPrefab, spawn, Quaternion.identity);
                heroInstance = hero.Builder.MakeHero(heroInstance);
                heroInstance.transform.SetParent(folder.transform);
                heroInstance.GetComponent<PlayerHealth>().AddHealthbar(hp);
                var outfit = Instantiate(outfits[Random.Range(0, outfits.Count)], heroInstance.transform);
                outfit.transform.SetParent(heroInstance.transform);
                count++;
                if (mainControl == null) continue;
                mainControl.ControlGroups.Add(
                    count,
                    new List<MouseControllable> {heroInstance.GetComponent<MouseControllable>()}
                );
            }
        }


        private Vector3 GetModification() {
            return new Vector3(count * 2, 0, 0);
        }
    }
}