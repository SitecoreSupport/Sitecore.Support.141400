namespace Sitecore.Support.Mvc.ExperienceEditor.DatasourceValidator
{
  using Sitecore.Data;
  using System;
  public interface IDatasourceValidator
  {
    bool IsDatasourceValid(string dataSource, Database database);
  }
}