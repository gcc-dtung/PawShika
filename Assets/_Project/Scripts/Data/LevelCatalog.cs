using UnityEngine;

[CreateAssetMenu(menuName = "PawShika/Level Catalog")]
public sealed class LevelCatalog : ScriptableObject
{
    [SerializeField] private LevelDefinition[] levels;

    public LevelDefinition[] Levels => levels;
}
