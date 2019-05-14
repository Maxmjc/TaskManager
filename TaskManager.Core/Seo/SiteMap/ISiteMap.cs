using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Core.Seo.SiteMap
{
    public interface ISiteMap
    {
        List<SiteMap> GetSiteMapList();
    }
}
