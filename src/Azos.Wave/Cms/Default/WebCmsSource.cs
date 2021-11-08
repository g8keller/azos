﻿/*<FILE_LICENSE>
 * Azos (A to Z Application Operating System) Framework
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Azos.Apps;
using Azos.Conf;
using Azos.Data;
using Azos.Client;
using Azos.Log;
using Azos.IO.FileSystem;
using Azos.Security;
using Azos.Web;
using System.Linq;
using Azos.Serialization.JSON;

namespace Azos.Wave.Cms.Default
{
  /// <summary>
  /// Provides implementation of CmsSource based on web calls to `CmsSourceFeeder` API
  /// </summary>
  public sealed class WebCmsSource : ApplicationComponent<ICmsFacade>, ICmsSource
  {
    public const string CONFIG_SERVICE_SECTION = "service";

    public WebCmsSource(ICmsFacade facade) : base(facade)
    {
    }

    private HttpService m_Service;

    public override string ComponentLogTopic => CoreConsts.CMS_TOPIC;

    [Config]
    public string RemoteAddress{  get; set; }


    public void Configure(IConfigSectionNode node)
    {
      ConfigAttribute.Apply(this, node);
      DisposeAndNull(ref m_Service);
      if (node == null) return;

      var nService = node[CONFIG_SERVICE_SECTION];
      m_Service = FactoryUtils.MakeDirectedComponent<HttpService>(this,
                                                                 nService,
                                                                 typeof(HttpService),
                                                                 new object[] { nService });
    }

    public async Task<Dictionary<string, IEnumerable<LangInfo>>> FetchAllLangDataAsync()
    {
      var (svc, adr) = ensureOperationalState();
      var result = await svc.Call(adr, nameof(ICmsSource), new ShardKey(0), async (http, ct) => {
        var map = await http.Client.GetJsonMapAsync("languages").ConfigureAwait(false);

        var dict = map.Where(kvp => kvp.Value is IJsonDataObject)
           .ToDictionary(kvp => kvp.Key,
                         kvp => ((List<object>)kvp.Value).OfType<JsonDataMap>()
                                                         .Select( item => new LangInfo(item["iso"].AsAtom(), item["name"].AsString())));
        return dict;
      }).ConfigureAwait(false);

      return result;
    }

    public async Task<Content> FetchContentAsync(ContentId id, Atom isoLang, DateTime utcNow, ICacheParams caching)
    {
      var (svc, adr) = ensureOperationalState();
      return null;//todo finish
    }


    private (HttpService service, string address) ensureOperationalState()
    {
      EnsureObjectNotDisposed();
      var svc = m_Service.NonDisposed("Configured service");
      var adr = RemoteAddress.NonBlank(nameof(RemoteAddress));
      return (svc, adr);
    }

  }
}
