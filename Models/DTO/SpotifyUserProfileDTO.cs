namespace sts_net.Models.DTO
{
    public class SpotifyUserProfileDTO
    {
        public required string UserId {  get; set; }
        public required string DisplayName { get; set; }
        public string ProfileImageUrl { get; set; }
        public required string ProfileUrl { get; set; }


    }
}
