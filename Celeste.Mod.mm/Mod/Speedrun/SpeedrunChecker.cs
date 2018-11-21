using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Security.Policy;
using System.Security.Cryptography;

namespace Celeste.Mod.Speedrun
{
    public static class SpeedrunChecker
    {
        private static readonly Lazy<bool> lazy = new Lazy<bool>(Check);

        public static bool IsGood => lazy.Value;

        public static bool Check() {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(e => e != Assembly.GetExecutingAssembly());
            var types = assemblies.SelectMany(e => {
                try {
                    return e.GetTypes();
                } catch {
                    return new Type[] {};
                }
            });
            var modules = types.Where(t => t.IsSubclassOf(typeof(EverestModule)));
            var mods = modules.Select(e => e.Assembly).Distinct();

            HashAlgorithm hasher = new SHA256CryptoServiceProvider();
            var allowedHashes = new string[] { "C12961C34CFB197C446D099A4F709A228AFA5EE5397FE66215F0960A686C0C56",
                                               "7F04CDD07EB35EB7D95EC6AEBFED0E9C782BD32B13DA049A3724DC929B42EE60",
                                               "4E5AA4A5E84B1C460500AC52242ECC4FD1EF3DFF3338491BC13D5EABF354C038" };
            var hashes = mods.Select(e => {
                var hash = BitConverter.ToString(new Hash(e).GenerateHash(hasher)).Replace("-", "");
                Console.WriteLine($"{e.FullName}: {hash} ({(allowedHashes.Contains(hash) ? "" : "NOT ")}ON WHITELIST)");
                return hash;
            });

            return hashes.All(e => allowedHashes.Contains(e));
        }
    }
}
