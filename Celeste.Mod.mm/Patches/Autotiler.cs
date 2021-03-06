﻿#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

using Celeste.Mod;
using Microsoft.Xna.Framework.Input;
using Monocle;
using MonoMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Celeste {
    class patch_Autotiler : Autotiler {

        public string Filename;

        public patch_Autotiler(string filename)
            : base(filename) {
            // no-op. MonoMod ignores this - we only need this to make the compiler shut up.
        }

        // Patching constructors is ugly.
        public extern void orig_ctor(string filename);
        [MonoModConstructor]
        public void ctor(string filename) {
            Filename = filename;
            orig_ctor(filename);
            Everest.Content.Process(this, filename);
        }

        private extern void orig_ReadInto(patch_TerrainType data, Tileset tileset, XmlElement xml);
        private void ReadInto(patch_TerrainType data, Tileset tileset, XmlElement xml) {
            orig_ReadInto(data, tileset, xml);

            if (xml.HasAttr("sound"))
                SurfaceIndex.TileToIndex[xml.AttrChar("id")] = xml.AttrInt("sound");
        }

        // Required because TerrainType is private.
        [MonoModIgnore]
        private class patch_TerrainType {
        }

    }
    public static class AutotilerExt {

        // Mods can't access patch_ classes directly.
        // We thus expose any new members through extensions.

        /// <summary>
        /// Get the filename of the file belonging to the Autotiler.
        /// </summary>
        // TODO: Is this the file path? What is this exactly?
        public static string GetFilename(this Autotiler self)
            => ((patch_Autotiler) self).Filename;

    }
}
