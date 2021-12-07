namespace CustomTemplate.Installers.Extensions
{
    public class ApplicationInstaller : IExtensionInstaller
    {
        public void InstallExtensions(WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/error-local-development");
#if (EnableSwaggerSupport)
                app.UseSwagger();
                app.UseSwaggerUI();
#endif
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseStaticFiles();

            app.UseCors(opt => {
                opt.AllowAnyHeader();
                opt.AllowAnyHeader();
                opt.SetIsOriginAllowed((host) => true);
                opt.AllowCredentials();
            });

            app.UseAuthorization();

            app.UseAuthentication();

            app.MapControllers();

            app.Run();
        }
    }
}