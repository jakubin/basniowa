using System.Net;
using Tests.Common;
using Tests.DataSets;
using Tests.Helpers;
using Tests.HttpAssert;
using Xunit;

namespace Tests.Integration.Shows
{
    public class On_Empty_Env_Action : EmptyEnvironmentTestsBase
    {
        private static readonly string BasePath = "Integration/Shows";
        private static readonly string SchemaBasePath = "Integration/Schemas";

        public On_Empty_Env_Action(EnvironmentFixture environment) : base(environment)
        {
        }

        [Fact(Skip = "Needs fixing")]
        public void Adding_Valid_Show_Should_Succeed()
        {
            using (var client = Environment.CreateClient())
            {
                client.AuthenticateAsDefaultAdmin();

                // add show
                long showId = 0;
                client
                    .PostAsJsonFromFile("/api/shows/create", $"{BasePath}/AddShow/01_add.request.json")
                    .ExpectStatusCode(HttpStatusCode.OK)
                    .ExpectJsonSchemaFromFile($"{SchemaBasePath}/api_shows_create.post.200.json")
                    .ProcessJsonContent(x => { showId = (long)x.id; });

                // get added show by id
                client
                    .GetAsJson($"/api/shows/{showId}")
                    .ExpectStatusCode(HttpStatusCode.OK)
                    .ExpectJsonSchemaFromFile($"{SchemaBasePath}/api_shows_(id).get.200.json")
                    .ExpectJsonContentFromFile(
                        $"{BasePath}/AddShow/get.response.json",
                        preprocessExpected: x => x.id = showId);
            }
        }
    }
}
