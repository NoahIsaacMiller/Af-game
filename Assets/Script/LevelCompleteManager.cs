using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MyGame
{
    /// <summary>
    /// 关卡结束管理器 - 动态创建并管理游戏通关或失败界面
    /// </summary>
    public class LevelCompleteManager : MonoBehaviour
    {
        // UI组件引用
        private Canvas canvas;            // 主画布
        private GameObject panel;         // 主面板
        private TextMeshProUGUI titleText; // 标题文本
        private TextMeshProUGUI statsText; // 统计信息文本
        private Button nextLevelButton;   // 下一关/继续按钮
        private Button retryButton;       // 重试按钮
        private Button mainMenuButton;    // 主菜单按钮

        // UI 尺寸和间距常量
        private const float BUTTON_WIDTH = 400f;
        private const float BUTTON_HEIGHT = 80f;
        private const float BUTTON_VERTICAL_SPACING = 100f; // 按钮之间的垂直中心点间距

        // 小驼峰命名方法名
        public void Start()
        {
            // 初始化通关界面UI
            InitializeUI();
        }

        /// <summary>
        /// 初始化通关界面UI元素
        /// </summary>
        private void InitializeUI()
        {
            // 创建Canvas对象作为UI容器
            GameObject canvasObject = new GameObject("LevelEndCanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; // 根据屏幕尺寸缩放
            canvasScaler.referenceResolution = new Vector2(1920, 1080); // 参考分辨率
            canvasScaler.matchWidthOrHeight = 0.5f; // 平衡宽度和高度的缩放

            canvasObject.AddComponent<GraphicRaycaster>();
            canvas.sortingOrder = 100; // 设置高排序值确保界面显示在最前面

            // 创建主面板
            panel = new GameObject("LevelEndPanel");
            panel.transform.SetParent(canvas.transform, false);
            
            // 添加 CanvasGroup 用于统一控制面板的交互和射线阻挡
            CanvasGroup panelCanvasGroup = panel.AddComponent<CanvasGroup>();
            panelCanvasGroup.alpha = 1f; // 初始完全不透明
            panelCanvasGroup.interactable = true; // 初始可交互
            panelCanvasGroup.blocksRaycasts = true; // 初始阻挡射线

            // 设置面板背景为半透明黑色
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.7f); // 半透明黑色背景

            // 设置面板尺寸为全屏
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero; // 铺满整个Canvas

            // 初始隐藏面板
            panel.SetActive(false);

            // 创建标题文本
            CreateTitleText();

            // 创建统计信息文本
            CreateStatsText();

            // 创建操作按钮
            CreateButtons();
        }

        /// <summary>
        /// 创建标题文本元素
        /// </summary>
        private void CreateTitleText()
        {
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(panel.transform, false);

            titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.fontSize = 64; // 设置字体大小
            titleText.alignment = TextAlignmentOptions.Center; // 居中对齐
            titleText.color = Color.white; // 白色文本
            titleText.text = ""; // 初始文本会在 ShowLevelEndScreen 中设置

            // 设置文本位置和大小
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.8f); // 锚点在顶部中央
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 0.5f); // 轴心在中心
            titleRect.sizeDelta = new Vector2(800, 100); // 固定大小
            titleRect.anchoredPosition = new Vector2(0, -50); // 微调位置，向下偏移
        }

        /// <summary>
        /// 创建统计信息文本元素
        /// </summary>
        private void CreateStatsText()
        {
            GameObject statsObj = new GameObject("StatsText");
            statsObj.transform.SetParent(panel.transform, false);

            statsText = statsObj.AddComponent<TextMeshProUGUI>();
            statsText.fontSize = 36;
            statsText.alignment = TextAlignmentOptions.Center;
            statsText.color = Color.white;
            statsText.text = ""; // 默认文本会在 ShowLevelEndScreen 中设置

            // 设置文本位置和大小
            RectTransform statsRect = statsObj.GetComponent<RectTransform>();
            statsRect.anchorMin = new Vector2(0.5f, 0.65f); // 锚点在中央偏上
            statsRect.anchorMax = new Vector2(0.5f, 0.75f);
            statsRect.pivot = new Vector2(0.5f, 0.5f);
            statsRect.sizeDelta = new Vector2(600, 100); // 固定大小
            statsRect.anchoredPosition = new Vector2(0, -50); // 微调位置
        }

        /// <summary>
        /// 创建所有操作按钮
        /// </summary>
        private void CreateButtons()
        {
            // 按钮从屏幕中心向下排列
            float initialYOffset = 100f; // 第一个按钮中心距离屏幕中心向下偏移量

            // 创建下一关按钮
            nextLevelButton = CreateButton("NextLevelButton", "Next", new Vector2(0.5f, 0.5f), new Vector2(0, initialYOffset));
            nextLevelButton.onClick.AddListener(LoadNextLevel);

            // 创建重试按钮
            retryButton = CreateButton("RetryButton", "Retry", new Vector2(0.5f, 0.5f), new Vector2(0, initialYOffset - BUTTON_VERTICAL_SPACING));
            retryButton.onClick.AddListener(RetryLevel);

            // 创建主菜单按钮
            mainMenuButton = CreateButton("MainMenuButton", "Menu", new Vector2(0.5f, 0.5f), new Vector2(0, initialYOffset - BUTTON_VERTICAL_SPACING * 2));
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }

        /// <summary>
        /// 创建单个按钮及其文本
        /// </summary>
        /// <param name="name">按钮对象名称</param>
        /// <param name="text">按钮显示文本</param>
        /// <param name="anchor">按钮锚点 (例如 Vector2(0.5f, 0.5f) 表示中心)</param>
        /// <param name="anchoredPosition">按钮在锚点上的偏移量</param>
        /// <returns>创建好的按钮组件</returns>
        private Button CreateButton(string name, string text, Vector2 anchor, Vector2 anchoredPosition)
        {
            // 创建按钮对象
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(panel.transform, false);

            Button button = buttonObj.AddComponent<Button>();

            // 设置按钮背景颜色
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.15f, 0.45f, 0.15f); // 深绿色背景

            // 设置按钮大小和位置
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.anchorMin = anchor;
            buttonRect.anchorMax = anchor;
            buttonRect.pivot = new Vector2(0.5f, 0.5f); // 轴心在中心
            buttonRect.sizeDelta = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT); // 固定按钮大小
            buttonRect.anchoredPosition = anchoredPosition; // 设置在锚点上的偏移量

            // 添加按钮轮廓 (Outline)
            Outline buttonOutline = buttonObj.AddComponent<Outline>();
            buttonOutline.effectColor = Color.white; // 白色轮廓
            buttonOutline.effectDistance = new Vector2(2f, -2f); // 轮廓偏移量

            // 添加按钮阴影 (Shadow)
            Shadow buttonShadow = buttonObj.AddComponent<Shadow>();
            buttonShadow.effectColor = new Color(0f, 0f, 0f, 0.5f); // 半透明黑色阴影
            buttonShadow.effectDistance = new Vector2(5f, -5f); // 阴影偏移量

            // 添加按钮文本
            GameObject buttonTextObj = new GameObject("Text");
            buttonTextObj.transform.SetParent(buttonObj.transform, false);

            TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
            buttonText.fontSize = 32; // 调整字体大小以适应新按钮尺寸
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = Color.white;
            buttonText.text = text;

            // 设置文本填满按钮
            RectTransform textRect = buttonTextObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero; // 文本铺满按钮

            return button;
        }

        /// <summary>
        /// 显示关卡结束界面并更新统计信息（成功或失败）
        /// </summary>
        /// <param name="isSuccess">是否通关成功</param>
        /// <param name="score">玩家得分</param>
        /// <param name="timeTaken">关卡用时(秒)</param>
        /// <param name="collectedItems">收集物品数量</param>
        /// <param name="totalItems">总物品数量</param>
        public void ShowLevelEndScreen(bool isSuccess, int score, float timeTaken, int collectedItems, int totalItems)
        {
            panel.SetActive(true);
            Time.timeScale = 0f; // 暂停游戏时间

            // 设置标题文本
            titleText.text = isSuccess ? "Level Complete!" : "Level Failed!";
            titleText.color = isSuccess ? Color.green : Color.red; // 成功绿色，失败红色

            // 更新统计信息文本
            statsText.text = $"Score: {score}\n" +
                             $"Time consuming: {timeTaken:F1}s\n" +
                             $"Item: {collectedItems}/{totalItems}";

            // 根据是否成功和是否有下一关来设置下一关按钮
            if (isSuccess)
            {
                // 检查是否有下一关
                bool hasNextLevel = SceneManager.sceneCountInBuildSettings > SceneManager.GetActiveScene().buildIndex + 1;
                nextLevelButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next Level";
                nextLevelButton.interactable = hasNextLevel;
                nextLevelButton.gameObject.SetActive(true); // 确保按钮可见
            }
            else
            {
                nextLevelButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next Level"; // 保持文本，但禁用或隐藏
                nextLevelButton.interactable = false; // 失败时禁用下一关按钮
                nextLevelButton.gameObject.SetActive(false); // 失败时隐藏下一关按钮
            }
        }

        /// <summary>
        /// 加载下一关场景
        /// </summary>
        private void LoadNextLevel()
        {
            Debug.Log("加载下一关");
            Time.timeScale = 1f; // 恢复时间
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        /// <summary>
        /// 重新加载当前关卡
        /// </summary>
        private void RetryLevel()
        {
            Time.timeScale = 1f; // 恢复时间
            Debug.Log("重开");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// 返回主菜单场景
        /// </summary>
        private void ReturnToMainMenu()
        {
            Debug.Log("返回主菜单");
            Time.timeScale = 1f; // 恢复时间
            SceneManager.LoadScene("StartScene"); // 确保 "StartScene" 已添加到 Build Settings
        }
    }
}