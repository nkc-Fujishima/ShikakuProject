using UnityEngine;

public interface IDamage
{
    public void Damage() { Debug.Log("IDamage : Damage �f�t�H���g�������N��"); }

    public void Damage(Vector3 position) { Debug.Log("IDamage(Vector3 position) : Damage �f�t�H���g�������N��"); }
}
