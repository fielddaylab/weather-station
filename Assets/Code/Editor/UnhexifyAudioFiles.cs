using System.Collections.Generic;
using System.Globalization;
using System.IO;
using BeauUtil;
using BeauUtil.Debugger;
using BeauUtil.Editor;
using FieldDay.Scripting;
using Leaf;
using UnityEditor;
using UnityEngine;

static public class UnhexifyAudioFiles {
    static private Dictionary<uint, string> EnsureAllLineCodesLoaded() {
        var allLeafAssets = AssetDBUtils.FindAssets<LeafAsset>();
        var parser = ScriptNodePackage.Parser.Instance;

        Dictionary<uint, string> lineCodes = new Dictionary<uint, string>();
        foreach(var leaf in allLeafAssets) {
            string path = AssetDatabase.GetAssetPath(leaf);
            StringSlice parentDir = Path.GetDirectoryName(path).Replace('\\', '/');
            if (parentDir.EndsWith('/')) {
                parentDir = parentDir.Substring(0, parentDir.Length - 1);
            }
            parentDir = parentDir.Substring(parentDir.LastIndexOf('/') + 1);

            //Log.Msg(parentDir.ToString());

            //if (parentDir.Equals("Old", true)) {
            //    continue;
            //}

            Log.Msg("compiling '{0}'...", AssetDatabase.GetAssetPath(leaf));
            var pkg = LeafAsset.Compile(leaf, parser);

            foreach(var line in pkg.AllLines()) {
                string custom = pkg.GetLineCustomName(line.Key);
                if (custom != null) {
                    if (lineCodes.TryGetValue(line.Key.HashValue, out string existingLine)) {
                        if (existingLine != custom) {
                            Log.Warn("Conflicting line codes '{0}' ('{1}' vs '{2}')", line.Key.ToDebugString(), existingLine, custom);
                        } else {
                            Log.Warn("Line code '{0}' ('{1}') appears multiple times", line.Key.ToDebugString(), existingLine);
                        }
                    } else {
                        lineCodes.Add(line.Key.HashValue, custom);
                    }
                }
            }

            pkg.Clear();
        }

        return lineCodes;
    }

    [MenuItem("WeatherStation/Un-hex Streaming VO File Names")]
    static public void ConvertAllVOFiles() {
        var allLineCodes = EnsureAllLineCodesLoaded();

        var dirInfo = new DirectoryInfo("Assets/StreamingAssets/vo/en");
        if (!dirInfo.Exists) {
            Log.Error("directory doesn't exist!");
            return;
        }

        FileInfo[] allFiles = dirInfo.GetFiles();

        foreach(var file in allFiles) {
            string fileName = file.Name;

            string fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);
            if (uint.TryParse(fileNameNoExt, NumberStyles.HexNumber, null, out uint result)) {
                if (allLineCodes.TryGetValue(result, out string realName)) {
                    string newFileName = Path.ChangeExtension(realName, file.Extension);
                    string newPath = Path.Combine(file.DirectoryName, newFileName);
                    Log.Msg("renaming file '{0}' to '{1}'...", file.FullName, newPath);
                    File.Move(file.FullName, newPath);
                } else {
                    StringHash32 hash = new StringHash32(result);
                    string debugVersion = hash.ToDebugString();
                    Log.Msg(debugVersion);
                }
            }
        }

        AssetDatabase.Refresh();
    }
}