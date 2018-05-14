using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NPC
{
    public class NPCTypeInfo
    {
        public static class DefaultStatSection {
            public const string maxHp = "maxHp";
            public const string speed = "speed";
            public const string defense = "defense";
            public const string damage = "damage";
        }

        public int Id = 0;

        public string Name;
        public string Script;
        public string Prefab;

        public Dictionary<string, float> stats = new Dictionary<string, float>();

        public float GetStatValue(string statName) {
            float output = 0f;
            if (stats.TryGetValue(statName, out output)) {
                
            }
            return output;
        }

        public int GetMaxHp() {
            return (int)GetStatValue(DefaultStatSection.maxHp);
        }

        public float GetSpeed() {
            return GetStatValue(DefaultStatSection.speed) * GameStaticInfo.GameSpeedFactor;
        }

        public float GetDefense() {
            return GetStatValue(DefaultStatSection.defense);
        }

        public float GetDamage() {
            return GetStatValue(DefaultStatSection.damage);
        }
    }
}