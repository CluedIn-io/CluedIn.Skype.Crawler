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
                { SkypeConstants.KeyName.Email, "ttr@cluedin.com" }, // TODO: REMOVE
                { SkypeConstants.KeyName.Password, "math*RIGH7smom4aif" } // TODO: REMOVE
            };
    }
  }
}
