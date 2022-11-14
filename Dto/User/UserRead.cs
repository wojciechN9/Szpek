namespace Szpek.Application.User
{
    public class UserRead
    {
        public string UserId { get; }

        public string UserName { get; }

        public string Email { get; }

        public long? SensorOwnerId { get; }

        public UserRead(string userId, string userName, string email, long? sensorOwnerId)
        {
            this.UserId = userId;
            this.UserName = userName;
            this.Email = email;
            this.SensorOwnerId = sensorOwnerId;
        }
    }
}
