using UnityEngine;

namespace Mecha {
    [CreateAssetMenu(fileName = "LevelProperties", menuName = "Scriptable Objects/LevelProperties")]
    public class LevelProperties : ScriptableObject
    {
        public int levelNumber;
        public Sprite preview;
        public string header;
        public string description;
    }
}