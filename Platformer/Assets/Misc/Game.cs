using System;
using System.IO;
using UnityEngine;

public static class Game
{
    public static class Layer
    {
        public static readonly LayerMask groundLayer = LayerMask.GetMask("Level Collidable");
        public static readonly LayerMask enemyLayer = LayerMask.GetMask("Enemies");
        public static readonly LayerMask ladderLayer = LayerMask.GetMask("Level Ladder");

        public static bool LayerMaskContainsLayer(LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }
    }

    public static class Save
    {
        public static readonly string primaryAttackLocation = "primaryAttackID";
        public static readonly string rangedAttackLocation = "rangedAttackID";
        public static readonly string dashAttackLocation = "dashAttackID";
        public static readonly string specialAttackLocation = "specialAttackID";

        public static T Get<T>(string path)
        {
            if (typeof(T) == typeof(int))
                return (T)(object)PlayerPrefs.GetInt(path);
            if (typeof(T) == typeof(float))
                return (T)(object)PlayerPrefs.GetFloat(path);
            if (typeof(T) == typeof(string))
                return (T)(object)PlayerPrefs.GetString(path);

            throw new ArgumentException($"Type {typeof(T)} is not supported.");
        }

        public static void Set<T>(string path, T value)
        {
            if (typeof(T) == typeof(int))
                PlayerPrefs.SetInt(path, (int)(object)value);
            if (typeof(T) == typeof(float))
                PlayerPrefs.SetFloat(path, (float)(object)value);
            if (typeof(T) == typeof(string))
                PlayerPrefs.SetString(path, (string)(object)value);
            else
                throw new ArgumentException($"Type {typeof(T)} is not supported by PlayerPrefs.");
            PlayerPrefs.Save();
        }

    }

    public static class Damager
    {
        public static void Damage(GameObject obj, float damage)
        {
            obj.TryGetComponent<IDamageable>(out var target);
            target?.TakeDamage(damage);
        }
    }

    public static class Logger
    {
        public static void Log(string message)
        {
            Debug.Log(message); // Optional: auch in Unity-Konsole
            File.AppendAllText(Application.persistentDataPath, $"[{System.DateTime.Now:HH:mm:ss}] - {message}\n");
        }
    }
}
