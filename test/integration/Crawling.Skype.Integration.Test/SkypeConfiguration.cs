using System.Collections.Generic;
using CluedIn.Crawling.Skype.Core;

namespace CluedIn.Crawling.Skype.Integration.Test
{
  public static class SkypeConfiguration
  {
    public static Dictionary<string, object> Create()
    {
      return new Dictionary<string, object>
            {
                { SkypeConstants.KeyName.email, "replace@this.com" },
                { SkypeConstants.KeyName.email, "hidden_secret123" }
            };
    }
  }
}
