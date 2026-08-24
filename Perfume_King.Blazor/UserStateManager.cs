namespace Perfume_King.Blazor
{
    public class UserStateManager
    {
        private static bool _isLoggedIn;

        public static bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                if (_isLoggedIn != value)
                {
                    _isLoggedIn = value;
                    OnStateChanged?.Invoke();
                }
            }
        }

        private static int cartItemsCount;

        public static int CartItemsCount
        {
            get => cartItemsCount;
            set
            {
                if (cartItemsCount != value)
                {
                    cartItemsCount = value;
                    OnStateChanged?.Invoke();
                }
            }
        }

        public static event Action? OnStateChanged;

        public static string? AccessToken { get; set; }

        public static void Logout()
        {
            AccessToken = null!;
            IsLoggedIn = false;
        }
    }
}
