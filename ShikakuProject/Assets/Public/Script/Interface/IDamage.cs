using UnityEngine;

public interface IDamage
{
    public void Damage() { Debug.Log("IDamage : Damage デフォルト実装が起動"); }

    public void Damage(Vector3 position) { Debug.Log("IDamage(Vector3 position) : Damage デフォルト実装が起動"); }
}
