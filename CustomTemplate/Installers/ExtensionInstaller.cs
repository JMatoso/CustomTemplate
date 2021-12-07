namespace CustomTemplate.Installers
{ 
    public static class ExtensionInstaller 
    {
        public static void InstallServices(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceInstallers = typeof(Program).Assembly.ExportedTypes
                .Where(x => typeof(IServiceInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IServiceInstaller>()
                .ToList();

            serviceInstallers.ForEach(installer => installer.InstallServices(services, configuration));
        }

        public static void InstallExtensions(this WebApplication app)
        {
            var extensionInstaller = typeof(Program).Assembly.ExportedTypes
                .Where(x => typeof(IExtensionInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IExtensionInstaller>()
                .ToList();

            extensionInstaller.ForEach(installer => installer.InstallExtensions(app));
        }
    }
}