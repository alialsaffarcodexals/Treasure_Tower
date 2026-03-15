using TreasureTower.Core;
using TreasureTower.Enemies;
using UnityEngine;
using UnityEngine.UI;

namespace TreasureTower.UI
{
    public sealed class MiniBossHudController : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Text bossNameText;
        [SerializeField] private Text healthText;
        [SerializeField] private Image healthFill;
        [SerializeField] private MiniBossController boss;

        private void OnEnable()
        {
            if (boss != null)
            {
                boss.HealthChanged += OnBossHealthChanged;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (boss != null)
            {
                boss.HealthChanged -= OnBossHealthChanged;
            }
        }

        private void Update()
        {
            Refresh();
        }

        private void OnBossHealthChanged(MiniBossController changedBoss)
        {
            Refresh();
        }

        private void Refresh()
        {
            var hasBoss = boss != null && !boss.IsDefeated;
            var isGameplayVisible = GameManager.Instance == null || GameManager.Instance.State == GameFlowState.Playing;
            if (panel != null)
            {
                panel.SetActive(hasBoss && isGameplayVisible);
            }

            if (!hasBoss || !isGameplayVisible)
            {
                return;
            }

            if (bossNameText != null)
            {
                bossNameText.text = boss.BossName;
            }

            if (healthText != null)
            {
                healthText.text = $"{boss.CurrentHealth}/{boss.MaxHealth}";
            }

            if (healthFill != null)
            {
                healthFill.fillAmount = boss.MaxHealth > 0 ? (float)boss.CurrentHealth / boss.MaxHealth : 0f;
            }
        }
    }
}
