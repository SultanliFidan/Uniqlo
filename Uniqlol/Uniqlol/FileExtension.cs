namespace Uniqlol
{
    public static class FileExtension
    {
        public static bool IsValidType(this string contentType)
        {
            if(contentType.StartsWith("image")) return true;
            return false;
        }

        static bool IsValidSize(this int kb)
        {
            if(kb <= 2*1024) return true;
            return false;
        }

        static string Upload(this string  path)
        {
            string newFileName = Path.GetRandomFileName() + Path.GetExtension(path);
            return newFileName;

        }
    }
}
