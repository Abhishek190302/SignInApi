namespace SignInApi.Models
{
    public interface ITrackUserService
    {
        void TrackActiveUser(ActiveUserModel model);
    }
}