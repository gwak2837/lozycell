using UnityEngine;

public class TouchControlsManager : MonoBehaviour
{
    [SerializeField]
    private GameObject touchControlsRoot;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            // 초기 상태 설정
            UpdateVisibility(GameManager.Instance.ShowVirtualControls);

            // 설정 변경 이벤트 구독
            GameManager.Instance.OnVirtualControlsSettingChanged += UpdateVisibility;
        }
        else
        {
            // GameManager가 없는 경우 (테스트 등) 모바일 여부로 판단
            UpdateVisibility(Application.isMobilePlatform);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnVirtualControlsSettingChanged -= UpdateVisibility;
        }
    }

    private void UpdateVisibility(bool show)
    {
        if (touchControlsRoot != null)
        {
            touchControlsRoot.SetActive(show);
        }
    }
}
