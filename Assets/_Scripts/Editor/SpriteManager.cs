using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

/// <summary>
/// Unity 2022+ 新版API 自动裁剪工具
/// 100%保留原图 + 1.75:1.5居中裁剪，无自动Trim，无警告
/// </summary>
public class SpriteAutoCropper
{
    // 目标裁剪比例 1.75:1.5 = 7:6
    private const float TARGET_ASPECT = 1.2656f / 2.25f;

    [MenuItem("Assets/自动裁剪Sprite(原比例+7:6)", false, 10)]
    public static void AutoCropSprite()
    {
        var textures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
        if (textures.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", "请选中图片纹理！", "确定");
            return;
        }

        foreach (var tex in textures) ProcessTexture(tex);
        EditorUtility.DisplayDialog("完成", "裁剪成功！", "确定");
    }

    static void ProcessTexture(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;

        // -------------------------- 1. 先获取原图尺寸（避免后续导入影响） --------------------------
        int w = texture.width;
        int h = texture.height;

        // -------------------------- 2. 关闭自动裁剪透明部分！ --------------------------
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.alphaIsTransparency = true;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        // 关键：关闭Unity自动裁剪透明背景，否则会把你的上下深色/透明部分裁掉！

        // 保存并重新导入，让基础设置生效
        importer.SaveAndReimport();

        // -------------------------- 3. 新版API设置精灵数据 --------------------------
        var dataProvider = new SpriteDataProviderFactories().GetSpriteEditorDataProviderFromObject(importer);
        dataProvider.InitSpriteEditorDataProvider();

        // 构建两个精灵区域
        var rects = new SpriteRect[2];

        // ① 原比例完整精灵：强制覆盖整个图片，不做任何裁剪
        rects[0] = new SpriteRect
        {
            name = $"{texture.name}_Original",
            rect = new Rect(0, 0, w, h),
            pivot = new Vector2(0.5f, 0.5f),
            alignment = SpriteAlignment.Center
        };

        // ② 1.75:1.5居中裁剪精灵
        var cropRect = CalculateCropRect(w, h, TARGET_ASPECT);
        rects[1] = new SpriteRect
        {
            name = $"{texture.name}_Cropped_7x6",
            rect = cropRect,
            pivot = new Vector2(0.5f, 0.5f),
            alignment = SpriteAlignment.Center
        };

        // 应用设置
        dataProvider.SetSpriteRects(rects);
        dataProvider.Apply();
        importer.SaveAndReimport();
    }

    /// <summary>
    /// 计算居中裁剪的Rect（y轴按Unity Sprite坐标处理，原点在左下角）
    /// </summary>
    static Rect CalculateCropRect(int srcW, int srcH, float targetAspect)
    {
        float srcAspect = (float)srcW / srcH;
        float cropW, cropH;

        if (srcAspect > targetAspect)
        {
            // 原图太宽，裁剪宽度，保留高度
            cropH = srcH;
            cropW = cropH * targetAspect;
        }
        else
        {
            // 原图太高，裁剪高度，保留宽度（你的剑图会走这个分支）
            cropW = srcW;
            cropH = cropW / targetAspect;
        }

        // 计算居中偏移：x是左右偏移，y是底部偏移（Unity Sprite Rect原点在左下角）
        float offsetX = (srcW - cropW) / 2f;
        float offsetY = (srcH - cropH) / 2f;

        return new Rect(offsetX, offsetY, cropW, cropH);
    }
}