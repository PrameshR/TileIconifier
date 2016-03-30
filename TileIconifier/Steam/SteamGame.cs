﻿#region LICENCE

// /*
//         The MIT License (MIT)
// 
//         Copyright (c) 2016 Johnathon M
// 
//         Permission is hereby granted, free of charge, to any person obtaining a copy
//         of this software and associated documentation files (the "Software"), to deal
//         in the Software without restriction, including without limitation the rights
//         to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//         copies of the Software, and to permit persons to whom the Software is
//         furnished to do so, subject to the following conditions:
// 
//         The above copyright notice and this permission notice shall be included in
//         all copies or substantial portions of the Software.
// 
//         THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//         IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//         FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//         AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//         LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//         OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//         THE SOFTWARE.
// 
// */

#endregion

using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Microsoft.Win32;
using TileIconifier.Properties;
using TileIconifier.Utilities;

namespace TileIconifier.Steam
{
    [Serializable]
    public class SteamGame : ListViewItem
    {
        private string _iconPath;

        protected SteamGame(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SteamGame(string appId, string gameName, string appManifestPath)
        {
            AppId = appId;
            GameName = gameName;
            AppManifestPath = appManifestPath;
            Text = AppId;
            SubItems.Add(GameName);
        }

        public string AppId { get; }
        public string GameName { get; }
        public string AppManifestPath { get; private set; }

        public string IconPath
        {
            get
            {
                if (_iconPath != null) return _iconPath;
                var defaultRegistryKey =
                    $@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Steam App {
                        AppId}";
                var defaultRegistryKey32BitOs =
                    $@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App {AppId}";

                _iconPath = (string) Registry.GetValue(defaultRegistryKey, "DisplayIcon", null) ??
                            (string) Registry.GetValue(defaultRegistryKey32BitOs, "DisplayIcon", null);
                return _iconPath;
            }
        }

        public byte[] IconAsBytes
        {
            get
            {
                if (IconPath == null) return ImageUtils.ImageToByteArray(Resources.SteamIco.ToBitmap());

                try
                {
                    using (var readImage = new FileStream(IconPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        return ImageUtils.ImageToByteArray(new Bitmap(readImage));
                }
                catch
                {
                    // ignored
                }
                return ImageUtils.ImageToByteArray(Resources.SteamIco.ToBitmap());
            }
        }

        public string GameExecutionArgument => $"-applaunch \"{AppId}\"";

        public override string ToString()
        {
            return AppId;
        }
    }
}