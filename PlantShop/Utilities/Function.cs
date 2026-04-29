using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PlantShop.Utilities
{
    public class Function
    {
        public static int _AccountId = 0;
        public static string _Username = string.Empty;
        public static string _Password = string.Empty;
        public static string _Email = string.Empty;
        public static string _Message = string.Empty;
        public static string _MessageEmail = string.Empty;
        public static string _Role = string.Empty;

        public static string TitleSlugGenerationAlias(string Title)
        {
            return SlugGenerator.SlugGenerator.GenerateSlug(Title);
        }
        public static bool IsLogin()
        {
            if (string.IsNullOrEmpty(Function._Username) || Function._AccountId <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
