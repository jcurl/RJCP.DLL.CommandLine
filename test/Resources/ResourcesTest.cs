namespace RJCP.Core.Resources
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using NUnit.Framework;

    [TestFixture]
    public class ResourcesPrintTest
    {
        private const string CmdLineStringResources = "RJCP.Core.Resources.CmdLineStrings";

        private static readonly CultureInfo[] Cultures = new CultureInfo[] {
            new CultureInfo("en"),
            new CultureInfo("en-US"),
            new CultureInfo("en-GB"),
            new CultureInfo("en-AU"),
            new CultureInfo("de"),
            new CultureInfo("de-DE"),
            new CultureInfo("de-AT"),
        };

        private static CultureInfo GetNeutralResourceLanguage()
        {
            Assembly assembly = typeof(CommandLine.Options).Assembly;
            if ((Attribute.GetCustomAttribute(assembly, typeof(NeutralResourcesLanguageAttribute))
                is NeutralResourcesLanguageAttribute lang)) {
                return new CultureInfo(lang.CultureName);
            }
            return null;
        }

        [Test]
        public void CheckNeutralResourcesLanguage()
        {
            CultureInfo neutral = GetNeutralResourceLanguage();
            Assert.That(neutral, Is.Not.Null, "Check AssemblyInfo has a NeutralResourcesLanguage");
        }

        [TestCaseSource(nameof(Cultures))]
        public void MissingResources(CultureInfo culture)
        {
            bool allTranslation =
                CheckMissingResources(CmdLineStringResources, typeof(CommandLine.Options).Assembly, culture);

            Assert.That(allTranslation, Is.True, "Some translations missing for {0}", culture.ToString());
        }

        private static bool CheckMissingResources(string baseName, Assembly assembly, CultureInfo culture)
        {
            ResourceManager rsrc = new ResourceManager(baseName, assembly);
            ResourceSet set = rsrc.GetResourceSet(culture, true, true);
            ResourceSet invariant = rsrc.GetResourceSet(GetNeutralResourceLanguage(), true, false);

            HashSet<string> found = new HashSet<string>();
            foreach (DictionaryEntry entry in set) {
                string key = entry.Key.ToString();
                if (entry.Value is string resource && !string.IsNullOrWhiteSpace(resource))
                    found.Add(key);
            }
            if (!culture.IsNeutralCulture) {
                ResourceSet parent = rsrc.GetResourceSet(culture.Parent, true, true);
                foreach (DictionaryEntry entry in parent) {
                    string key = entry.Key.ToString();
                    if (entry.Value is string resource && !string.IsNullOrWhiteSpace(resource))
                        found.Add(key);
                }
            }

            Console.WriteLine("Resource: {0}; Culture: {1}; Parent: {2}", baseName, culture.ToString(), culture.Parent.ToString());
            List<string> missing = new List<string>();
            foreach (DictionaryEntry entry in invariant) {
                string key = entry.Key.ToString();
                if (!found.Contains(key)) {
                    missing.Add(key);
                }
            }
            var sortedKeys = from key in missing orderby key select key;
            foreach (var key in sortedKeys) {
                Console.WriteLine("Missing Key: {0}", key);
            }

            return missing.Count == 0;
        }
    }
}
