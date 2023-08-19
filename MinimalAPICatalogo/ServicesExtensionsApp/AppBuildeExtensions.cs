namespace MinimalAPICatalogo.ServicesExtensionsApp
{
    public static class AppBuildeExtensions
    {
        // Método de extenssão de IApplicationBuilder
        // Tratamento de excesssão
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app,
            IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            return app;
        }

        // Método de extenssão de IApplicationBuilder
        // Habilitação do CORS
        public static IApplicationBuilder UseAppCors(this IApplicationBuilder app)
        {
            app.UseCors(c =>
            {
                c.AllowAnyOrigin();
                c.WithMethods("GET");
                c.AllowAnyHeader();
            });

            return app;
        }
        // Método de extenssão de IApplicationBuilder
        // Habilitação do Suagger (Midlleware)
        public static IApplicationBuilder UseSwaggerMidlleware(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c=> {  });
            return app;
        }
    }
}
