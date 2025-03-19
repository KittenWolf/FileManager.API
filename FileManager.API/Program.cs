using FileManager.API.Abstractions;
using FileManager.API.Services;

namespace FileManager.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "CorsPolicy",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                            .WithMethods("POST");
                    });
            });

            builder.Services.AddSingleton<IFileCatalogReaderService, FileCatalogReaderService>();
            builder.Services.AddSingleton<IZipArchiveReaderService, ZipArchiveReaderService>();
            builder.Services.AddSingleton<IFileManagerService, FileManagerService>();

            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
