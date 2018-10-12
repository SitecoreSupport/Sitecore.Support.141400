namespace Sitecore.Support.Mvc.ExperienceEditor.Pipelines.Response.RenderRendering
{
  using System;
  using Sitecore;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Support.Mvc.ExperienceEditor.DatasourceValidator;
  using Sitecore.Mvc.ExperienceEditor.Extensions;
  using Sitecore.Mvc.ExperienceEditor.Presentation;
  using Sitecore.Mvc.Pipelines.Response.RenderRendering;
  using Sitecore.Mvc.Presentation;
  using System.Runtime.CompilerServices;
  using Microsoft.Extensions.DependencyInjection;
  using Sitecore.DependencyInjection;

  class AddWrapper : Sitecore.Mvc.ExperienceEditor.Pipelines.Response.RenderRendering.AddWrapper
  {
    protected readonly IDatasourceValidator datasourceValidator;
    public AddWrapper() : this(ServiceProviderServiceExtensions.GetService<IDatasourceValidator>(ServiceLocator.ServiceProvider))
    {
    }

    public AddWrapper(IDatasourceValidator datasourceValidator)
    {
      this.datasourceValidator = new DatasourceValidator();
    }


    public override void Process(RenderRenderingArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      string str = (args.Rendering == null) ? null : args.Rendering.DataSource;
      PageContext pageContext = args.PageContext;
      Database database = null;
      if (pageContext != null)
      {
        Item item = pageContext.Item;
        database = ((item != null) ? (item.Database) : null);
      }

      if (database == null)
      {
        database = args.PageContext.Database;
      }

      if (((database != null) && !string.IsNullOrEmpty(str)) && ((database.Name != "core") && !this.datasourceValidator.IsDatasourceValid(str, database)))
      {
        Log.Warn(string.Format("'{0}' is not valid datasource for {1} or user does not have permissions to access.", str, database.Name), this);
        args.AbortPipeline();
      }
      else if ((!args.Rendered && (Context.Site != null)) && Context.PageMode.IsExperienceEditorEditing)
      {
        IMarker marker = this.GetMarker();
        if (marker != null)
        {
          int index = args.Disposables.FindIndex(x => x.GetType() == typeof(Wrapper));
          if (index < 0)
          {
            index = 0;
          }
          args.Disposables.Insert(index, new Wrapper(args.Writer, marker));
        }
      }
    }
  }
}