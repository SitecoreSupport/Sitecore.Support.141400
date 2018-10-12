namespace Sitecore.Support.Mvc.ExperienceEditor.DatasourceValidator
{
  using Sitecore;
  using Sitecore.ContentSearch;
  using Sitecore.ContentSearch.Utilities;
  using Sitecore.ContentSearch.SearchTypes;
  using Sitecore.Data;
  using Sitecore.Diagnostics;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class DatasourceValidator : IDatasourceValidator
  {
    private bool WasIpSet = false;
    public virtual bool IsDatasourceValid(string dataSource, Database database)
    {
      try
      {
        using (IProviderSearchContext context = ContentSearchManager.CreateSearchContext((Sitecore.ContentSearch.SitecoreIndexableItem)Context.Item))
        {
          IEnumerable<SearchStringModel> source = SearchStringModel.ParseDatasourceString(dataSource).ToList();
          if (source.Any())
          {
            return LinqHelper.CreateQuery<SearchResultItem>(context, source).Any();
          }
          return this.IsItem(dataSource, database);
        }
      }
      catch (Exception exception)
      {
        Log.Warn(string.Format("Failed to execute datasource query {0}", exception), this);
      }
      finally
      {
        if (WasIpSet)
        {
          Sitecore.Analytics.Tracker.Current.Interaction.Ip = null;
          WasIpSet = false;
        }
      }
      return false;
    }

    protected virtual bool IsItem(string datasource, Database database)
    {
      return (database.GetItem(datasource) != null);
    }
  }
}