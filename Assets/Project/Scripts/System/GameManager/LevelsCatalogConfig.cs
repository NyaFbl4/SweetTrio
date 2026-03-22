using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Project.Scripts.GameManager
{
    [CreateAssetMenu(menuName = "Configs/Levels Catalog Config", fileName = "LevelsCatalogConfig")]
    public class LevelsCatalogConfig : ScriptableObject
    {
        [SerializeField] private List<LevelConfig> _levels = new();
        [SerializeField] private LevelConfig _defaultLevel;

#if UNITY_EDITOR
        [SerializeField] private bool _autoCollectInEditor = true;
        [SerializeField] private string[] _searchFolders = { "Assets/Project/Configs" };
#endif

        public IReadOnlyList<LevelConfig> Levels => _levels;
        public LevelConfig DefaultLevel => _defaultLevel;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!_autoCollectInEditor)
                return;

            CollectLevelsFromFolders();
        }

        [ContextMenu("Collect Levels From Folders")]
        private void CollectLevelsFromFolders()
        {
            var folders = _searchFolders != null && _searchFolders.Length > 0
                ? _searchFolders
                : new[] { "Assets" };

            var guids = AssetDatabase.FindAssets("t:LevelConfig", folders);
            var collectedLevels = new List<LevelConfig>(guids.Length);

            for (var i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var levelConfig = AssetDatabase.LoadAssetAtPath<LevelConfig>(path);
                if (levelConfig == null || collectedLevels.Contains(levelConfig))
                    continue;

                collectedLevels.Add(levelConfig);
            }

            collectedLevels.Sort((left, right) =>
            {
                if (left == null && right == null)
                    return 0;
                if (left == null)
                    return 1;
                if (right == null)
                    return -1;

                return string.Compare(left.name, right.name, global::System.StringComparison.Ordinal);
            });

            _levels = collectedLevels;

            if (_defaultLevel == null || !_levels.Contains(_defaultLevel))
            {
                _defaultLevel = _levels.Count > 0 ? _levels[0] : null;
            }

            EditorUtility.SetDirty(this);
        }
#endif
    }
}

