using System.ComponentModel.Composition;
using System.Reflection;
using Acuerdo.Plugin;
using Inscribe.Plugin;

namespace OfficialMediaUploadPlugin
{
    [Export(typeof(IPlugin))]
    public class EntryPoint : IPlugin
    {
        public string Name
        {
            get { return "Official Media Upload Plugin"; }
        }

        public double Version
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return double.Parse(version.Major + "." + version.Minor);
            }
        }

        public void Loaded()
        {
            TwitterPictureResolver.Init();
            UploaderManager.RegisterUploader(new TwitterUploader());
        }

        public IConfigurator ConfigurationInterface
        {
            get { return null; }
        }
    }
}
