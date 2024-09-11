using System.Text.Json.Nodes;

namespace sts_net.Services.Interfaces
{
    public interface ISetlistService
    {
        public JsonObject GetSetlist(string setId);
        public List<String> GetSongTitlesFromSetlistResponse(JsonObject setlistServiceResponse);
    }
}
