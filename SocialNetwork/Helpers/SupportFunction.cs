using DocumentFormat.OpenXml.Math;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SocialNetwork.Helpers
{
    public class SupportFunction
    {
        public static string RemoveAccentsAndSpaces(string input){

            string normalizedString = input.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            

            return Regex.Replace(stringBuilder.ToString().Normalize(NormalizationForm.FormC), " ", ""); 
        }
    }
}
