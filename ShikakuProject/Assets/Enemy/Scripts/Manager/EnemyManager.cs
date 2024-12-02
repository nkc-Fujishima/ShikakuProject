using System;
using System.Collections.Generic;

public class EnemyManager : IEnemyListProvider
{
    private List<EnemyControllerBase> enemyList = new List<EnemyControllerBase>();

    public List<EnemyControllerBase> EnemyList => enemyList;

    public event Action OnEnemyDestroyHundle = null;
    public event Action OnClearHundle = null;

    // ���X�g�ɃG�l�~�[��ǉ�
    public void AddEnemy(EnemyControllerBase enemy)
    {
        if (enemy != null) enemyList.Add(enemy);
    }

    // �G�l�~�[���X�g��������ɑ���ꂽ�G�l�~�[���폜
    private void RemoveEnemy(EnemyControllerBase enemy)
    {
        enemyList.Remove(enemy);
        enemy.OnDestroyHundle -= RemoveEnemy;

        OnEnemyDestroyHundle?.Invoke();

        // �G�l�~�[���X�g���̗v�f��0�ɂȂ����ꍇ�A�Q�[���N���A�C�x���g�𔭉�
        if (enemyList.Count == 0) OnClearHundle?.Invoke();
    }

    // ���X�g�ɓo�^����Ă���G�l�~�[�̃Q�[���X�^�[�g���̏����ݒ���N��
    public void ExexuteEnemyStartMethod()
    {
        foreach(var enemy in enemyList)
        {
            enemy.OnDestroyHundle += RemoveEnemy;
            enemy.OnStart();
        }
    }

}
