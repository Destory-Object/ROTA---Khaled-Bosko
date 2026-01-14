using UnityEngine;

public static class AttackUtilities
{
    public static Collider2D[] DetectEnemies(Vector2 position, float radius, LayerMask enemyLayers)
    {
        return Physics2D.OverlapCircleAll(position, radius, enemyLayers);
    }
}