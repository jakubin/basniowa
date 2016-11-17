using System.Net;
using Tests.DataSets;
using Tests.Helpers;
using Tests.HttpAssert;
using Xunit;

namespace Tests.Integration.Shows
{
    public class On_Empty_Env : EmptyEnvironmentReadOnlyTestsBase
    {
        private static readonly string BasePath = "Integration/Shows";
        private static readonly string SchemaBasePath = "Integration/Schemas";

        public On_Empty_Env(EnvironmentFixture environment) : base(environment)
        {
        }

        [Fact]
        public void Get_Shows_Should_Return_Empty_List()
        {
            using (var client = Environment.CreateClient())
            {
                client
                    .GetAsJson("/api/shows")
                    .ExpectStatusCode(HttpStatusCode.OK)
                    .ExpectJsonSchemaFromFile($"{SchemaBasePath}/api_shows.get.200.json")
                    .ExpectJsonContentFromFile($"{BasePath}/GetEmptyShowsList/get.response.json");
            }
        }

        [Fact]
        public void Get_Show_Shoud_Return_404()
        {
            using (var client = Environment.CreateClient())
            {
                client
                    .GetAsJson("/api/shows/1")
                    .ExpectStatusCode(HttpStatusCode.NotFound);
            }
        }
    }
}
