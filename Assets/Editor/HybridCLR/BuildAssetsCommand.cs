
#if UNITY_EDITOR

using System.IO;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using Joy.Base.Config;
using UnityEditor;
using UnityEngine;

namespace Joy.EditorTools.HybridCLR
{
    /// <summary>
    /// HybridCLR移动DLL文件工具类
    /// </summary>
    public static class DllBytesBuildCommand
    {
        /// <summary>
        /// Dll文件资源根目录
        /// </summary>
        private static readonly string s_DllResRootPath = Application.dataPath + "/GameRes/Dlls";

        /// <summary>
        /// AOT Dll文件的目标路径
        /// </summary>
        private static readonly string s_AOTDllResDestPath = $"{s_DllResRootPath}/AOTDlls";

        /// <summary>
        /// Hotfix Dll文件的目标路径
        /// </summary>
        private static readonly string s_HotfixDllResDestPath = $"{s_DllResRootPath}/HotDlls";

        /// <summary>
        /// Dll文件描述文件
        /// </summary>
        private static readonly string s_DllManifestFile = $"{s_DllResRootPath}/DllManifest.json";

        /// <summary>
        /// HybridCLR移动Dll文件菜单工具入口
        /// </summary>
        [MenuItem("HybridCLR/Build/构建和移动Dll并更新Manifest")]
        public static void BuildAndCopyDLLs()
        {
            // 编译Dll
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            CompileDllCommand.CompileDll(target);
            // 将Dll文件从Hybrid的生成路径移动到游戏热更新资源路径下
            CopyAOTHotfixDlls(target);
            // 生成Dll集合描述文件到游戏热更新资源路径下
            GenerateDllManifestFile();
            // 刷新Unity资源库
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 拷贝Dll文件到游戏资源目录下，包括AOT Dll和Hotfix Dll
        /// </summary>
        /// <param name="target"></param>
        private static void CopyAOTHotfixDlls(BuildTarget target)
        {
            ClearAndEmptyFolder(s_AOTDllResDestPath);
            CopyAOTDllToGameRes(target);
            ClearAndEmptyFolder(s_HotfixDllResDestPath);
            CopyHotfixDllToGameRes(target);
        }

        /// <summary>
        /// 清空热更新资源目录下的文件
        /// </summary>
        /// <param name="folder"></param>
        private static void ClearAndEmptyFolder(string folder)
        {
            DirectoryInfo directory = new DirectoryInfo(folder);
            // 清空所有子目录
            DirectoryInfo[] subDirs = directory.GetDirectories("*.*", SearchOption.AllDirectories);
            foreach (var item in subDirs)
            {
                FileSystemInfo[] subFiles = item.GetFileSystemInfos();
                if (subFiles.Length == 0)
                {
                    File.Delete(item.FullName + ".meta");
                    item.Delete();
                }
            }
            // 清空当前目录下文件
            FileInfo[] files = directory.GetFiles();
            foreach (FileInfo file in files)
            {
                File.Delete(file.FullName);
            }
            // 刷新资源库
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 拷贝HybridCLR生成路径下的AOT Dll到游戏热更新资源目录下，重命名为.bytes后缀文件
        /// </summary>
        /// <param name="target"></param>
        private static void CopyAOTDllToGameRes(BuildTarget target)
        {
            string aotAssemblySrcPath = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
            foreach (var dll in SettingsUtil.AOTAssemblyNames)
            {
                string srcDllPath = $"{aotAssemblySrcPath}/{dll}.dll";
                if (!File.Exists(srcDllPath))
                {
                    Debug.LogError($"ab中添加AOT补充元数据dll:{srcDllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                    continue;
                }
                string destDllPath = $"{s_AOTDllResDestPath}/{dll}.dll.bytes";
                File.Copy(srcDllPath, destDllPath, true);
                Debug.Log($"[CopyAOTAssemblies] copy AOT dll {srcDllPath} -> {destDllPath}");
            }
        }

        /// <summary>
        /// 拷贝HybridCLR生成路径下的AOT Dll到游戏热更新资源目录下，重命名为.bytes后缀文件
        /// </summary>
        /// <param name="target"></param>
        private static void CopyHotfixDllToGameRes(BuildTarget target)
        {
            string hotAssemblySrcPath = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
            foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
            {
                string srcDllPath = $"{hotAssemblySrcPath}/{dll}";
                if (!File.Exists(srcDllPath))
                {
                    Debug.LogError($"ab中添加热更新dll:{srcDllPath} 时发生错误,文件不存在。请先构建热更新dll");
                    continue;
                }
                string destDllPath = $"{s_HotfixDllResDestPath}/{dll}.bytes";
                File.Copy(srcDllPath, destDllPath, true);
                Debug.Log($"[CopyHotfixAssemblies] copy hotfix dll {srcDllPath} -> {destDllPath}");
            }
        }

        /// <summary>
        /// 生成描述DLL的Manifest文件(Json格式化)
        /// </summary>
        private static void GenerateDllManifestFile()
        {
            DllManifest manifest = new DllManifest();
            // 获取配置中所有的AOT Dlls
            int aotDllCount = SettingsUtil.AOTAssemblyNames.Count;
            manifest.aotDlls = new string[aotDllCount];
            for (int i = 0; i < aotDllCount; ++i)
            {
                manifest.aotDlls[i] = $"{SettingsUtil.AOTAssemblyNames[i]}.dll";
            }
            // 获取HybridCLR配置中的所有Hotfix Dlls
            int hotfixDllCount = SettingsUtil.HotUpdateAssemblyFilesExcludePreserved.Count;
            manifest.hotfixDlls = new string[hotfixDllCount];
            for (int i = 0; i < hotfixDllCount; ++i)
            {
                manifest.hotfixDlls[i] = SettingsUtil.HotUpdateAssemblyFilesExcludePreserved[i];
            }
            // 生成Json数据，写入到文件中
            string jsonContent = JsonUtility.ToJson(manifest);
            File.WriteAllText(s_DllManifestFile, jsonContent, System.Text.Encoding.UTF8);
            Debug.Log($"[GenerateManifest] => {s_DllManifestFile}");
        }
    }
}

#endif
