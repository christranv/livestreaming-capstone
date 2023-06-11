using Microsoft.OpenApi.Models;

namespace Team5.BuildingBlocks.Core.Swagger
{
    public class SwaggerOptions : OpenApiInfo
    {
        public string VersionName { get; set; } = "v1";
        public string RoutePrefix { get; set; } = "";
    }
}