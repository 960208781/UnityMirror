using System;
using System.IO;
using UnityEditor;

namespace Editor
{
    public static class PostInstallScript
    {
        private const string PackageId = "Packages/com.mirrornetworking.mirror";

        [InitializeOnLoadMethod]
        public static void Execute()
        {
            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(PackageId);
            if (packageInfo != null)
            {
                var packagePath = packageInfo.resolvedPath;
                if (DeleteExcept(Path.Combine(packagePath, "Mirror"), Path.Combine(packagePath, "Mirror", "Assets")))
                {
                    AssetDatabase.Refresh();
                }
            }
        }

        private static bool DeleteExcept(string targetPath, string preservePath)
        {
            var isDelete = false;
            if (!Directory.Exists(targetPath)) return false;
            var metaPath = $"{preservePath}.meta";
            if (!File.Exists(metaPath))
            {
                GenerateAMetaFile(metaPath);
            }
            foreach (var directory in Directory.GetDirectories(targetPath))
            {
                if (Path.GetFileName(directory) != Path.GetFileName(preservePath))
                {
                    isDelete = true;
                    Directory.Delete(directory, true);
                }
            }

            foreach (var file in Directory.GetFiles(targetPath))
            {
                if (Path.GetFileName(file) != Path.GetFileName(preservePath))
                {
                    isDelete = true;
                    File.Delete(file);
                }
            }

            return isDelete;
        }

        private static void GenerateAMetaFile(string metaFilePath)
        {
            var guid = Guid.NewGuid();
            var formattedGuid = guid.ToString("N");
            var contents = $@"
fileFormatVersion: 1
guid: {formattedGuid}
PackageManifestImporter:
externalObjects: {{}}
userData: 
assetBundleName: 
assetBundleVariant:";
            File.WriteAllText(metaFilePath, contents);
        }
    }
}