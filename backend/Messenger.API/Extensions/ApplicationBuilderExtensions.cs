public static class ApplicationBuilderExtensions
{
    public static WebApplication UseApplicationMiddlewares(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Savrani Messenger API");
            options.DocumentTitle = "Savrani Messenger Swagger";
        });

        app.UseCors(cors => cors
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}