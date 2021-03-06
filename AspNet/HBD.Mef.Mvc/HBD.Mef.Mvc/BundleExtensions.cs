#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using HBD.Framework;
using HBD.Framework.Attributes;
using HBD.Framework.Core;
using HBD.Mef.Mvc.Optimization;

#endregion

namespace HBD.Mef.Mvc
{
    public static class BundleExtensions
    {
        private const string AreaPath = "~/Areas";
        private const string Http = "http";
        private static readonly string AreaVirtualPathFormat = $"{AreaPath}/{{0}}/{{1}}";
        private const string BinConfigFileVirtualPath = "~/bin/{0}.config";
        private const string WebConfig = "Web.config";
        private const string AreaDataToken = "area";

        internal static VirtualPathProvider VirtualPathProvider => HostingEnvironment.VirtualPathProvider;

        internal static string GetCurrentAreaName()
        {
            return HttpContext.Current?.Request.RequestContext.RouteData
                       .DataTokens[AreaDataToken] as string ?? string.Empty;
        }

        internal static IEnumerable<T> GetBundlesFor<T>() where T : Bundle
            => BundleTable.Bundles.GetRegisteredBundles().OfType<T>();

        internal static string GetBundleName([NotNull] string areaName)
            => string.Format(AreaVirtualPathFormat, areaName, Guid.NewGuid());

        /// <summary>
        ///     Get virtual path of Area config file: ~/Areas/AreaName/Web.config
        /// </summary>
        /// <param name="areaName"></param>
        /// <returns></returns>
        internal static string GetAreaConfigFile([NotNull] string areaName)
            => string.Format(AreaVirtualPathFormat, areaName, WebConfig);

        /// <summary>
        ///     Get virtual path of Module config file: ~/bin/ModuleName.config
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        internal static string GetBinConfigFile([NotNull] string moduleName)
            => string.Format(BinConfigFileVirtualPath, moduleName);

        /// <summary>
        ///     Get the Area virtual path of the file
        ///     Ex: path = ~/Content/area.css then the returns will be ~/Areas/[AreaName]/Content/area.css.
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static string NornalizeAreaVirtualPath([NotNull] string areaName, string path)
        {
            if (path.StartsWith(AreaPath, StringComparison.OrdinalIgnoreCase) &&
                path.ContainsIgnoreCase(areaName)) return path;
            return path.StartsWith(Http, StringComparison.OrdinalIgnoreCase)
                ? path
                : string.Format(AreaVirtualPathFormat, areaName, path.Replace("~/", string.Empty));
        }

        public static AreaScriptBundle AddAreaScriptBundle(this BundleCollection @this, [NotNull] string areaName)
        {
            Guard.ArgumentIsNotNull(areaName, nameof(areaName));
            var b = @this.OfType<AreaScriptBundle>().FirstOrDefault(i => i.AreaName.EqualsIgnoreCase(areaName))
                    ?? new AreaScriptBundle(areaName);
            @this.Add(b);
            return b;
        }

        public static AreaStyleBundle AddAreaStyleBundle(this BundleCollection @this, [NotNull] string areaName)
        {
            Guard.ArgumentIsNotNull(areaName, nameof(areaName));
            var b = @this.OfType<AreaStyleBundle>().FirstOrDefault(i => i.AreaName.EqualsIgnoreCase(areaName))
                    ?? new AreaStyleBundle(areaName);
            @this.Add(b);
            return b;
        }

        internal static IList<string> GetOrCreateScriptBundles(this WebViewPage @this)
        {
            if (@this.ViewBag.ScriptBundles is List<string> list) return list;

            list = new List<string>();
            @this.ViewBag.ScriptBundles = list;
            return list;
        }

        internal static IList<string> GetOrCreateStyleBundles(this WebViewPage @this)
        {
            if (@this.ViewBag.StyleBundles is List<string> list) return list;

            list = new List<string>();
            @this.ViewBag.StyleBundles = list;
            return list;
        }

        /// <summary>
        /// Register the dedicated script bundles for particular page. Calling this method and register needed bundles on top of page.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="bundleNames"></param>
        public static WebViewPage RegisterScriptBundles(this WebViewPage @this, params string[] bundleNames)
        {
            var list = @this.GetOrCreateScriptBundles();
            list.AddRange(bundleNames);
            return @this;
        }

        /// <summary>
        /// Register the dedicated style bundles for particular page. Calling this method and register needed bundles on top of page.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="bundleNames"></param>
        /// <returns></returns>
        public static WebViewPage RegisterStyleBundles(this WebViewPage @this, params string[] bundleNames)
        {
            var list = @this.GetOrCreateStyleBundles();
            list.AddRange(bundleNames);
            return @this;
        }
    }
}