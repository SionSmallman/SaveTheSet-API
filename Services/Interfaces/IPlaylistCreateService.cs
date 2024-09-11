using SpotifyAPI.Web;

namespace sts_net.Services.Interfaces
{
    public interface IPlaylistCreateService
    {
        PlaylistCreateRequest CreatePlaylistRequestObject(string title, string description, bool isPublic, bool isCollaborative);
    }
}