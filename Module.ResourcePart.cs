namespace Bars.Gkh.Ris
{
    using B4.ResourceBundling;

    public partial class Module
    {
        /// <summary>
        /// Метод регистрации бандлов
        /// </summary>
        private void RegisterBundlers()
        {
            var bundler = Container.Resolve<IResourceBundler>();

            bundler.RegisterCssBundle("b4-all", new[]
                {
                    "~/content/css/risMain.css"
                });

            bundler.RegisterScriptsBundle("external-libs", new[]
                {
                    "~/libs/B4/cryptopro/jsxmlsigner.js",
                    "~/libs/B4/cryptopro/xadessigner.js"
                });
        }
    }
}